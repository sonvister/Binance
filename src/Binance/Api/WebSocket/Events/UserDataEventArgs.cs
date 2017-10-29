using System;

namespace Binance.Api.WebSocket.Events
{
    /// <summary>
    /// User data web socket client event.
    /// </summary>
    public abstract class UserDataEventArgs : EventArgs, IChronological
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
        public UserDataEventArgs(long timestamp)
        {
            if (timestamp <= 0)
                throw new ArgumentException($"{nameof(UserDataEventArgs)} timestamp must be greater than 0.", nameof(timestamp));

            Timestamp = timestamp;
        }

        #endregion Constructors
    }
}
