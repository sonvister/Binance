using System;

namespace Binance.Accounts.Cache
{
    public sealed class AccountCacheEventArgs : EventArgs
    {
        #region Public Properties

        /// <summary>
        /// Get the account.
        /// </summary>
        public Account Account { get; private set; }

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="account">The account.</param>
        public AccountCacheEventArgs(Account account)
        {
            Throw.IfNull(account, nameof(account));

            Account = account;
        }

        #endregion Constructors
    }
}
