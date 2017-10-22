using Binance.Api.Json;
using Binance.Orders.Book;
using System;
using System.Collections.Generic;

namespace Binance
{
    /// <summary>
    /// Depth of Market (DOM) for a symbol.
    /// </summary>
    public interface IOrderBook : ICloneable
    {
        #region Public Properties

        /// <summary>
        /// The symbol.
        /// </summary>
        string Symbol { get; }

        /// <summary>
        /// The last update ID used to synchronize with websocket client.
        /// </summary>
        long LastUpdateId { get; }

        /// <summary>
        /// The best ask and bid prices and quantities.
        /// </summary>
        OrderBookTop Top { get; }

        /// <summary>
        /// The order book bids (prices and quantities).
        /// </summary>
        IEnumerable<OrderBookPriceLevel> Bids { get; }

        /// <summary>
        /// The order book asks (prices and quantities).
        /// </summary>
        IEnumerable<OrderBookPriceLevel> Asks { get; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Get the aggregate quantity at a price level.
        /// </summary>
        /// <param name="price">The price level.</param>
        /// <returns>The quantity at price (0 if no entry at price).</returns>
        decimal Quantity(decimal price);

        /// <summary>
        /// Get the sum quantity of bids at and above the price or
        /// sum quantity of asks at and below the price.
        /// </summary>
        /// <param name="price"></param>
        /// <returns>The order book depth up to price.</returns>
        decimal Depth(decimal price);

        /// <summary>
        /// Get the sum volume (price * quantity) of bids at and above the
        /// price or sum volume of asks at and below the price.
        /// </summary>
        /// <param name="price"></param>
        /// <returns>The order book volume up to price.</returns>
        decimal Volume(decimal price);

        /// <summary>
        /// Get a duplicate order book (deep copy) with depth limit.
        /// </summary>
        /// <param name="limit"></param>
        /// <returns></returns>
        IOrderBook Clone(int limit = BinanceJsonApi.OrderBookLimitDefault);

        #endregion Public Methods
    }
}
