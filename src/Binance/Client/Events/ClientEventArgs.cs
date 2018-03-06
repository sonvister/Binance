using System;

// ReSharper disable once CheckNamespace
namespace Binance.Client
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

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="time">The event time.</param>
        protected ClientEventArgs(DateTime time)
        {
            Time = time;
        }

        #endregion Constructors
    }
}
