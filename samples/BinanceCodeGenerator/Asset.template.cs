// ReSharper disable InconsistentNaming
using System;
using System.Collections.Generic;
using System.Linq;

namespace Binance
{
    /// <summary>
    /// Defined assets (for convenience/reference only).
    /// </summary>
    /// <remarks></remarks>
    public sealed class Asset : IComparable<Asset>, IEquatable<Asset>
    {
        #region Public Constants

        /// <summary>
        /// When the assets were last updated.
        /// </summary>
        // <<insert timestamp>>

        // <<insert assets>>

        // Redirect (BCH) Bitcoin Cash (BCC = BitConnect)
        public static readonly Asset BCH;

        #endregion Public Constants

        #region Implicit Operators

        public static bool operator ==(Asset x, Asset y) => Equals(x, y);

        public static bool operator !=(Asset x, Asset y) => !(x == y);

        public static implicit operator string(Asset asset) => asset?.ToString();

        public static implicit operator Asset(string s)
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
        /// Asset cache.
        /// </summary>
        public static IDictionary<string, Asset> Cache { get; }

        /// <summary>
        /// Get the asset symbol.
        /// </summary>
        public string Symbol { get; }

        /// <summary>
        /// Get the asset precision.
        /// </summary>
        public int Precision { get; }

        #endregion Public Properties

        #region Private Fields

        private static readonly object _sync = new object();

        #endregion Private Fields

        #region Constructors

        static Asset()
        {
            try
            {
                // Redirect (BCH) Bitcoin Cash (BCC = BitConnect)
                BCH = BCC;

                Cache = new Dictionary<string, Asset>
                {
                    // <<insert asset definitions>>
            
                    // Redirect (BCH) Bitcoin Cash (BCC = BitConnect)
                    { "BCH", BCC }
                };
            }
            catch (Exception e)
            {
                Console.Error?.WriteLine($"{nameof(Binance)}.{nameof(Asset)}(): \"{e.Message}\"");
            }
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="symbol">The asset symbol.</param>
        /// <param name="precision">The asset precision.</param>
        public Asset(string symbol, int precision)
        {
            if (string.IsNullOrWhiteSpace(symbol))
                throw new ArgumentNullException(nameof(symbol));

            Symbol = symbol.ToUpperInvariant();
            Precision = precision;
        }

        #endregion Constructors

        #region Public Methods

        /// <summary>
        /// Verify that asset is valid. If fails, but known to be valid,
        /// call Symbol.UpdateCacheAsync() to get the latest assets.
        /// </summary>
        /// <param name="asset"></param>
        /// <returns></returns>
        public static bool IsValid(string asset)
        {
            if (string.IsNullOrWhiteSpace(asset))
                return false;

            asset = asset.FormatSymbol();

            lock (_sync)
            {
                return Cache.ContainsKey(asset)
                    && Cache[asset].ToString() == asset;
            }
        }

        public override bool Equals(object obj)
        {
            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (obj == null)
                return false;

            if (obj is Asset asset)
                return Equals(asset);

            return Symbol.Equals(obj);
        }

        public override int GetHashCode()
        {
            return Symbol.GetHashCode();
        }

        public override string ToString()
        {
            return Symbol;
        }

        #endregion Public Methods

        #region Internal Methods

        /// <summary>
        /// Update the asset cache.
        /// </summary>
        /// <param name="symbols">The symbols.</param>
        /// <returns></returns>
        internal static void UpdateCache(IEnumerable<Symbol> symbols)
        {
            Throw.IfNull(symbols, nameof(symbols));

            // ReSharper disable once PossibleMultipleEnumeration
            if (!symbols.Any())
                throw new ArgumentException("Enumerable must not be empty.", nameof(symbols));

            var assets = new List<Asset>();

            // ReSharper disable once PossibleMultipleEnumeration
            foreach (var symbol in symbols)
            {
                if (!assets.Contains(symbol.BaseAsset))
                    assets.Add(symbol.BaseAsset);

                if (!assets.Contains(symbol.QuoteAsset))
                    assets.Add(symbol.QuoteAsset);
            }

            lock (_sync)
            {
                // Remove any old assets (preserves redirections).
                foreach (var asset in Cache.Values.ToArray())
                {
                    if (!assets.Contains(asset))
                    {
                        Cache.Remove(asset);
                    }
                }

                // Update existing and add any new assets.
                foreach (var asset in assets)
                {
                    Cache[asset] = asset;
                }
            }
        }

        #endregion Internal Methods

        #region IComparable<Asset>

        public int CompareTo(Asset other)
        {
            return other == null ? 1 : string.Compare(Symbol, other.Symbol, StringComparison.Ordinal);
        }

        #endregion IComparable<Asset>

        #region IEquatable<Asset>

        public bool Equals(Asset other)
        {
            return CompareTo(other) == 0;
        }

        #endregion IEquatable<Asset>
    }
}
