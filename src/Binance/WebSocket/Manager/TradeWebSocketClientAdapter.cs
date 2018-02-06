using System;
using System.Threading;
using Binance.WebSocket.Events;
using Microsoft.Extensions.Logging;
// ReSharper disable InconsistentlySynchronizedField

namespace Binance.WebSocket.Manager
{
    internal sealed class TradeWebSocketClientAdapter : BinanceWebSocketClientAdapter<ITradeWebSocketClient>, ITradeWebSocketClient
    {
        #region Public Events

        public event EventHandler<TradeEventArgs> Trade
        {
            add => Client.Trade += value;
            remove => Client.Trade -= value;
        }

        #endregion Public Events

        #region Constructors

        public TradeWebSocketClientAdapter(IBinanceWebSocketManager manager, ITradeWebSocketClient client, ILogger<IBinanceWebSocketManager> logger = null, Action<Exception> onError = null)
            : base(manager, client, logger, onError)
        { }

        #endregion Constructors

        #region Public Methods

        public void Subscribe(string symbol, Action<TradeEventArgs> callback)
        {
            lock (Sync)
            {
                Task = Task.ContinueWith(async _ =>
                {
                    try
                    {
                        Logger?.LogDebug($"{nameof(TradeWebSocketClientAdapter)}.{nameof(Subscribe)}: Cancel streaming...  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                        await Controller.CancelAsync()
                            .ConfigureAwait(false);

                        Client.Subscribe(symbol, callback);

                        if (!Manager.IsAutoStreamingDisabled && !Controller.IsActive)
                        {
                            Logger?.LogDebug($"{nameof(TradeWebSocketClientAdapter)}.{nameof(Subscribe)}: Begin streaming...  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                            Controller.Begin();
                        }
                    }
                    catch (OperationCanceledException) { /* ignored */ }
                    catch (Exception e)
                    {
                        Logger?.LogError(e, $"{nameof(TradeWebSocketClientAdapter)}.{nameof(Subscribe)}: Failed.  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                        OnError?.Invoke(e);
                    }
                });
            }
        }

        public void Unsubscribe(string symbol, Action<TradeEventArgs> callback)
        {
            lock (Sync)
            {
                Task = Task.ContinueWith(async _ =>
                {
                    try
                    {
                        Logger?.LogDebug($"{nameof(TradeWebSocketClientAdapter)}.{nameof(Unsubscribe)}: Cancel streaming...  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                        await Controller.CancelAsync()
                            .ConfigureAwait(false);

                        Client.Unsubscribe(symbol, callback);

                        if (!Manager.IsAutoStreamingDisabled && !Controller.IsActive)
                        {
                            Logger?.LogDebug($"{nameof(TradeWebSocketClientAdapter)}.{nameof(Unsubscribe)}: Begin streaming...  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                            Controller.Begin();
                        }
                    }
                    catch (OperationCanceledException) { /* ignored */ }
                    catch (Exception e)
                    {
                        Logger?.LogError(e, $"{nameof(TradeWebSocketClientAdapter)}.{nameof(Unsubscribe)}: Failed.  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                        OnError?.Invoke(e);
                    }
                });
            }
        }

        #endregion Public Methods
    }
}
