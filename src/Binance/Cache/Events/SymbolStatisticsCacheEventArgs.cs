// ReSharper disable once CheckNamespace
namespace Binance.Cache
{
    public sealed class SymbolStatisticsCacheEventArgs : CacheEventArgs
    {
        #region Public Properties

        /// <summary>
        /// Get the symbol statistics.
        /// </summary>
        public SymbolStatistics[] Statistics { get; }

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="statistics">The symbol statistics.</param>
        public SymbolStatisticsCacheEventArgs(params SymbolStatistics[] statistics)
        {
            Throw.IfNull(statistics, nameof(statistics));

            Statistics = statistics;
        }

        #endregion Constructors
    }
}
