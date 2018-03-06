using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace Binance.WebSocket
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
            : this(new AggregateTradeWebSocketClient(),
                   new CandlestickWebSocketClient(),
                   new DepthWebSocketClient(),
                   new SymbolStatisticsWebSocketClient(),
                   new TradeWebSocketClient())
        { }

        /// <summary>
        /// The DI constructor.
        /// </summary>
        /// <param name="aggregateTradeWebSocketClient"></param>
        /// <param name="candlestickWebSocketClient"></param>
        /// <param name="depthWebSocketClient"></param>
        /// <param name="symbolStatisticsWebSocketClient"></param>
        /// <param name="tradeWebSocketClient"></param>
        /// <param name="logger"></param>
        public BinanceWebSocketClientManager(
            IAggregateTradeWebSocketClient aggregateTradeWebSocketClient,
            ICandlestickWebSocketClient candlestickWebSocketClient,
            IDepthWebSocketClient depthWebSocketClient,
            ISymbolStatisticsWebSocketClient symbolStatisticsWebSocketClient,
            ITradeWebSocketClient tradeWebSocketClient,
            ILogger<BinanceWebSocketClientManager> logger = null)
            : base(aggregateTradeWebSocketClient,
                   candlestickWebSocketClient,
                   depthWebSocketClient,
                   symbolStatisticsWebSocketClient,
                   tradeWebSocketClient,
                   logger)
        { }

        #endregion Constructors
    }
}
