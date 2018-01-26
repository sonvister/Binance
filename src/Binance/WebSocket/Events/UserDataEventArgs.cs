using System;
using System.Threading;

namespace Binance.WebSocket.Events
{
    /// <summary>
    /// User data web socket client event.
    /// </summary>
    public abstract class UserDataEventArgs : ClientEventArgs
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="time">The event time.</param>
        /// <param name="token">The cancellation token.</param>
        protected UserDataEventArgs(DateTime time, CancellationToken token)
            : base(time, token)
        { }

        #endregion Constructors
    }
}
