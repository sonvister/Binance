using System;
using System.Threading;

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

        /// <summary>
        /// Get the cancellation token.
        /// </summary>
        public CancellationToken Token { get; }

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="timestamp">The event time.</param>
        /// <param name="token"></param>
        protected WebSocketClientEventArgs(long timestamp, CancellationToken token)
        {
            if (timestamp <= 0)
                throw new ArgumentException($"{nameof(WebSocketClientEventArgs)} timestamp must be greater than 0.", nameof(timestamp));

            Timestamp = timestamp;
            Token = token;
        }

        #endregion Constructors
    }
}
