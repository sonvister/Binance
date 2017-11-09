using System;
using System.Threading;
using System.Threading.Tasks;
using Binance.Api;
using Microsoft.Extensions.Logging;

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

        private bool _isLinked;

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

            if (_isLinked)
            {
                if (client == Client)
                    throw new InvalidOperationException($"{GetType().Name} is already linked to this {Client.GetType().Name}.");

                throw new InvalidOperationException($"{GetType().Name} is linked to another {Client.GetType().Name}.");
            }

            _isLinked = true;

            _callback = callback;

            LeaveClientOpen = leaveClientOpen;

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
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="event"></param>
        protected async void OnClientEvent(object sender, TEventArgs @event)
        {
            try
            {
                var eventArgs = await OnAction(@event);
                if (eventArgs != null)
                {
                    try
                    {
                        _callback?.Invoke(eventArgs);
                        Update?.Invoke(this, eventArgs);
                    }
                    catch (OperationCanceledException) { }
                    catch (Exception e) { LogException(e, $"{GetType().Name} event handler"); }
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception e)
            {
                LogException(e, $"{GetType().Name}.{nameof(OnClientEvent)}");
            }
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
