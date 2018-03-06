using System;
using System.Threading;
using System.Threading.Tasks;
using Binance.Client;

// ReSharper disable once CheckNamespace
namespace Binance.WebSocket
{
    /// <summary>
    /// A user data web socket manager.
    /// </summary>
    public interface IUserDataWebSocketManager : IDisposable
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
        /// Get the client.
        /// </summary>
        IUserDataWebSocketClient Client { get; }

        /// <summary>
        /// Subscribe to user events.
        /// </summary>
        /// <typeparam name="TEventArgs"></typeparam>
        /// <param name="user"></param>
        /// <param name="callback"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task SubscribeAsync<TEventArgs>(IBinanceApiUser user, Action<TEventArgs> callback, CancellationToken token = default)
            where TEventArgs : UserDataEventArgs;

        /// <summary>
        /// Unsubscribe from user events.
        /// </summary>
        /// <typeparam name="TEventArgs"></typeparam>
        /// <param name="user"></param>
        /// <param name="callback"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task UnsubscribeAsync<TEventArgs>(IBinanceApiUser user, Action<TEventArgs> callback, CancellationToken token = default)
            where TEventArgs : UserDataEventArgs;

        /// <summary>
        /// Unsubscribe from all user events.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task UnsubscribeAllAsync(CancellationToken token = default);
    }
}
