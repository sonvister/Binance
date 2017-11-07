using System.Threading;
using Binance.Account;

namespace Binance.Api.WebSocket.Events
{
    /// <summary>
    /// User data web socket client event.
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
        /// <param name="timestamp">The event time.</param>
        /// <param name="token">The cancellation token.</param>
        /// <param name="accountInfo">The account information.</param>
        public AccountUpdateEventArgs(long timestamp, CancellationToken token, AccountInfo accountInfo)
            : base(timestamp, token)
        {
            Throw.IfNull(accountInfo, nameof(accountInfo));

            AccountInfo = accountInfo;
        }

        #endregion Constructors
    }
}
