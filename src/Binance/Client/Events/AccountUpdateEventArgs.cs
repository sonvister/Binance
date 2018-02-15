using System;
using System.Threading;
using Binance.Account;

namespace Binance.Client.Events
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
        /// <param name="token">The cancellation token.</param>
        /// <param name="accountInfo">The account information.</param>
        public AccountUpdateEventArgs(DateTime time, CancellationToken token, AccountInfo accountInfo)
            : base(time, token)
        {
            Throw.IfNull(accountInfo, nameof(accountInfo));

            AccountInfo = accountInfo;
        }

        #endregion Constructors
    }
}
