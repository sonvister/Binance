using System;
using System.Threading;
using System.Threading.Tasks;
using Binance.Utility;
using Microsoft.Extensions.Logging;

namespace Binance.WebSocket.Manager
{
    internal abstract class BinanceWebSocketClientAdapter<TWebSocketClient> : IBinanceWebSocketClient, IBinanceWebSocketClientAdapter
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

        public Task Task => TaskCompletionSource?.Task ?? Task.CompletedTask;

        #endregion Public Properties

        #region Protected Properties & Fields

        protected ITaskController _controller => Manager.GetController(Client);

        protected readonly IBinanceWebSocketManager Manager;

        protected readonly TWebSocketClient Client;

        protected readonly ILogger<IBinanceWebSocketManager> Logger;

        protected readonly Action<Exception> OnError;

        protected TaskCompletionSource<bool> TaskCompletionSource;

        #endregion Protected Properties & Fields

        #region Private Fields

        private readonly object _sync = new object();

        #endregion Private Fields

        #region Constructors

        public BinanceWebSocketClientAdapter(IBinanceWebSocketManager manager, TWebSocketClient client, ILogger<IBinanceWebSocketManager> logger = null, Action<Exception> onError = null)
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
            CreateTaskCompletionSource();

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

                TaskCompletionSource.SetResult(true);
            }
            catch (OperationCanceledException) { /* ignored */ }
            catch (Exception e)
            {
                Logger?.LogError(e, $"{GetType().Name}.{nameof(UnsubscribeAll)}: Failed.  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                TaskCompletionSource.SetException(e);
                OnError?.Invoke(e);
            }
        }

        #endregion Public Methods

        #region Protected Methods

        protected void CreateTaskCompletionSource()
        {
            lock (_sync)
            {
                if (TaskCompletionSource != null && !TaskCompletionSource.Task.IsCompleted)
                {
                    throw new Exception($"{GetType().Name}.{nameof(CreateTaskCompletionSource)}: An asynchronous operation is already in progress.");
                }

                TaskCompletionSource = new TaskCompletionSource<bool>(TaskCreationOptions.None);
            }
        }

        #endregion Protected Methods
    }
}
