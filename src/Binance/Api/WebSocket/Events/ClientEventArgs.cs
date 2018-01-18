using System;
using System.Threading;

namespace Binance.Api.WebSocket.Events
{
    /// <summary>
    /// Binance web socket client event.
    /// </summary>
    public abstract class ClientEventArgs : EventArgs, IChronological
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
        protected ClientEventArgs(long timestamp, CancellationToken token)
        {
            if (timestamp <= 0)
                throw new ArgumentException($"{nameof(ClientEventArgs)} timestamp must be greater than 0.", nameof(timestamp));

            Timestamp = timestamp;
            Token = token;
        }

        #endregion Constructors
    }
}
