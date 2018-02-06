using System;
using System.Threading;
using Binance.WebSocket.Events;
using Microsoft.Extensions.Logging;

namespace Binance.WebSocket.Manager
{
    internal sealed class DepthWebSocketClientAdapter : BinanceWebSocketClientAdapter<IDepthWebSocketClient>, IDepthWebSocketClient
    {
        #region Public Events

        public event EventHandler<DepthUpdateEventArgs> DepthUpdate
        {
            add { Client.DepthUpdate += value; }
            remove { Client.DepthUpdate -= value; }
        }

        #endregion Public Events

        #region Constructors

        public DepthWebSocketClientAdapter(IBinanceWebSocketManager manager, IDepthWebSocketClient client, ILogger<IBinanceWebSocketManager> logger = null, Action<Exception> onError = null)
            : base(manager, client, logger, onError)
        { }

        #endregion Constructors

        #region Public Methods

        public async void Subscribe(string symbol, int limit, Action<DepthUpdateEventArgs> callback)
        {
            CreateTaskCompletionSource();

            try
            {
                Logger?.LogDebug($"{nameof(DepthWebSocketClientAdapter)}.{nameof(Subscribe)}: Cancel streaming...  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                await _controller.CancelAsync()
                    .ConfigureAwait(false);

                Client.Subscribe(symbol, limit, callback);

                if (!Manager.IsAutoStreamingDisabled && !_controller.IsActive)
                {
                    Logger?.LogDebug($"{nameof(DepthWebSocketClientAdapter)}.{nameof(Subscribe)}: Begin streaming...  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                    _controller.Begin();
                }

                TaskCompletionSource.SetResult(true);
            }
            catch (OperationCanceledException) { /* ignored */ }
            catch (Exception e)
            {
                Logger?.LogError(e, $"{nameof(DepthWebSocketClientAdapter)}.{nameof(Subscribe)}: Failed.  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                TaskCompletionSource.SetException(e);
                OnError?.Invoke(e);
            }
        }

        public async void Unsubscribe(string symbol, int limit, Action<DepthUpdateEventArgs> callback)
        {
            CreateTaskCompletionSource();

            try
            {
                Logger?.LogDebug($"{nameof(DepthWebSocketClientAdapter)}.{nameof(Unsubscribe)}: Cancel streaming...  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                await _controller.CancelAsync()
                    .ConfigureAwait(false);

                Client.Unsubscribe(symbol, limit, callback);

                if (!Manager.IsAutoStreamingDisabled && !_controller.IsActive)
                {
                    Logger?.LogDebug($"{nameof(DepthWebSocketClientAdapter)}.{nameof(Unsubscribe)}: Begin streaming...  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                    _controller.Begin();
                }

                TaskCompletionSource.SetResult(true);
            }
            catch (OperationCanceledException) { /* ignored */ }
            catch (Exception e)
            {
                Logger?.LogError(e, $"{nameof(DepthWebSocketClientAdapter)}.{nameof(Unsubscribe)}: Failed.  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                TaskCompletionSource.SetException(e);
                OnError?.Invoke(e);
            }
        }

        #endregion Public Methods
    }
}
