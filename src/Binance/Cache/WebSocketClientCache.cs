using Binance.Api;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Binance.Cache
{
    public abstract class WebSocketClientCache<TClient, TEventArgs, TCacheEventArgs> : IDisposable
        where TClient : class, IDisposable 
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

        protected bool LeaveClientOpen;

        protected CancellationToken Token = CancellationToken.None;

        #endregion Protected Fields

        #region Private Fields

        private Action<TCacheEventArgs> _callback;

        private BufferBlock<TEventArgs> _bufferBlock;
        private ActionBlock<TEventArgs> _actionBlock;

        #endregion Private Fields

        #region Constructors

        protected WebSocketClientCache(IBinanceApi api, TClient client, bool leaveClientOpen = false, ILogger logger = null)
        {
            Throw.IfNull(api, nameof(api));
            Throw.IfNull(client, nameof(client));

            Api = api;
            Client = client;
            LeaveClientOpen = leaveClientOpen;
            Logger = logger;
        }

        #endregion Constructors

        #region Public Methods

        public void LinkTo(TClient client, Action<TCacheEventArgs> callback = null, bool leaveClientOpen = true)
        {
            Throw.IfNull(client, nameof(client));

            if (_bufferBlock != null)
            {
                if (client == Client)
                    throw new InvalidOperationException($"{GetType().Name} is already linked to this {nameof(TClient)}.");

                throw new InvalidOperationException($"{GetType().Name} is linked to another {nameof(TClient)}.");
            }

            _callback = callback;

            LeaveClientOpen = leaveClientOpen;

            _bufferBlock = new BufferBlock<TEventArgs>(new DataflowBlockOptions()
            {
                EnsureOrdered = true,
                CancellationToken = Token,
                BoundedCapacity = DataflowBlockOptions.Unbounded,
                MaxMessagesPerTask = DataflowBlockOptions.Unbounded,
            });

            _actionBlock = new ActionBlock<TEventArgs>(async @event =>
            {
                try
                {
                    var eventArgs = await OnAction(@event);

                    if (eventArgs != null)
                    {
                        _callback?.Invoke(eventArgs);
                        RaiseUpdateEvent(eventArgs);
                    }
                }
                catch (OperationCanceledException) { }
                catch (Exception e)
                {
                    Logger?.LogError(e, $"{GetType().Name}: \"{e.Message}\"");
                }
            }, new ExecutionDataflowBlockOptions()
            {
                BoundedCapacity = 1,
                EnsureOrdered = true,
                MaxDegreeOfParallelism = 1,
                CancellationToken = Token,
                SingleProducerConstrained = true,
            });

            _bufferBlock.LinkTo(_actionBlock);

            OnLinkTo();
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        protected abstract Task<TCacheEventArgs> OnAction(TEventArgs @event);

        /// <summary>
        /// 
        /// </summary>
        protected abstract void OnLinkTo();

        /// <summary>
        /// Raise cache update event.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void RaiseUpdateEvent(TCacheEventArgs args)
        {
            Throw.IfNull(args, nameof(args));

            try { Update?.Invoke(this, args); }
            catch (Exception e)
            {
                LogException(e, $"{GetType().Name}.{nameof(RaiseUpdateEvent)}");
                throw;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="event"></param>
        protected void OnClientEvent(object sender, TEventArgs @event)
        {
            // Post event to buffer block (queue).
            _bufferBlock.Post(@event);
        }

        /// <summary>
        /// Log an exception if not already logged within this library.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="source"></param>
        protected void LogException(Exception e, string source)
        {
            if (e.IsLogged()) return;
            Logger?.LogError(e, $"{source}: \"{e.Message}\"");
            e.Logged();
        }

        #endregion Protected Methods

        #region IDisposable

        private bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                if (!LeaveClientOpen)
                {
                    Client.Dispose();
                }

                _bufferBlock?.Complete();
                _actionBlock?.Complete();
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion IDisposable
    }
}
