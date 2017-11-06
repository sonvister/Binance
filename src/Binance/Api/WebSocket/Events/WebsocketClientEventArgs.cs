using System;

namespace Binance.Api.WebSocket.Events
{
    /// <summary>
    /// Web socket client event.
    /// </summary>
    public abstract class WebSocketClientEventArgs : EventArgs, IChronological
    {
        #region Public Properties

        /// <summary>
        /// Get the event time.
        /// </summary>
        public long Timestamp { get; }

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="timestamp">The event time.</param>
        protected WebSocketClientEventArgs(long timestamp)
        {
            if (timestamp <= 0)
                throw new ArgumentException($"{nameof(WebSocketClientEventArgs)} timestamp must be greater than 0.", nameof(timestamp));

            Timestamp = timestamp;
        }

        #endregion Constructors
    }
}
