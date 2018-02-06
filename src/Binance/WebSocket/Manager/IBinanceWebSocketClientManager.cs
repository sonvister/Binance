using System;
using Binance.Utility;

namespace Binance.WebSocket.Manager
{
    public interface IBinanceWebSocketClientManager : IDisposable
    {
        /// <summary>
        /// The error event.
        /// </summary>
        event EventHandler<BinanceWebSocketClientManagerErrorEventArgs> Error;

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
