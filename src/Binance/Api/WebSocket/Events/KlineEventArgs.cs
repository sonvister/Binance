using System;

namespace Binance.Api.WebSocket.Events
{
    /// <summary>
    /// Kline web socket client event.
    /// </summary>
    public sealed class KlineEventArgs : EventArgs, IChronological
    {
        #region Public Properties

        /// <summary>
        /// Get the event time.
        /// </summary>
        public long Timestamp { get; private set; }

        /// <summary>
        /// Get the candlestick.
        /// </summary>
        public Candlestick Candlestick { get; private set; }

        /// <summary>
        /// Get the first trade ID.
        /// </summary>
        public long FirstTradeId { get; private set; }

        /// <summary>
        /// Get the last trade ID.
        /// </summary>
        public long LastUpdateId { get; private set; }

        /// <summary>
        /// Get whether the candlestick is final.
        /// </summary>
        public bool IsFinal { get; private set; }

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="timestamp">The event time.</param>
        /// <param name="candlestick">The candlestick.</param>
        /// <param name="firstTradeId">The first trade ID.</param>
        /// <param name="lastTradeId">The last trade ID.</param>
        /// <param name="isFinal">Is candlestick final.</param>
        public KlineEventArgs(long timestamp, Candlestick candlestick, long firstTradeId, long lastTradeId, bool isFinal)
        {
            if (timestamp <= 0)
                throw new ArgumentException($"{nameof(KlineEventArgs)}: Event {nameof(timestamp)} must be greater than 0.", nameof(timestamp));

            Throw.IfNull(candlestick, nameof(candlestick));

            Timestamp = timestamp;

            Candlestick = candlestick;

            FirstTradeId = firstTradeId;
            LastUpdateId = lastTradeId;

            IsFinal = isFinal;
        }

        #endregion Constructors
    }
}
