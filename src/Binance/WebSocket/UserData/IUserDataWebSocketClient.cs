using System;
using Binance.Api;
using Binance.WebSocket.Events;

namespace Binance.WebSocket.UserData
{
    public interface IUserDataWebSocketClient : IBinanceWebSocketClient
    {
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

        /// <summary>
        /// Subscribe to the specified listen key (for use with combined streams).
        /// Call <see cref="IWebSocketStream"/> StreamAsync to begin streaming.
        /// </summary>
        /// <param name="listenKey">The listen key to subscribe.</param>
        /// <param name="user">The user.</param>
        /// <param name="callback">An event callback (optional).</param>
        void Subscribe(string listenKey, IBinanceApiUser user, Action<UserDataEventArgs> callback);

        /// <summary>
        /// Unsubscribe a callback from listen key events. If no callback is
        /// specified, then unsubscribe listen key (all callbacks).
        /// </summary>
        /// <param name="listenKey"></param>
        /// <param name="callback"></param>
        void Unsubscribe(string listenKey, Action<UserDataEventArgs> callback);
    }
}
