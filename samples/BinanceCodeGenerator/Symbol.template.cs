// ReSharper disable InconsistentNaming
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Binance.Cache;

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

        #endregion Public Constants

        #region Implicit Operators

        public static bool operator ==(Symbol x, Symbol y) => Equals(x, y);

        public static bool operator !=(Symbol x, Symbol y) => !(x == y);

        public static implicit operator string(Symbol symbol) => symbol?.ToString();

        #endregion Implicit Operators

        #region Public Properties

        /// <summary>
        /// Symbol cache.
        /// </summary>
        public static IObjectCache<Symbol> Cache { get; set; }

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
                Cache = new InMemoryCache<Symbol>();

                Cache.Set(
                    new[] {
                        // <<insert symbol definitions>>
                    });
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

            _symbol = string.Intern($"{baseAsset}{quoteAsset}");
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

            return Cache.Get(symbol) == symbol;
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

            Cache.Clear();
            Cache.Set(symbols);

            var assets = new List<Asset>();

            foreach (var symbol in symbols)
            {
                if (!assets.Contains(symbol.BaseAsset))
                    assets.Add(symbol.BaseAsset);

                if (!assets.Contains(symbol.QuoteAsset))
                    assets.Add(symbol.QuoteAsset);
            }

            Asset.Cache.Clear();
            Asset.Cache.Set(assets);
        }

        public override string ToString()
        {
            return _symbol;
        }

        public override bool Equals(object obj)
        {
            // ReSharper disable once ConvertIfStatementToSwitchStatement
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
