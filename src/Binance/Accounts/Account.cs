using System.Collections.Generic;

namespace Binance.Accounts
{
    /// <summary>
    /// Account information.
    /// </summary>
    public sealed class Account
    {
        #region Public Properties

        /// <summary>
        /// Get the account commissions.
        /// </summary>
        public AccountCommissions Commissions { get; private set; }

        /// <summary>
        /// Get the account status.
        /// </summary>
        public AccountStatus Status { get; private set; }

        /// <summary>
        /// Get the account balances.
        /// </summary>
        public IEnumerable<AccountBalance> Balances { get; private set; }

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="commissions">The account commissions.</param>
        /// <param name="status">The account status.</param>
        /// <param name="balances">The account balances.</param>
        public Account(AccountCommissions commissions, AccountStatus status, IEnumerable<AccountBalance> balances)
        {
            Commissions = commissions;
            Status = status;
            Balances = balances;
        }

        #endregion Constructors
    }
}
