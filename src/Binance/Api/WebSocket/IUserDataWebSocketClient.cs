using Binance.Api.WebSocket.Events;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Binance
{
    public interface IUserDataWebSocketClient : IDisposable
    {
        #region Public Events

        /// <summary>
        /// The account update event.
        /// </summary>
        event EventHandler<AccountUpdateEventArgs> AccountUpdate;

        #endregion Public Events

        #region Public Methods

        /// <summary>
        /// Subscribe to the specified user key and begin receiving account
        /// update events. Awaiting this method will not return until the token
        /// is canceled, this <see cref="IUserDataWebSocketClient"> is disposed,
        /// or an exception occurs.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns><see cref="Task"/></returns>
        Task SubscribeAsync(IBinanceUser user, CancellationToken token = default);

        #endregion Public Methods
    }
}
