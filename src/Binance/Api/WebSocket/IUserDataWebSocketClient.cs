using System;
using System.Threading;
using System.Threading.Tasks;
using Binance.Api.WebSocket.Events;

namespace Binance.Api.WebSocket
{
    public interface IUserDataWebSocketClient : IDisposable
    {
        #region Public Events

        /// <summary>
        /// The account update event.
        /// </summary>
        event EventHandler<AccountUpdateEventArgs> AccountUpdate;

        /// <summary>
        /// The order update event.
        /// </summary>
        event EventHandler<OrderUpdateEventArgs> OrderUpdate;

        /// <summary>
        /// The trade update event.
        /// </summary>
        event EventHandler<TradeUpdateEventArgs> TradeUpdate;

        #endregion Public Events

        #region Public Methods

        /// <summary>
        /// Subscribe to the specified user key and begin receiving account
        /// update events. Awaiting this method will not return until the token
        /// is canceled, this <see cref="IUserDataWebSocketClient"/> is disposed,
        /// or an exception occurs.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns><see cref="Task"/></returns>
        Task SubscribeAsync(IBinanceApiUser user, CancellationToken token = default);

        /// <summary>
        /// Subscribe to the specified user key and begin receiving account
        /// update events. Awaiting this method will not return until the token
        /// is canceled, this <see cref="IUserDataWebSocketClient"/> is disposed,
        /// or an exception occurs.
        /// </summary>
        /// <param name="user">The user.</param>
        /// <param name="callback">An event callback.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns><see cref="Task"/></returns>
        Task SubscribeAsync(IBinanceApiUser user, Action<UserDataEventArgs> callback, CancellationToken token = default);

        #endregion Public Methods
    }
}
