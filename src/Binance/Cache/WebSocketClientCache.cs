using System;
using System.Threading;
using System.Threading.Tasks;
using Binance.Api;
using Microsoft.Extensions.Logging;

namespace Binance.Cache
{
    public abstract class WebSocketClientCache<TClient, TEventArgs, TCacheEventArgs>
        where TClient : class 
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

        public void LinkTo(TClient client, Action<TCacheEventArgs> callback = null)
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

            OnLinkTo();
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// 
        /// </summary>
        protected abstract void OnLinkTo();

        /// <summary>
        /// 
        /// </summary>
        /// <param name="event"></param>
        /// <returns></returns>
        protected abstract Task<TCacheEventArgs> OnAction(TEventArgs @event);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="event"></param>
        protected async void OnClientEvent(object sender, TEventArgs @event)
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
        }

        #endregion Protected Methods
    }
}
