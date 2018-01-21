using System;
using System.Threading;
using System.Threading.Tasks;
using Binance.Api;
using Binance.WebSocket.Events;

namespace Binance.WebSocket
{
    public interface IUserDataWebSocketClient : IBinanceWebSocketClient
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
        event EventHandler<AccountTradeUpdateEventArgs> TradeUpdate;

        #endregion Public Events

        #region Public Methods

        /// <summary>
        /// Subscribe to the specified user (for use with combined streams).
        /// Call <see cref="IWebSocketStream"/> StreamAsync to begin streaming.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="callback"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task SubscribeAsync(IBinanceApiUser user, Action<UserDataEventArgs> callback, CancellationToken token = default);

        #endregion Public Methods
    }
}
