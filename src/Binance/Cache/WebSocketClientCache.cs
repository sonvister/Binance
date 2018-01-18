using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Binance.Api;
using Binance.Api.WebSocket;
using Microsoft.Extensions.Logging;

namespace Binance.Cache
{
    public abstract class WebSocketClientCache<TClient, TEventArgs, TCacheEventArgs>
        where TClient : class, IBinanceWebSocketClient
        where TEventArgs : EventArgs
        where TCacheEventArgs : EventArgs
    {
        #region Public Events

        public event EventHandler<TCacheEventArgs> Update;

        #endregion Public Events

        #region Public Properties

        public TClient Client { get; }

        #endregion Public Properties

        #region Protected Fields

        protected readonly IBinanceApi Api;

        protected readonly ILogger Logger;

        protected CancellationToken Token = CancellationToken.None;

        #endregion Protected Fields

        #region Private Fields

        private Action<TCacheEventArgs> _callback;

        private BufferBlock<TEventArgs> _bufferBlock;
        private ActionBlock<TEventArgs> _actionBlock;

        private bool _isLinked;

        #endregion Private Fields

        #region Constructors

        protected WebSocketClientCache(IBinanceApi api, TClient client, ILogger logger = null)
        {
            Throw.IfNull(api, nameof(api));
            Throw.IfNull(client, nameof(client));

            Api = api;
            Client = client;
            Logger = logger;
        }

        #endregion Constructors

        #region Public Methods

        public virtual void LinkTo(TClient client, Action<TCacheEventArgs> callback = null)
        {
            Throw.IfNull(client, nameof(client));

            if (_isLinked)
            {
                if (client == Client)
                    throw new InvalidOperationException($"{GetType().Name} is already linked to this {Client.GetType().Name}.");

                throw new InvalidOperationException($"{GetType().Name} is linked to another {Client.GetType().Name}.");
            }

            _isLinked = true;

            _callback = callback;

            _bufferBlock = new BufferBlock<TEventArgs>(new DataflowBlockOptions
            {
                EnsureOrdered = true,
                CancellationToken = Token,
                BoundedCapacity = DataflowBlockOptions.Unbounded,
                MaxMessagesPerTask = DataflowBlockOptions.Unbounded
            });

            _actionBlock = new ActionBlock<TEventArgs>(async @event =>
            {
                TCacheEventArgs eventArgs = null;

                try
                {
                    eventArgs = await OnAction(@event)
                        .ConfigureAwait(false);
                }
                catch (OperationCanceledException) { }
                catch (Exception e)
                {
                    Logger?.LogError(e, $"{GetType().Name}: Unhandled {nameof(OnAction)} exception.");
                }

                if (eventArgs != null)
                {
                    try
                    {
                        _callback?.Invoke(eventArgs);
                        Update?.Invoke(this, eventArgs);
                    }
                    catch (OperationCanceledException) { }
                    catch (Exception e)
                    {
                        Logger?.LogError(e, $"{GetType().Name}: Unhandled update event handler exception.");
                    }
                }
            }, new ExecutionDataflowBlockOptions
            {
                BoundedCapacity = 1,
                EnsureOrdered = true,
                MaxDegreeOfParallelism = 1,
                CancellationToken = Token,
                SingleProducerConstrained = true
            });

            _bufferBlock.LinkTo(_actionBlock);

            client.WebSocket.WebSocket.Close += OnWebSocketClose;
        }

        private void OnWebSocketClose(object sender, EventArgs e)
        {
            UnLink();
        }

        public virtual void UnLink()
        {
            _callback = null;

            _isLinked = false;

            _bufferBlock?.Complete();
            _actionBlock?.Complete();
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Abstract event action handler.
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        protected abstract Task<TCacheEventArgs> OnAction(TEventArgs @event);

        /// <summary>
        /// Route event handler to callback method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="event"></param>
        protected void OnClientEvent(object sender, TEventArgs @event)
            => ClientCallback(@event);

        /// <summary>
        /// Handle client event (provides buffering and single-threaded execution).
        /// </summary>
        /// <param name="event"></param>
        protected void ClientCallback(TEventArgs @event)
            => _bufferBlock.Post(@event);

        #endregion Protected Methods
    }
}
