using System;

// ReSharper disable once CheckNamespace
namespace Binance.Client
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
        protected UserDataEventArgs(DateTime time)
            : base(time)
        { }

        #endregion Constructors
    }
}
