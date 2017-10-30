using Binance.Account;
using System;

namespace Binance.Cache.Events
{
    public sealed class AccountInfoCacheEventArgs : EventArgs
    {
        #region Public Properties

        /// <summary>
        /// Get the account.
        /// </summary>
        public AccountInfo Account { get; }

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="account">The account.</param>
        public AccountInfoCacheEventArgs(AccountInfo account)
        {
            Throw.IfNull(account, nameof(account));

            Account = account;
        }

        #endregion Constructors
    }
}
