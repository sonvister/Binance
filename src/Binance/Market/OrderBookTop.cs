using System;

namespace Binance.Market
{
    /// <summary>
    /// Best order book bid and ask price and quantity.
    /// </summary>
    public sealed class OrderBookTop : ICloneable
    {
        #region Public Properties

        /// <summary>
        /// The symbol.
        /// </summary>
        public string Symbol { get; private set; }

        /// <summary>
        /// Best bid price and quantity.
        /// </summary>
        public OrderBookPriceLevel Bid { get; private set; }

        /// <summary>
        /// Best ask price and quantity.
        /// </summary>
        public OrderBookPriceLevel Ask { get; private set; }

        #endregion Public Properties

        #region Constructors

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

        #region ICloneable

        /// <summary>
        /// Get a duplicate order book top (deep copy).
        /// </summary>
        /// <returns><see cref="OrderBookTop"/></returns>
        public OrderBookTop Clone()
        {
            return new OrderBookTop(Symbol, Bid.Price, Bid.Quantity, Ask.Price, Ask.Quantity);
        }

        /// <summary>
        /// Get a duplicate order book top (deep copy).
        /// </summary>
        /// <returns><see cref="object"/></returns>
        object ICloneable.Clone() { return Clone(); }

        #endregion ICloneable
    }
}
