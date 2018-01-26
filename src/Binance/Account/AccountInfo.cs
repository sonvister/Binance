using System;
using System.Collections.Generic;
using Binance.Api;

namespace Binance.Account
{
    /// <summary>
    /// Account information.
    /// </summary>
    public sealed class AccountInfo : IChronological
    {
        #region Public Properties

        /// <summary>
        /// Get the account user.
        /// </summary>
        public IBinanceApiUser User { get; }

        /// <summary>
        /// Get the account commissions.
        /// </summary>
        public AccountCommissions Commissions { get; }

        /// <summary>
        /// Get the account status.
        /// </summary>
        public AccountStatus Status { get; }
        
        /// <summary>
        /// Get the account update time.
        /// </summary>
        public DateTime Time { get; }

        /// <summary>
        /// Get the account balances.
        /// </summary>
        public IEnumerable<AccountBalance> Balances { get; }

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="commissions">The account commissions.</param>
        /// <param name="status">The account status.</param>
        /// <param name="time">The update time.</param>
        /// <param name="balances">The account balances.</param>
        public AccountInfo(IBinanceApiUser user, AccountCommissions commissions, AccountStatus status, DateTime time, IEnumerable<AccountBalance> balances = null)
        {
            Throw.IfNull(user, nameof(user));
            Throw.IfNull(commissions, nameof(commissions));
            Throw.IfNull(status, nameof(status));

            User = user;
            Commissions = commissions;
            Status = status;
            Time = time;

            Balances = balances ?? new AccountBalance[] { };
        }

        #endregion Constructors
    }
}
