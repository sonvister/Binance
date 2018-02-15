using System;
using System.Threading;

namespace Binance.Client.Events
{
    /// <summary>
    /// An abstract client event arguments base class.
    /// </summary>
    public abstract class ClientEventArgs : EventArgs, IChronological
    {
        #region Public Properties

        /// <summary>
        /// Get the event time.
        /// </summary>
        public DateTime Time { get; }

        /// <summary>
        /// Get the cancellation token.
        /// </summary>
        public CancellationToken Token { get; }

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="time">The event time.</param>
        /// <param name="token"></param>
        protected ClientEventArgs(DateTime time, CancellationToken token)
        {
            Time = time;
            Token = token;
        }

        #endregion Constructors
    }
}
