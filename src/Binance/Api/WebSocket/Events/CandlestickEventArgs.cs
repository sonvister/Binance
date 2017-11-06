using System;
using Binance.Market;

namespace Binance.Api.WebSocket.Events
{
    /// <summary>
    /// Candlestick/K-Line web socket client event.
    /// </summary>
    public sealed class CandlestickEventArgs : WebSocketClientEventArgs
    {
        #region Public Properties

        /// <summary>
        /// Get the candlestick.
        /// </summary>
        public Candlestick Candlestick { get; }

        /// <summary>
        /// Get the first trade ID.
        /// </summary>
        public long FirstTradeId { get; }

        /// <summary>
        /// Get the last trade ID.
        /// </summary>
        public long LastTradeId { get; }

        /// <summary>
        /// Get whether the candlestick is final.
        /// </summary>
        public bool IsFinal { get; }

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
        public CandlestickEventArgs(long timestamp, Candlestick candlestick, long firstTradeId, long lastTradeId, bool isFinal)
            : base(timestamp)
        {
            Throw.IfNull(candlestick, nameof(candlestick));

            if (firstTradeId < 0)
                throw new ArgumentException($"{nameof(CandlestickEventArgs)}: Trade ID must be greater than 0.", nameof(firstTradeId));
            if (lastTradeId < 0)
                throw new ArgumentException($"{nameof(CandlestickEventArgs)}: Trade ID must be greater than 0.", nameof(lastTradeId));
            if (lastTradeId < firstTradeId)
                throw new ArgumentException($"{nameof(CandlestickEventArgs)}: Last trade ID must be greater than or equal to first trade ID.", nameof(lastTradeId));

            Candlestick = candlestick;
            FirstTradeId = firstTradeId;
            LastTradeId = lastTradeId;
            IsFinal = isFinal;
        }

        #endregion Constructors
    }
}
