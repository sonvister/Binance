using System;
using System.Linq;

namespace Binance.Market
{
    /// <summary>
    /// Best order book bid and ask price and quantity.
    /// </summary>
    public sealed class OrderBookTop
    {
        #region Public Properties

        /// <summary>
        /// The symbol.
        /// </summary>
        public string Symbol { get; }

        /// <summary>
        /// Best bid price and quantity.
        /// </summary>
        public OrderBookPriceLevel Bid { get; }

        /// <summary>
        /// Best ask price and quantity.
        /// </summary>
        public OrderBookPriceLevel Ask { get; }

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Construct order book top.
        /// </summary>
        /// <param name="orderBook">The order book.</param>
        public OrderBookTop(OrderBook orderBook)
        {
            Throw.IfNull(orderBook, nameof(orderBook));

            Symbol = orderBook.Symbol;

            Bid = orderBook.Bids.First();
            Ask = orderBook.Asks.First();
        }

        /// <summary>
        /// Construct order book top.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <param name="bidPrice">The best bid price.</param>
        /// <param name="bidQuantity">The best bid quantity.</param>
        /// <param name="askPrice">The best ask price.</param>
        /// <param name="askQuantity">The best ask quantity.</param>
        public OrderBookTop(string symbol, decimal bidPrice, decimal bidQuantity, decimal askPrice, decimal askQuantity)
        {
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            if (bidPrice > askPrice)
                throw new ArgumentException($"{nameof(OrderBookTop)} ask price must be greater than the bid price.", nameof(askPrice));

            Symbol = symbol;

            Bid = new OrderBookPriceLevel(bidPrice, bidQuantity);
            Ask = new OrderBookPriceLevel(askPrice, askQuantity);
        }

        #endregion Constructors
    }
}
