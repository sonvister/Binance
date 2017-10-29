using System;
using System.Collections.Generic;

namespace Binance.Api.WebSocket.Events
{
    /// <summary>
    /// Depth web socket client event.
    /// </summary>
    public sealed class DepthUpdateEventArgs : EventArgs, IChronological
    {
        #region Public Properties

        /// <summary>
        /// The symbol.
        /// </summary>
        public string Symbol { get; }

        /// <summary>
        /// The event time.
        /// </summary>
        public long Timestamp { get; }

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
        /// <param name="timestamp">The event time.</param>
        /// <param name="symbol">The symbol.</param>
        /// <param name="firstUpdateId"></param>
        /// <param name="lastUpdateId"></param>
        /// <param name="bids">The bids.</param>
        /// <param name="asks">The asks.</param>
        public DepthUpdateEventArgs(long timestamp, string symbol, long firstUpdateId, long lastUpdateId, IEnumerable<(decimal, decimal)> bids, IEnumerable<(decimal, decimal)> asks)
        {
            if (timestamp <= 0)
                throw new ArgumentException($"{nameof(DepthUpdateEventArgs)} timestamp must be greater than 0.", nameof(timestamp));

            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            if (firstUpdateId < 0)
                throw new ArgumentException($"{nameof(DepthUpdateEventArgs)}: Update ID must not be less than 0.", nameof(firstUpdateId));
            if (lastUpdateId < 0)
                throw new ArgumentException($"{nameof(DepthUpdateEventArgs)}: Update ID must not be less than 0.", nameof(lastUpdateId));
            if (lastUpdateId < firstUpdateId)
                throw new ArgumentException($"{nameof(DepthUpdateEventArgs)}: Last update ID must be greater than or equal to first update ID.", nameof(lastUpdateId));

            Throw.IfNull(bids, nameof(bids));
            Throw.IfNull(asks, nameof(asks));

            Timestamp = timestamp;
            Symbol = symbol;

            FirstUpdateId = firstUpdateId;
            LastUpdateId = lastUpdateId;

            Bids = bids;
            Asks = asks;
        }

        #endregion Constructors
    }
}
