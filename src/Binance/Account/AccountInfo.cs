using Binance.Api;
using System.Collections.Generic;

namespace Binance.Account
{
    /// <summary>
    /// Account information.
    /// </summary>
    public sealed class AccountInfo
    {
        #region Public Properties

        /// <summary>
        /// Get the account user.
        /// </summary>
        public IBinanceUser User { get; private set; }

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
        /// <param name="user">The user.</param>
        /// <param name="commissions">The account commissions.</param>
        /// <param name="status">The account status.</param>
        /// <param name="balances">The account balances.</param>
        public AccountInfo(IBinanceUser user, AccountCommissions commissions, AccountStatus status, IEnumerable<AccountBalance> balances = null)
        {
            Throw.IfNull(user, nameof(user));
            Throw.IfNull(commissions, nameof(commissions));
            Throw.IfNull(status, nameof(status));

            User = user;
            Commissions = commissions;
            Status = status;

            Balances = balances ?? new AccountBalance[] { };
        }

        #endregion Constructors
    }
}
