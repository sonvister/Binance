using System;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace Binance
{
    /// <summary>
    /// Best order book bid and ask price and quantity.
    /// </summary>
    public sealed class OrderBookTop : IEquatable<OrderBookTop>
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
        /// <param name="symbol"></param>
        /// <param name="bid">The best bid price and quantity.</param>
        /// <param name="ask">The best ask price and quantity.</param>
        /// <returns></returns>
        public static OrderBookTop Create(string symbol, (decimal, decimal) bid, (decimal, decimal) ask)
            => Create(symbol, bid.Item1, bid.Item2, ask.Item1, ask.Item2);

        /// <summary>
        /// Construct order book top.
        /// </summary>
        /// <param name="symbol">The symbol.</param> 
        /// <param name="bidPrice">The best bid price.</param> 
        /// <param name="bidQuantity">The best bid quantity.</param> 
        /// <param name="askPrice">The best ask price.</param> 
        /// <param name="askQuantity">The best ask quantity.</param> 
        /// <returns></returns>
        public static OrderBookTop Create(string symbol, decimal bidPrice, decimal bidQuantity, decimal askPrice, decimal askQuantity)
        {
            return new OrderBookTop(symbol, new OrderBookPriceLevel(bidPrice, bidQuantity), new OrderBookPriceLevel(askPrice, askQuantity));
        }

        /// <summary>
        /// Construct order book top.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <param name="bid">The bid price and quantity.</param>
        /// <param name="ask">The ask price and quantity.</param>
        public OrderBookTop(string symbol, OrderBookPriceLevel bid, OrderBookPriceLevel ask)
        {
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));
            Throw.IfNull(bid, nameof(bid));
            Throw.IfNull(ask, nameof(ask));

            if (bid.Price > ask.Price)
                throw new ArgumentException($"{nameof(OrderBookTop)} ask price must be greater than the bid price.", nameof(ask.Price));

            Symbol = symbol.FormatSymbol();

            Bid = bid;
            Ask = ask;
        }

        /// <summary>
        /// Construct order book top.
        /// </summary>
        /// <param name="orderBook">The order book.</param>
        internal OrderBookTop(OrderBook orderBook)
        {
            Throw.IfNull(orderBook, nameof(orderBook));

            Symbol = orderBook.Symbol;

            Bid = orderBook.Bids.First();
            Ask = orderBook.Asks.First();
        }

        #endregion Constructors

        #region IEquatable

        public bool Equals(OrderBookTop other)
        {
            if (other == null)
                return false;

            return other.Symbol == Symbol
                && other.Bid.Price == Bid.Price
                && other.Bid.Quantity == Bid.Quantity
                && other.Ask.Price == Ask.Price
                && other.Ask.Quantity == Ask.Quantity;
        }

        #endregion IEquatable
    }
}
