// ReSharper disable InconsistentNaming
using System;
using System.Collections.Generic;
using System.Linq;

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
        /// Get the base asset symbol.
        /// </summary>
        public Asset BaseAsset { get; }

        /// <summary>
        /// Get the quote asset symbol.
        /// </summary>
        public Asset QuoteAsset { get; }

        /// <summary>
        /// Get the minimum order quantity.
        /// </summary>
        public decimal BaseMinQuantity { get; }

        /// <summary>
        /// Get the maximum order quantity.
        /// </summary>
        public decimal BaseMaxQuantity { get; }

        /// <summary>
        /// Get the minimum order price as well as the price increment.
        /// NOTE: The order price must be a multiple of this increment.
        /// </summary>
        public decimal QuoteIncrement { get; }

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
        /// <param name="baseMaxQty">The base asset maximum quantity.</param>
        /// <param name="baseMinQty">The base asset minimum quantity.</param>
        /// <param name="quoteIncrement">The quote asset increment price.</param>
        public Symbol(Asset baseAsset, Asset quoteAsset, decimal baseMinQty, decimal baseMaxQty, decimal quoteIncrement)
        {
            Throw.IfNull(baseAsset, nameof(baseAsset));
            Throw.IfNull(quoteAsset, nameof(quoteAsset));

            if (baseMinQty <= 0)
                throw new ArgumentException($"{nameof(Symbol)} quantity must be greater than 0.", nameof(baseMinQty));
            if (baseMaxQty <= 0)
                throw new ArgumentException($"{nameof(Symbol)} quantity must be greater than 0.", nameof(baseMaxQty));
            if (quoteIncrement <= 0)
                throw new ArgumentException($"{nameof(Symbol)} increment must be greater than 0.", nameof(quoteIncrement));

            BaseAsset = baseAsset;
            QuoteAsset = quoteAsset;
            BaseMinQuantity = baseMinQty;
            BaseMaxQuantity = baseMaxQty;
            QuoteIncrement = quoteIncrement;

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

            if (symbols.Any())
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
