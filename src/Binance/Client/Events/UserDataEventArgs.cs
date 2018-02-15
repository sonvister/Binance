using System;
using System.Threading;

namespace Binance.Client.Events
{
    /// <summary>
    /// An abstract user data client event arguments base class.
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
