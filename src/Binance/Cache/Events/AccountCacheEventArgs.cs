using Binance.Account;
using System;

namespace Binance.Cache.Events
{
    public sealed class AccountCacheEventArgs : EventArgs
    {
        #region Public Properties

        /// <summary>
        /// Get the account.
        /// </summary>
        public AccountInfo Account { get; private set; }

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="account">The account.</param>
        public AccountCacheEventArgs(AccountInfo account)
        {
            Throw.IfNull(account, nameof(account));

            Account = account;
        }

        #endregion Constructors
    }
}
