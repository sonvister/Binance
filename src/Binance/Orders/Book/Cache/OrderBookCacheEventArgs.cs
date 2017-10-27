using System;

namespace Binance.Orders.Book.Cache
{
    /// <summary>
    /// Depth of market updated event.
    /// </summary>
    public class OrderBookCacheEventArgs : EventArgs
    {
        #region Public Properties

        /// <summary>
        /// The depth of market snapshot (order book).
        /// </summary>
        public OrderBook OrderBook { get; private set; }

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="orderBook"></param>
        public OrderBookCacheEventArgs(OrderBook orderBook)
        {
            Throw.IfNull(orderBook);

            OrderBook = orderBook;
        }

        #endregion Constructors
    }
}
