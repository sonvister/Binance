using System;

// ReSharper disable once CheckNamespace
namespace Binance.Client
{
    /// <summary>
    /// Symbol statistics client event arguments.
    /// </summary>
    public sealed class SymbolStatisticsEventArgs : ClientEventArgs
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
        /// <param name="time">The event time.</param>
        /// <param name="statistics">The symbol statistics.</param>
        public SymbolStatisticsEventArgs(DateTime time, params SymbolStatistics[] statistics)
            : base(time)
        {
            Throw.IfNull(statistics, nameof(statistics));

            Statistics = statistics;
        }

        #endregion Constructors
    }
}
