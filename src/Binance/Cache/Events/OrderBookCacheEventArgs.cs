// ReSharper disable once CheckNamespace
namespace Binance.Cache
{
    /// <summary>
    /// Depth of market updated event.
    /// </summary>
    public class OrderBookCacheEventArgs : CacheEventArgs
    {
        #region Public Properties

        /// <summary>
        /// The depth of market snapshot (order book).
        /// </summary>
        public OrderBook OrderBook { get; }

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="orderBook"></param>
        public OrderBookCacheEventArgs(OrderBook orderBook)
        {
            Throw.IfNull(orderBook, nameof(orderBook));

            OrderBook = orderBook;
        }

        #endregion Constructors
    }
}
