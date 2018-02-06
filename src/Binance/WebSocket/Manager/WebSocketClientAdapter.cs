using System;
using System.Threading;
using Binance.Utility;
using Microsoft.Extensions.Logging;

namespace Binance.WebSocket.Manager
{
    internal abstract class WebSocketClientAdapter<TWebSocketClient> : IBinanceWebSocketClient
        where TWebSocketClient : IBinanceWebSocketClient
    {
        #region Public Events

        public event EventHandler<EventArgs> Open
        {
            add { Client.Open += value; }
            remove { Client.Open -= value; }
        }

        public event EventHandler<EventArgs> Close
        {
            add { Client.Close += value; }
            remove { Client.Close -= value; }
        }

        #endregion Public Events

        #region Public Properties

        public IWebSocketStream WebSocket => Client.WebSocket;

        #endregion Public Properties

        #region Protected Properties & Fields

        protected ITaskController _controller => Manager.GetController(Client);

        protected readonly IBinanceWebSocketClientManager Manager;

        protected readonly TWebSocketClient Client;

        protected readonly ILogger<IBinanceWebSocketClientManager> Logger;

        protected readonly Action<Exception> OnError;

        #endregion Protected Properties & Fields

        #region Constructors

        public WebSocketClientAdapter(IBinanceWebSocketClientManager manager, TWebSocketClient client, ILogger<IBinanceWebSocketClientManager> logger = null, Action<Exception> onError = null)
        {
            Throw.IfNull(manager, nameof(manager));
            Throw.IfNull(client, nameof(client));

            Manager = manager;
            Client = client;
            Logger = logger;
            OnError = onError;
        }

        #endregion Constructors

        #region Public Methods

        public async void UnsubscribeAll()
        {
            try
            {
                Logger?.LogDebug($"{GetType().Name}.{nameof(UnsubscribeAll)}: Cancel streaming...  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                await _controller.CancelAsync()
                    .ConfigureAwait(false);

                Client.UnsubscribeAll();

                if (!Manager.IsAutoStreamingDisabled && !_controller.IsActive)
                {
                    Logger?.LogDebug($"{GetType().Name}.{nameof(UnsubscribeAll)}: Begin streaming...  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                    _controller.Begin();
                }
            }
            catch (OperationCanceledException) { /* ignored */ }
            catch (Exception e)
            {
                Logger?.LogError(e, $"{GetType().Name}.{nameof(UnsubscribeAll)}: Failed.  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                OnError?.Invoke(e);
            }
        }

        #endregion Public Methods
    }
}
