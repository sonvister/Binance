// ReSharper disable InconsistentNaming
using System;
using Binance.Cache;

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
        public static Asset BCH => BCC;

        #endregion Public Constants

        #region Implicit Operators

        public static bool operator ==(Asset x, Asset y) => Equals(x, y);

        public static bool operator !=(Asset x, Asset y) => !(x == y);

        public static implicit operator string(Asset asset) => asset?.ToString();

        #endregion Implicit Operators

        #region Public Properties

        /// <summary>
        /// Asset cache.
        /// </summary>
        public static IObjectCache<Asset> Cache { get; set; }

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
                Cache = new InMemoryCache<Asset>();

                Cache.Set(
                    new[] {
                        // <<insert asset definitions>>
                    });

                AddCacheRedirections();
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

            Symbol = string.Intern(symbol.ToUpperInvariant());
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

            return Cache.Get(asset) == asset;
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

        internal static void AddCacheRedirections()
        {
            // Redirect (BCH) Bitcoin Cash (BCC = BitConnect)
            Cache.Set("BCH", Cache.Get("BCC"));
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
