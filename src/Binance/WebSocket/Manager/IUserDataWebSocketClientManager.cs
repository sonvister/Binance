using System;
using System.Threading;
using System.Threading.Tasks;
using Binance.Api;
using Binance.Client.Events;
using Binance.Manager;

namespace Binance.WebSocket.Manager
{
    /// <summary>
    /// A user data web socket client manager.
    /// </summary>
    public interface IUserDataWebSocketClientManager : IControllerManager<IWebSocketStream> //IUserDataClientManager<IWebSocketStream>, IUserDataClientManager // TODO
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
        /// 
        /// </summary>
        /// <typeparam name="TEventArgs"></typeparam>
        /// <param name="user"></param>
        /// <param name="callback"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task SubscribeAsync<TEventArgs>(IBinanceApiUser user, Action<TEventArgs> callback, CancellationToken token = default)
            where TEventArgs : UserDataEventArgs;

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEventArgs"></typeparam>
        /// <param name="user"></param>
        /// <param name="callback"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task UnsubscribeAsync<TEventArgs>(IBinanceApiUser user, Action<TEventArgs> callback, CancellationToken token = default)
            where TEventArgs : UserDataEventArgs;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task UnsubscribeAllAsync(CancellationToken token = default);
    }
}
