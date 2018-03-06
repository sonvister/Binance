using System;

// ReSharper disable once CheckNamespace
namespace Binance.Client
{
    /// <summary>
    /// User data client event arguments.
    /// </summary>
    public sealed class AccountUpdateEventArgs : UserDataEventArgs
    {
        #region Public Properties

        /// <summary>
        /// Get the account information.
        /// </summary>
        public AccountInfo AccountInfo { get; }

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="time">The event time.</param>
        /// <param name="accountInfo">The account information.</param>
        public AccountUpdateEventArgs(DateTime time, AccountInfo accountInfo)
            : base(time)
        {
            Throw.IfNull(accountInfo, nameof(accountInfo));

            AccountInfo = accountInfo;
        }

        #endregion Constructors
    }
}
