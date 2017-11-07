using System;
using Binance.Account;

namespace Binance.Cache.Events
{
    public sealed class AccountInfoCacheEventArgs : EventArgs
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
        /// <param name="accountInfo">The account information.</param>
        public AccountInfoCacheEventArgs(AccountInfo accountInfo)
        {
            Throw.IfNull(accountInfo, nameof(accountInfo));

            AccountInfo = accountInfo;
        }

        #endregion Constructors
    }
}
