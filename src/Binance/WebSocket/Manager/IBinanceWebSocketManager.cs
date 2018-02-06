using System;
using Binance.Utility;

namespace Binance.WebSocket.Manager
{
    /// <summary>
    /// A facade for managing multiple <see cref="IBinanceWebSocketClient"/>
    /// implementations. Clients are controlled via adapter implementations
    /// that automatically cancel streaming before subscribe/unsubscribe
    /// and automaically re-enable streaming afterwards (if not disabled).
    /// The familiar client interfaces presented imply synchronous operation,
    /// but the subscribe/unsubscribe operations are done asynchronously.
    /// </summary>
    public interface IBinanceWebSocketManager : IDisposable
    {
        /// <summary>
        /// The error event. Raised when exceptions occur from client task
        /// controller actions or from client adapter subscribe/unsubscribe
        /// (async) operations.
        /// </summary>
        event EventHandler<WebSocketManagerErrorEventArgs> Error;

        /// <summary>
        /// Get the <see cref="IAggregateTradeWebSocketClient"/> interface.
        /// </summary>
        IAggregateTradeWebSocketClient AggregateTradeClient { get; }

        /// <summary>
        /// Get the <see cref="ICandlestickWebSocketClient"/> interface.
        /// </summary>
        ICandlestickWebSocketClient CandlestickClient { get; }

        /// <summary>
        /// Get the <see cref="IDepthWebSocketClient"/> interface.
        /// </summary>
        IDepthWebSocketClient DepthClient { get; }

        /// <summary>
        /// Get the <see cref="ISymbolStatisticsWebSocketClient"/> interface.
        /// </summary>
        ISymbolStatisticsWebSocketClient StatisticsClient { get; }

        /// <summary>
        /// Get the <see cref="ITradeWebSocketClient"/> interface.
        /// </summary>
        ITradeWebSocketClient TradeClient { get; }

        /// <summary>
        /// Get or set the flag controlling whether auto-streaming is enabled
        /// during subscribe operations (auto-streaming is enabled by default).
        /// Disable (set to true) when subscribing to multiple streams at once
        /// then set to false to begin auto-streaming with next subscribe call.
        /// </summary>
        bool IsAutoStreamingDisabled { get; set; }

        /// <summary>
        /// Get the <see cref="IRetryTaskController"/> assigned to the stream.
        /// </summary>
        /// <param name="webSocket">The <see cref="IWebSocketStream"/>.</param>
        /// <returns></returns>
        IRetryTaskController GetController(IWebSocketStream webSocket);
    }
}
