using System;
using System.Threading;
using System.Threading.Tasks;
using Binance.Api;
using Binance.WebSocket.Events;

namespace Binance.WebSocket
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
        /// Subscribe to the specified user and begin streaming.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="callback"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task SubscribeAndStreamAsync(IBinanceApiUser user, Action<UserDataEventArgs> callback, CancellationToken token = default);
    }
}
