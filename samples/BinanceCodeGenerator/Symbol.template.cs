// ReSharper disable InconsistentNaming
using System;
using System.Collections.Generic;
using System.Linq;
using Binance.Account.Orders;

namespace Binance
{
    /// <summary>
    /// Defined symbols (for convienience/reference only).
    /// </summary>
    public sealed class Symbol : IComparable<Symbol>, IEquatable<Symbol>
    {
        #region Public Constants

        /// <summary>
        /// When the symbols (currency pairs) were last updated.
        /// </summary>
        // <<insert timestamp>>

        // Redirect (BCH) Bitcoin Cash (BCC = BitConnect)
        public static readonly Symbol BCH_USDT = BCC_USDT;
        public static readonly Symbol BCH_BNB = BCC_BNB;
        public static readonly Symbol BCH_BTC = BCC_BTC;
        public static readonly Symbol BCH_ETH = BCC_ETH;

        // <<insert symbols>>

        #endregion Public Constants

        #region Implicit Operators

        public static bool operator ==(Symbol x, Symbol y) => Equals(x, y);

        public static bool operator !=(Symbol x, Symbol y) => !(x == y);

        public static implicit operator string(Symbol symbol) => symbol.ToString();

        public static implicit operator Symbol(string s)
        {
            var _s = s.FormatSymbol();
            return Cache.ContainsKey(_s) ? Cache[_s] : null;
        }

        #endregion Implicit Operators

        #region Public Properties

        /// <summary>
        /// Symbol cache.
        /// </summary>
        public static readonly IDictionary<string, Symbol> Cache = new Dictionary<string, Symbol>()
        {
            // Redirect (BCH) Bitcoin Cash (BCC = BitConnect)
            { "BCHUSDT", BCC_USDT },
            { "BCHBNB", BCC_BNB },
            { "BCHBTC", BCC_BTC },
            { "BCHETH", BCC_ETH },

            // <<insert symbol definitions>>
        };

        /// <summary>
        /// Get the symbol status.
        /// </summary>
        public SymbolStatus Status { get; }

        /// <summary>
        /// Get the base asset symbol.
        /// </summary>
        public Asset BaseAsset { get; }

        /// <summary>
        /// Get the quote asset symbol.
        /// </summary>
        public Asset QuoteAsset { get; }

        /// <summary>
        /// Get base asset range.
        /// </summary>
        public InclusiveRange Quantity { get; }

        /// <summary>
        /// Get the quote asset range.
        /// </summary>
        public InclusiveRange Price { get; }

        /// <summary>
        /// Get the minimum notional value.
        /// </summary>
        public decimal NotionalMinimumValue { get; }

        /// <summary>
        /// Get the allowed order types.
        /// </summary>
        public IEnumerable<OrderType> OrderTypes { get; }

        /// <summary>
        /// Get the flag indicating if iceberg orders are allowed.
        /// </summary>
        public bool IsIcebergAllowed { get; }

        #endregion Public Properties

        #region Private Fields

        private readonly string _symbol;

        private static readonly object _sync = new object();

        #endregion Private Fields

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="baseAsset">The symbol base asset.</param>
        /// <param name="quoteAsset">The symbol quote asset.</param>
        /// <param name="quantity">The minimum, maximum, and incremental quantity values.</param>
        /// <param name="price">The minimum, maximum, and incremental price values.</param>
        /// <param name="notionalMinimumValue">The minimum notional value.</param>
        /// <param name="isIcebergAllowed">The flag indicating if iceberg orders are allowed.</param>
        /// <param name="orderTypes">The list of allowed order types.</param>
        public Symbol(SymbolStatus status, Asset baseAsset, Asset quoteAsset, InclusiveRange quantity, InclusiveRange price, decimal notionalMinimumValue, bool isIcebergAllowed, IEnumerable<OrderType> orderTypes)
        {
            Throw.IfNull(baseAsset, nameof(baseAsset));
            Throw.IfNull(quoteAsset, nameof(quoteAsset));
            Throw.IfNull(quantity, nameof(quantity));
            Throw.IfNull(price, nameof(price));
            Throw.IfNull(orderTypes, nameof(orderTypes));

            Status = status;

            BaseAsset = baseAsset;
            QuoteAsset = quoteAsset;

            Quantity = quantity;
            Price = price;

            NotionalMinimumValue = notionalMinimumValue;
            IsIcebergAllowed = isIcebergAllowed;
            OrderTypes = orderTypes;

            _symbol = $"{baseAsset}{quoteAsset}";
        }

        #endregion Constructors

        #region Public Methods

        /// <summary>
        /// Update the symbol cache.
        /// </summary>
        /// <param name="symbols">The symbols.</param>
        /// <returns></returns>
        public static void UpdateCache(IEnumerable<Symbol> symbols)
        {
            Throw.IfNull(symbols, nameof(symbols));

            if (!symbols.Any())
                throw new ArgumentException("Enumerable must not be empty.", nameof(symbols));

            lock (_sync)
            {
                // Remove any old symbols (preserves redirections).
                foreach (var symbol in Cache.Values.ToArray())
                {
                    if (!symbols.Contains(symbol))
                    {
                        Cache.Remove(symbol);
                    }
                }

                // Update existing and add any new symbols.
                foreach (var symbol in symbols)
                {
                    Cache[symbol] = symbol;
                }
            }
        }

        public override string ToString()
        {
            return _symbol;
        }

        public override bool Equals(object obj)
        {
            return _symbol.Equals(obj);
        }

        public override int GetHashCode()
        {
            return _symbol.GetHashCode();
        }

        #endregion Public Methods

        #region IComparable<Symbol>

        public int CompareTo(Symbol other)
        {
            if (other == null)
                return 1;

            return _symbol.CompareTo(other._symbol);
        }

        #endregion IComparable<Symbol>

        #region IEquatable<Symbol>

        public bool Equals(Symbol other)
        {
            return CompareTo(other) == 0;
        }

        #endregion IEquatable<Symbol>

        // File generated by BinanceCodeGenerator tool.
    }
}
