using System;
using Binance.Client;

namespace Binance.WebSocket
{
    /// <summary>
    /// A facade for automatic control of multiple JSON clients and streams.
    /// </summary>
    public interface IBinanceJsonClientManager : IDisposable
    {
        /// <summary>
        /// The error event.
        /// </summary>
        event EventHandler<ErrorEventArgs> Error;

        /// <summary>
        /// Get the aggregate trade client manager.
        /// </summary>
        IAggregateTradeClient AggregateTradeClient { get; }

        /// <summary>
        /// Get the candlestick client manager.
        /// </summary>
        ICandlestickClient CandlestickClient { get; }

        /// <summary>
        /// Get the depth client manager.
        /// </summary>
        IDepthClient DepthClient { get; }

        /// <summary>
        /// Get the symbol statistics client manager.
        /// </summary>
        ISymbolStatisticsClient StatisticsClient { get; }

        /// <summary>
        /// Get the trade client manager.
        /// </summary>
        ITradeClient TradeClient { get; }
    }
}
