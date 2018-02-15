using System;
using System.Collections.Generic;
using System.Threading;

namespace Binance.Client.Events
{
    /// <summary>
    /// Depth client event arguments.
    /// </summary>
    public sealed class DepthUpdateEventArgs : ClientEventArgs
    {
        #region Public Properties

        /// <summary>
        /// The symbol.
        /// </summary>
        public string Symbol { get; }

        /// <summary>
        /// The first update ID (inclusive).
        /// </summary>
        public long FirstUpdateId { get; }

        /// <summary>
        /// The last update ID (inclusive).
        /// </summary>
        public long LastUpdateId { get; }

        /// <summary>
        /// The bids (price and quantity) to update.
        /// </summary>
        public IEnumerable<(decimal, decimal)> Bids { get; }

        /// <summary>
        /// The asks (price and quantity) to update.
        /// </summary>
        public IEnumerable<(decimal, decimal)> Asks { get; }

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="time">The event time.</param>
        /// <param name="token">The cancellation token.</param>
        /// <param name="symbol">The symbol.</param>
        /// <param name="firstUpdateId"></param>
        /// <param name="lastUpdateId"></param>
        /// <param name="bids">The bids.</param>
        /// <param name="asks">The asks.</param>
        public DepthUpdateEventArgs(DateTime time, CancellationToken token, string symbol, long firstUpdateId, long lastUpdateId, IEnumerable<(decimal, decimal)> bids, IEnumerable<(decimal, decimal)> asks)
            : base(time, token)
        {
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            if (firstUpdateId < 0)
                throw new ArgumentException($"{nameof(DepthUpdateEventArgs)}: Update ID must not be less than 0.", nameof(firstUpdateId));
            if (lastUpdateId < 0)
                throw new ArgumentException($"{nameof(DepthUpdateEventArgs)}: Update ID must not be less than 0.", nameof(lastUpdateId));
            if (lastUpdateId < firstUpdateId)
                throw new ArgumentException($"{nameof(DepthUpdateEventArgs)}: Last update ID must be greater than or equal to first update ID.", nameof(lastUpdateId));

            Throw.IfNull(bids, nameof(bids));
            Throw.IfNull(asks, nameof(asks));

            Symbol = symbol;

            FirstUpdateId = firstUpdateId;
            LastUpdateId = lastUpdateId;

            Bids = bids;
            Asks = asks;
        }

        #endregion Constructors
    }
}
