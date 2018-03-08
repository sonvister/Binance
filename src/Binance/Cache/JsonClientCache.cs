using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Binance.Client;
using Binance.Utility;
using Microsoft.Extensions.Logging;

namespace Binance.Cache
{
    /// <summary>
    /// An abstract JSON client cache base class.
    /// </summary>
    /// <typeparam name="TClient"></typeparam>
    /// <typeparam name="TEventArgs"></typeparam>
    /// <typeparam name="TCacheEventArgs"></typeparam>
    public abstract class JsonClientCache<TClient, TEventArgs, TCacheEventArgs>
        : IJsonClientCache<TClient, TCacheEventArgs>
            where TClient : class, IJsonClient
            where TEventArgs : ClientEventArgs
            where TCacheEventArgs : CacheEventArgs
    {
        #region Public Events

        public event EventHandler<TCacheEventArgs> Update;

        #endregion Public Events

        #region Public Properties

        public TClient Client
        {
            get => _client;
            set => LinkTo(value);
        }

        public abstract IEnumerable<string> SubscribedStreams { get; }

        #endregion Public Properties

        #region Protected Fields

        protected readonly IBinanceApi Api;

        protected readonly ILogger<JsonClientCache<TClient, TEventArgs, TCacheEventArgs>> Logger;

        #endregion Protected Fields

        #region Private Fields

        private TClient _client;

        private Action<TCacheEventArgs> _callback;

        private QueuedProcessor<TEventArgs> _buffer;

        #endregion Private Fields

        #region Constructors

        protected JsonClientCache(IBinanceApi api, TClient client, ILogger<JsonClientCache<TClient, TEventArgs, TCacheEventArgs>> logger = null)
        {
            Throw.IfNull(api, nameof(api));
            Throw.IfNull(client, nameof(client));

            Api = api;
            _client = client;
            Logger = logger;
        }

        #endregion Constructors

        #region Public Methods

        public virtual void HandleMessage(string stream, string json)
            => _client.HandleMessage(stream, json);

        public abstract IJsonSubscriber Unsubscribe();

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Unsubscribe cache from client.
        /// </summary>
        protected abstract void UnsubscribeFromClient();

        /// <summary>
        /// Subscribe cache to client.
        /// </summary>
        protected abstract void SubscribeToClient();

        /// <summary>
        /// Abstract event action handler.
        /// </summary>
        /// <param name="event"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        protected abstract ValueTask<TCacheEventArgs> OnActionAsync(TEventArgs @event, CancellationToken token = default);

        /// <summary>
        /// Handle subscribe.
        /// </summary>
        /// <param name="callback"></param>
        protected virtual void OnSubscribe(Action<TCacheEventArgs> callback = null)
        {
            _callback = callback;
            _buffer = new QueuedProcessor<TEventArgs>(ProcessEventAsync);
        }

        /// <summary>
        /// Handle unsubscribe.
        /// </summary>
        protected virtual void OnUnsubscribe()
        {
            _callback = null;
            _buffer.Complete();
            _buffer = null;
        }

        /// <summary>
        /// Handle client event (provides buffering and single-threaded execution).
        /// </summary>
        /// <param name="event"></param>
        protected void ClientCallback(TEventArgs @event)
            => _buffer.Post(@event);

        #endregion Protected Methods

        #region Private Methods

        /// <summary>
        /// Link to a new client.
        /// </summary>
        /// <param name="client"></param>
        private void LinkTo(TClient client)
        {
            if (client == _client)
                return;

            // Unlink from existing client.
            UnsubscribeFromClient();

            // Update the client reference.
            _client = client;

            // Subscribe with new client.
            if (_client != null)
            {
                SubscribeToClient();
            }
        }

        /// <summary>
        /// Process received event arguments.
        /// </summary>
        /// <param name="event"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task ProcessEventAsync(TEventArgs @event, CancellationToken token = default)
        {
            TCacheEventArgs eventArgs = null;

            try
            {
                eventArgs = await OnActionAsync(@event, token)
                    .ConfigureAwait(false);
}
            catch (OperationCanceledException) { /* ignore */ }
            catch (Exception e)
            {
                Logger?.LogWarning(e, $"{GetType().Name}: Unhandled {nameof(OnActionAsync)} exception.  [thread: {Thread.CurrentThread.ManagedThreadId}]");
            }

            if (eventArgs != null)
            {
                try
                {
                    _callback?.Invoke(eventArgs);
                    Update?.Invoke(this, eventArgs);
                }
                catch (OperationCanceledException) { /* ignore */ }
                catch (Exception e)
                {
                    Logger?.LogWarning(e, $"{GetType().Name}: Unhandled update event handler exception.  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                }
            }
        }

        #endregion Private methods
    }
}
