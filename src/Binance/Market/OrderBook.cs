using System;
using System.Collections.Generic;
using System.Linq;

namespace Binance.Market
{
    /// <summary>
    /// An snapshot of the depth of market (DOM) for a specific symbol with
    /// price levels and aggregate quantities.
    /// </summary>
    public sealed class OrderBook : ICloneable
    {
        #region Public Properties

        /// <summary>
        /// Get the symbol.
        /// </summary>
        public string Symbol { get; }

        /// <summary>
        /// Get the last update ID.
        /// </summary>
        public long LastUpdateId { get; private set; }

        /// <summary>
        /// Get the order book top (best ask and bid) or null
        /// if either the bid or ask is not available.
        /// </summary>
        public OrderBookTop Top { get; private set; }

        /// <summary>
        /// Get the buyer bids in order of decreasing price.
        /// </summary>
        public IEnumerable<OrderBookPriceLevel> Bids { get; private set; }

        /// <summary>
        /// Get the seller asks in order of increasing price.
        /// </summary>
        public IEnumerable<OrderBookPriceLevel> Asks { get; private set; }

        #endregion Public Properties

        #region Private Fields

        private readonly SortedDictionary<decimal, OrderBookPriceLevel> _bids;
        private readonly SortedDictionary<decimal, OrderBookPriceLevel> _asks;

        #endregion Private Fields

        #region Constructors

        /// <summary>
        /// Construct an order book.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <param name="lastUpdateId">The last updated ID.</param>
        /// <param name="bids">The bids (price and aggregate quantity) in any sequence.</param>
        /// <param name="asks">The asks (price and aggregate quantity) in any sequence.</param>
        public OrderBook(string symbol, long lastUpdateId, IEnumerable<(decimal, decimal)> bids, IEnumerable<(decimal, decimal)> asks)
        {
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));
            Throw.IfNull(bids, nameof(bids));
            Throw.IfNull(asks, nameof(asks));

            if (lastUpdateId <= 0)
                throw new ArgumentException($"{nameof(OrderBook)} last update ID must be greater than 0.", nameof(lastUpdateId));

            Symbol = symbol.FormatSymbol();
            LastUpdateId = lastUpdateId;

            _bids = new SortedDictionary<decimal, OrderBookPriceLevel>(new ReverseComparer<decimal>());
            _asks = new SortedDictionary<decimal, OrderBookPriceLevel>();

            foreach (var bid in bids)
            {
                _bids.Add(bid.Item1, new OrderBookPriceLevel(bid.Item1, bid.Item2));
            }

            foreach (var ask in asks)
            {
                _asks.Add(ask.Item1, new OrderBookPriceLevel(ask.Item1, ask.Item2));
            }

            Bids = _bids.Values.ToArray();
            Asks = _asks.Values.ToArray();

            Top = Bids.Any() && Asks.Any() ? new OrderBookTop(this) : null;
        }

        #endregion Constructors

        #region Public Methods

        /// <summary>
        /// Get the aggregate quantity at a price level.
        /// </summary>
        /// <param name="price">The price level.</param>
        /// <returns>The quantity at price (0 if no entry at price).</returns>
        public decimal Quantity(decimal price)
        {
            return _bids.ContainsKey(price) ? _bids[price].Quantity
                : _asks.ContainsKey(price) ? _asks[price].Quantity : 0;
        }

        /// <summary>
        /// Get the sum quantity of bids at and above the price or
        /// the sum quantity of asks at and below the price.
        /// </summary>
        /// <param name="price">The price level (inclusive).</param>
        /// <returns>The order book depth up to price.</returns>
        public decimal Depth(decimal price)
        {
            return _bids.TakeWhile(_ => _.Key >= price).Sum(_ => _.Value.Quantity)
                + _asks.TakeWhile(_ => _.Key <= price).Sum(_ => _.Value.Quantity);
        }

        /// <summary>
        /// Get the sum volume (price * quantity) of bids at and above the
        /// price or the sum volume of asks at and below the price.
        /// </summary>
        /// <param name="price">The price level (inclusive).</param>
        /// <returns>The order book volume up to price.</returns>
        public decimal Volume(decimal price)
        {
            return _bids.TakeWhile(_ => _.Key >= price).Sum(_ => _.Value.Price * _.Value.Quantity)
                + _asks.TakeWhile(_ => _.Key <= price).Sum(_ => _.Value.Price * _.Value.Quantity);
        }

        #endregion Public Methods

        #region Internal Methods

        /// <summary>
        /// Modify the order book.
        /// </summary>
        /// <param name="lastUpdateId"></param>
        /// <param name="bids"></param>
        /// <param name="asks"></param>
        internal void Modify(long lastUpdateId, IEnumerable<(decimal, decimal)> bids, IEnumerable<(decimal, decimal)> asks)
        {
            // Update order book bids.
            foreach (var bid in bids)
            {
                // If quantity is > 0, then set the quantity.
                if (bid.Item2 > 0)
                {
                    if (_bids.ContainsKey(bid.Item1))
                        _bids[bid.Item1].Quantity = bid.Item2;
                    else
                        _bids[bid.Item1] = new OrderBookPriceLevel(bid.Item1, bid.Item2);
                }
                else // otherwise, remove the price level.
                    _bids.Remove(bid.Item1);
            }

            // Update order book asks.
            foreach (var ask in asks)
            {
                // If quantity is > 0, then set the quantity.
                if (ask.Item2 > 0)
                {
                    if (_asks.ContainsKey(ask.Item1))
                        _asks[ask.Item1].Quantity = ask.Item2;
                    else
                        _asks[ask.Item1] = new OrderBookPriceLevel(ask.Item1, ask.Item2);
                }
                else // otherwise, remove the price level.
                    _asks.Remove(ask.Item1);
            }

            Bids = _bids.Values.ToArray();
            Asks = _asks.Values.ToArray();

            Top = Bids.Any() && Asks.Any() ? new OrderBookTop(this) : null;

            // Set the order book last update ID.
            LastUpdateId = lastUpdateId;
        }

        #endregion Internal Methods

        #region ICloneable

        /// <summary>
        /// Get a duplicate order book (deep copy).
        /// </summary>
        /// <returns></returns>
        public OrderBook Clone()
        {
            return new OrderBook(Symbol, LastUpdateId, _bids.Select(_ => (_.Key, _.Value.Quantity)), _asks.Select(_ => (_.Key, _.Value.Quantity)));
        }

        /// <summary>
        /// Get a duplicate order book (deep copy).
        /// </summary>
        /// <returns></returns>
        public OrderBook Clone(int limit)
        {
            if (limit <= 0) throw new ArgumentOutOfRangeException(nameof(limit));

            return new OrderBook(Symbol, LastUpdateId, _bids.Take(limit).Select(_ => (_.Key, _.Value.Quantity)), _asks.Take(limit).Select(_ => (_.Key, _.Value.Quantity)));
        }

        /// <summary>
        /// Get a duplicate order book (deep copy)
        /// </summary>
        /// <returns></returns>
        object ICloneable.Clone() { return Clone(); }

        #endregion ICloneable

        #region Private Classes

        /// <summary>
        /// Comarer used for ordering bids in descending price order.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        private class ReverseComparer<T> : IComparer<T> where T : IComparable<T>
        {
            public int Compare(T x, T y)
            {
                return y.CompareTo(x);
            }
        }

        #endregion Private Classes
    }
}
