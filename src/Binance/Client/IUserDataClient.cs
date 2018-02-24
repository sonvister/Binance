using System;
using Binance.Api;
using Binance.Client.Events;

namespace Binance.Client
{
    public interface IUserDataClient : IJsonClient
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
        /// Subscribe to the specified listen key for user.
        /// </summary>
        /// <param name="listenKey">The listen key to subscribe.</param>
        /// <param name="user">The user.</param>
        /// <param name="callback">An event callback (optional).</param>
        /// <returns></returns>
        IUserDataClient Subscribe<TEventArgs>(string listenKey, IBinanceApiUser user, Action<TEventArgs> callback)
            where TEventArgs : UserDataEventArgs;

        /// <summary>
        /// Unsubscribe a callback from a listen key. If no callback is
        /// specified, then unsubscribe from listen key (all callbacks).
        /// </summary>
        /// <param name="listenKey"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        IUserDataClient Unsubscribe<TEventArgs>(string listenKey, Action<TEventArgs> callback)
            where TEventArgs : UserDataEventArgs;

        /// <summary>
        /// Unsubscribe from all symbols (and callbacks).
        /// </summary>
        /// <returns></returns>
        new IUserDataClient Unsubscribe();

        /// <summary>
        /// Replace an existing listen key with a new listen key.
        /// </summary>
        /// <param name="oldListenKey"></param>
        /// <param name="newListenKey"></param>
        void HandleListenKeyChange(string oldListenKey, string newListenKey);
    }
}
