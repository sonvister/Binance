using Binance.Manager;
using Microsoft.Extensions.Logging;

namespace Binance.WebSocket.Manager
{
    /// <summary>
    /// A <see cref="IBinanceJsonClientManager"/> web socket implementation.
    /// </summary>
    public class BinanceWebSocketClientManager : BinanceJsonClientManager<IWebSocketStream>, IBinanceWebSocketClientManager
    {
        #region Constructors

        /// <summary>
        /// The default constructor.
        /// </summary>
        public BinanceWebSocketClientManager()
            : this(new AggregateTradeWebSocketClientManager(),
                   new CandlestickWebSocketClientManager(),
                   new DepthWebSocketClientManager(),
                   new SymbolStatisticsWebSocketClientManager(),
                   new TradeWebSocketClientManager())
        { }

        /// <summary>
        /// The DI constructor.
        /// </summary>
        /// <param name="aggregateTradeWebSocketClientManager"></param>
        /// <param name="tradeWebSocketClientManager"></param>
        /// <param name="logger"></param>
        public BinanceWebSocketClientManager(
            IAggregateTradeWebSocketClientManager aggregateTradeWebSocketClientManager,
            ICandlestickWebSocketClientManager candlestickWebSocketClientManager,
            IDepthWebSocketClientManager depthWebSocketClientManager,
            ISymbolStatisticsWebSocketClientManager symbolStatisticsWebSocketClientManager,
            ITradeWebSocketClientManager tradeWebSocketClientManager,
            ILogger<BinanceWebSocketClientManager> logger = null)
            : base(aggregateTradeWebSocketClientManager,
                   candlestickWebSocketClientManager,
                   depthWebSocketClientManager,
                   symbolStatisticsWebSocketClientManager,
                   tradeWebSocketClientManager,
                   logger)
        { }

        #endregion Constructors
    }
}
