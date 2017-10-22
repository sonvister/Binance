using Binance.Accounts;
using System;

namespace Binance.Api.WebSocket.Events
{
    /// <summary>
    /// User data web socket client event.
    /// </summary>
    public sealed class AccountUpdateEventArgs : EventArgs, IChronological
    {
        #region Public Properties

        /// <summary>
        /// Get the event time.
        /// </summary>
        public long Timestamp { get; private set; }

        /// <summary>
        /// Get the account.
        /// </summary>
        public Account Account { get; private set; }

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="timestamp">The event time.</param>
        /// <param name="account">The account.</param>
        public AccountUpdateEventArgs(long timestamp, Account account)
        {
            if (timestamp <= 0)
                throw new ArgumentException($"{nameof(AccountUpdateEventArgs)}: Event {nameof(timestamp)} must be greater than 0.", nameof(timestamp));

            Throw.IfNull(account, nameof(account));

            Account = account;
        }

        #endregion Constructors
    }
}
