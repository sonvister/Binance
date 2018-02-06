using System;
using System.Threading;
using Binance.WebSocket.Events;
using Microsoft.Extensions.Logging;

namespace Binance.WebSocket.Manager
{
    internal sealed class SymbolStatisticsWebSocketClientAdapter : WebSocketClientAdapter<ISymbolStatisticsWebSocketClient>, ISymbolStatisticsWebSocketClient
    {
        #region Public Events

        public event EventHandler<SymbolStatisticsEventArgs> StatisticsUpdate
        {
            add { Client.StatisticsUpdate += value; }
            remove { Client.StatisticsUpdate -= value; }
        }

        #endregion Public Events

        #region Constructors

        public SymbolStatisticsWebSocketClientAdapter(IBinanceWebSocketManager manager, ISymbolStatisticsWebSocketClient client, ILogger<IBinanceWebSocketManager> logger = null, Action<Exception> onError = null)
            : base(manager, client, logger, onError)
        { }

        #endregion Constructors

        #region Public Methods

        public async void Subscribe(Action<SymbolStatisticsEventArgs> callback)
        {
            try
            {
                Logger?.LogDebug($"{nameof(SymbolStatisticsWebSocketClientAdapter)}.{nameof(Subscribe)}: Cancel streaming...  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                await _controller.CancelAsync()
                    .ConfigureAwait(false);

                Client.Subscribe(callback);

                if (!Manager.IsAutoStreamingDisabled && !_controller.IsActive)
                {
                    Logger?.LogDebug($"{nameof(SymbolStatisticsWebSocketClientAdapter)}.{nameof(Subscribe)}: Begin streaming...  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                    _controller.Begin();
                }
            }
            catch (OperationCanceledException) { /* ignored */ }
            catch (Exception e)
            {
                Logger?.LogError(e, $"{nameof(SymbolStatisticsWebSocketClientAdapter)}.{nameof(Subscribe)}: Failed.  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                OnError?.Invoke(e);
            }
        }

        public async void Subscribe(string symbol, Action<SymbolStatisticsEventArgs> callback)
        {
            try
            {
                Logger?.LogDebug($"{nameof(SymbolStatisticsWebSocketClientAdapter)}.{nameof(Subscribe)}: Cancel streaming...  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                await _controller.CancelAsync()
                    .ConfigureAwait(false);

                Client.Subscribe(symbol, callback);

                if (!Manager.IsAutoStreamingDisabled && !_controller.IsActive)
                {
                    Logger?.LogDebug($"{nameof(SymbolStatisticsWebSocketClientAdapter)}.{nameof(Subscribe)}: Begin streaming...  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                    _controller.Begin();
                }
            }
            catch (OperationCanceledException) { /* ignored */ }
            catch (Exception e)
            {
                Logger?.LogError(e, $"{nameof(SymbolStatisticsWebSocketClientAdapter)}.{nameof(Subscribe)}: Failed.  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                OnError?.Invoke(e);
            }
        }

        public async void Unsubscribe(Action<SymbolStatisticsEventArgs> callback)
        {
            try
            {
                Logger?.LogDebug($"{nameof(SymbolStatisticsWebSocketClientAdapter)}.{nameof(Unsubscribe)}: Cancel streaming...  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                await _controller.CancelAsync()
                    .ConfigureAwait(false);

                Client.Unsubscribe(callback);

                if (!Manager.IsAutoStreamingDisabled && !_controller.IsActive)
                {
                    Logger?.LogDebug($"{nameof(SymbolStatisticsWebSocketClientAdapter)}.{nameof(Unsubscribe)}: Begin streaming...  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                    _controller.Begin();
                }
            }
            catch (OperationCanceledException) { /* ignored */ }
            catch (Exception e)
            {
                Logger?.LogError(e, $"{nameof(SymbolStatisticsWebSocketClientAdapter)}.{nameof(Unsubscribe)}: Failed.  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                OnError?.Invoke(e);
            }
        }

        public async void Unsubscribe(string symbol, Action<SymbolStatisticsEventArgs> callback)
        {
            try
            {
                Logger?.LogDebug($"{nameof(SymbolStatisticsWebSocketClientAdapter)}.{nameof(Unsubscribe)}: Cancel streaming...  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                await _controller.CancelAsync()
                    .ConfigureAwait(false);

                Client.Unsubscribe(symbol, callback);

                if (!Manager.IsAutoStreamingDisabled && !_controller.IsActive)
                {
                    Logger?.LogDebug($"{nameof(SymbolStatisticsWebSocketClientAdapter)}.{nameof(Unsubscribe)}: Begin streaming...  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                    _controller.Begin();
                }
            }
            catch (OperationCanceledException) { /* ignored */ }
            catch (Exception e)
            {
                Logger?.LogError(e, $"{nameof(SymbolStatisticsWebSocketClientAdapter)}.{nameof(Unsubscribe)}: Failed.  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                OnError?.Invoke(e);
            }
        }

        #endregion Public Methods
    }
}
