using System;
using System.Threading;
using System.Threading.Tasks;
using Binance.WebSocket.Events;
using Microsoft.Extensions.Logging;

namespace Binance.WebSocket.Manager
{
    internal sealed class AggregateTradeWebSocketClientAdapter : BinanceWebSocketClientAdapter<IAggregateTradeWebSocketClient>, IAggregateTradeWebSocketClient, IBinanceWebSocketClientAdapter
    {
        #region Public Events

        public event EventHandler<AggregateTradeEventArgs> AggregateTrade
        {
            add { Client.AggregateTrade += value; }
            remove { Client.AggregateTrade -= value; }
        }

        #endregion Public Events

        #region Constructors

        public AggregateTradeWebSocketClientAdapter(IBinanceWebSocketManager manager, IAggregateTradeWebSocketClient client, ILogger<IBinanceWebSocketManager> logger = null, Action<Exception> onError = null)
            : base(manager, client, logger, onError)
        { }

        #endregion Constructors

        #region Public Methods

        public void Subscribe(string symbol, Action<AggregateTradeEventArgs> callback)
        {
            lock (Sync)
            {
                Task = Task.ContinueWith(async _ =>
                {
                    try
                    {
                        Logger?.LogDebug($"{nameof(AggregateTradeWebSocketClientAdapter)}.{nameof(Subscribe)}: Cancel streaming...  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                        await Controller.CancelAsync()
                            .ConfigureAwait(false);

                        Client.Subscribe(symbol, callback);

                        if (!Manager.IsAutoStreamingDisabled && !Controller.IsActive)
                        {
                            Logger?.LogDebug($"{nameof(AggregateTradeWebSocketClientAdapter)}.{nameof(Subscribe)}: Begin streaming...  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                            Controller.Begin();
                        }
                    }
                    catch (OperationCanceledException) { /* ignored */ }
                    catch (Exception e)
                    {
                        Logger?.LogError(e, $"{nameof(AggregateTradeWebSocketClientAdapter)}.{nameof(Subscribe)}: Failed.  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                        OnError?.Invoke(e);
                    }
                });
            }
        }

        public void Unsubscribe(string symbol, Action<AggregateTradeEventArgs> callback)
        {
            lock (Sync)
            {
                Task = Task.ContinueWith(async _ =>
                {
                    try
                    {
                        Logger?.LogDebug($"{nameof(AggregateTradeWebSocketClientAdapter)}.{nameof(Unsubscribe)}: Cancel streaming...  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                        await Controller.CancelAsync()
                            .ConfigureAwait(false);

                        Client.Unsubscribe(symbol, callback);

                        if (!Manager.IsAutoStreamingDisabled && !Controller.IsActive)
                        {
                            Logger?.LogDebug($"{nameof(AggregateTradeWebSocketClientAdapter)}.{nameof(Unsubscribe)}: Begin streaming...  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                            Controller.Begin();
                        }
                    }
                    catch (OperationCanceledException) { /* ignored */ }
                    catch (Exception e)
                    {
                        Logger?.LogError(e, $"{nameof(AggregateTradeWebSocketClientAdapter)}.{nameof(Unsubscribe)}: Failed.  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                        OnError?.Invoke(e);
                    }
                });
            }
        }

        #endregion Public Methods
    }
}
