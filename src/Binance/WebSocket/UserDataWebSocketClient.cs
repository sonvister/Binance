using System;
using Binance.Api;
using Binance.Client;
using Binance.Client.Events;
using Microsoft.Extensions.Logging;

namespace Binance.WebSocket
{
    /// <summary>
    /// The default <see cref="IUserDataWebSocketClient"/> implementation.
    /// </summary>
    public class UserDataWebSocketClient : BinanceWebSocketClient<IUserDataClient, UserDataEventArgs>, IUserDataWebSocketClient
    {
        #region Public Events

        public event EventHandler<AccountUpdateEventArgs> AccountUpdate
        {
            add => Client.AccountUpdate += value;
            remove => Client.AccountUpdate -= value;
        }

        public event EventHandler<OrderUpdateEventArgs> OrderUpdate
        {
            add => Client.OrderUpdate += value;
            remove => Client.OrderUpdate -= value;
        }

        public event EventHandler<AccountTradeUpdateEventArgs> TradeUpdate
        {
            add => Client.TradeUpdate += value;
            remove => Client.TradeUpdate -= value;
        }

        #endregion Public Events

        #region Constructors

        /// <summary>
        /// Default constructor provides default <see cref="IUserDataClient"/>
        /// and default <see cref="IBinanceWebSocketStream"/>, but no logger.
        /// </summary>
        public UserDataWebSocketClient()
            : this(new UserDataClient(), new BinanceWebSocketStream())
        { }

        /// <summary>
        /// The DI constructor.
        /// </summary>
        /// <param name="client">The JSON client (required).</param>
        /// <param name="stream">The web socket stream (required).</param>
        /// <param name="logger">The logger (optional).</param>
        public UserDataWebSocketClient(IUserDataClient client, IBinanceWebSocketStream stream, ILogger<UserDataWebSocketClient> logger = null)
            : base(client, stream, logger)
        { }

        #endregion Construtors

        #region Public Methods

        public virtual void Subscribe<TEventArgs>(string listenKey, IBinanceApiUser user, Action<TEventArgs> callback)
            where TEventArgs : UserDataEventArgs
            => HandleSubscribe(() => Client.Subscribe(listenKey, user, callback));

        public virtual void Unsubscribe<TEventArgs>(string listenKey, Action<TEventArgs> callback)
            where TEventArgs : UserDataEventArgs
            => HandleUnsubscribe(() => Client.Unsubscribe(listenKey, callback));

        public virtual void HandleListenKeyChange(string oldListenKey, string newListenKey)
            => Client.HandleListenKeyChange(oldListenKey, newListenKey);

        #endregion Public Methods
    }
}
