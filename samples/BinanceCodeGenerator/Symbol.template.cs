// ReSharper disable InconsistentNaming
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Binance.Account.Orders;
using Binance.Api;

namespace Binance
{
    /// <summary>
    /// Defined symbols (for convenience/reference only).
    /// </summary>
    /// <remarks></remarks>
    public sealed class Symbol : IComparable<Symbol>, IEquatable<Symbol>
    {
        #region Public Constants

        /// <summary>
        /// When the symbols (currency pairs) were last updated.
        /// </summary>
        // <<insert timestamp>>

        // <<insert symbols>>

        // Redirect (BCH) Bitcoin Cash (BCC = BitConnect)
        public static readonly Symbol BCH_USDT;
        public static readonly Symbol BCH_BNB;
        public static readonly Symbol BCH_BTC;
        public static readonly Symbol BCH_ETH;

        #endregion Public Constants

        #region Implicit Operators

        public static bool operator ==(Symbol x, Symbol y) => Equals(x, y);

        public static bool operator !=(Symbol x, Symbol y) => !(x == y);

        public static implicit operator string(Symbol symbol) => symbol?.ToString();

        public static implicit operator Symbol(string s)
        {
            if (s == null) return null;
            var _s = s.FormatSymbol();
            lock (_sync)
            {
                return Cache.ContainsKey(_s) ? Cache[_s] : null;
            }
        }

        #endregion Implicit Operators

        #region Public Properties

        /// <summary>
        /// Symbol cache.
        /// </summary>
        public static IDictionary<string, Symbol> Cache { get; }

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
        /// Get base asset range (min/max quantity and step size).
        /// </summary>
        public InclusiveRange Quantity { get; }

        /// <summary>
        /// Get the quote asset range (min/max price and tick size).
        /// </summary>
        public InclusiveRange Price { get; }

        /// <summary>
        /// Get the minimum notional value (price * quantity).
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

        static Symbol()
        {
            try
            {
                // Redirect (BCH) Bitcoin Cash (BCC = BitConnect)
                BCH_USDT = BCC_USDT;
                BCH_BNB = BCC_BNB;
                BCH_BTC = BCC_BTC;
                BCH_ETH = BCC_ETH;

                Cache = new Dictionary<string, Symbol>
                {
                    // <<insert symbol definitions>>

                    // Redirect (BCH) Bitcoin Cash (BCC = BitConnect)
                    { "BCHUSDT", BCC_USDT },
                    { "BCHBNB", BCC_BNB },
                    { "BCHBTC", BCC_BTC },
                    { "BCHETH", BCC_ETH }
                };
            }
            catch (Exception e)
            {
                Console.Error?.WriteLine($"{nameof(Binance)}.{nameof(Symbol)}(): \"{e.Message}\"");
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="status">The symbol status.</param>
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
        /// Verify that symbol is valid. If fails, but known to be valid,
        /// call UpdateCacheAsync() to get the latest symbols.
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public static bool IsValid(string symbol)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                return false;

            symbol = symbol.FormatSymbol();

            lock (_sync)
            {
                return Cache.ContainsKey(symbol)
                    && Cache[symbol].ToString() == symbol;
            }
        }

        /// <summary>
        /// Update the symbol cache and asset cache.
        /// </summary>
        /// <param name="api"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task UpdateCacheAsync(IBinanceApi api, CancellationToken token = default)
        {
            var symbols = await api.GetSymbolsAsync(token)
                .ConfigureAwait(false);

            UpdateCache(symbols);
        }

        /// <summary>
        /// Update the symbol cache and asset cache.
        /// </summary>
        /// <param name="symbols">The symbols.</param>
        /// <returns></returns>
        public static void UpdateCache(IEnumerable<Symbol> symbols)
        {
            Throw.IfNull(symbols, nameof(symbols));

            // ReSharper disable once PossibleMultipleEnumeration
            if (!symbols.Any())
                throw new ArgumentException("Enumerable must not be empty.", nameof(symbols));

            lock (_sync)
            {
                // Remove any old symbols (preserves redirections).
                // ReSharper disable once PossibleMultipleEnumeration
                foreach (var symbol in Cache.Values.ToArray())
                {
                    if (!symbols.Contains(symbol))
                    {
                        Cache.Remove(symbol);
                    }
                }

                // Update existing and add any new symbols.
                // ReSharper disable once PossibleMultipleEnumeration
                foreach (var symbol in symbols)
                {
                    Cache[symbol] = symbol;
                }
            }

            // ReSharper disable once PossibleMultipleEnumeration
            Asset.UpdateCache(symbols);
        }

        public override string ToString()
        {
            return _symbol;
        }

        public override bool Equals(object obj)
        {
            if (obj == null)
                return false;

            if (obj is Symbol symbol)
                return Equals(symbol);

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
            return other == null ? 1 : string.Compare(_symbol, other._symbol, StringComparison.Ordinal);
        }

        #endregion IComparable<Symbol>

        #region IEquatable<Symbol>

        public bool Equals(Symbol other)
        {
            return CompareTo(other) == 0;
        }

        #endregion IEquatable<Symbol>
    }
}
