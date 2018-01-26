using System;
using System.Threading;
using Binance.Market;

namespace Binance.WebSocket.Events
{
    /// <summary>
    /// Symbol statistics web socket client event.
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
        /// <param name="token">The cancellation token.</param>
        /// <param name="statistics">The symbol statistics.</param>
        public SymbolStatisticsEventArgs(DateTime time, CancellationToken token, params SymbolStatistics[] statistics)
            : base(time, token)
        {
            Throw.IfNull(statistics, nameof(statistics));

            Statistics = statistics;
        }

        #endregion Constructors
    }
}
