using System;
using Binance.Api;
using Binance.Client;
using Binance.Client.Events;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace Binance.WebSocket
{
    /// <summary>
    /// The default <see cref="IUserDataWebSocketClient"/> implementation.
    /// </summary>
    public class UserDataWebSocketClient : BinanceWebSocketClient<IWebSocketStream, IUserDataClient, UserDataEventArgs>, IUserDataWebSocketClient
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

        public event EventHandler<ErrorEventArgs> Error
        {
            add => Publisher.Controller.Error += value;
            remove => Publisher.Controller.Error -= value;
        }

        #endregion Public Events

        #region Constructors

        /// <summary>
        /// Default constructor provides default <see cref="IUserDataClient"/>
        /// and default <see cref="IBinanceWebSocketStreamPublisher"/>, but no logger.
        /// </summary>
        public UserDataWebSocketClient()
            : this(new UserDataClient(), new BinanceWebSocketStreamPublisher())
        { }

        /// <summary>
        /// The DI constructor.
        /// </summary>
        /// <param name="client">The JSON client (required).</param>
        /// <param name="publisher">The web socket stream publisher (required).</param>
        /// <param name="logger">The logger (optional).</param>
        public UserDataWebSocketClient(IUserDataClient client, IBinanceWebSocketStreamPublisher publisher, ILogger<UserDataWebSocketClient> logger = null)
            : base(client, publisher, logger)
        { }

        #endregion Construtors

        #region Public Methods

        public virtual IUserDataClient Subscribe<TEventArgs>(string listenKey, IBinanceApiUser user, Action<TEventArgs> callback)
            where TEventArgs : UserDataEventArgs
            => (IUserDataClient)HandleSubscribe(() => Client.Subscribe(listenKey, user, callback));

        public virtual IUserDataClient Unsubscribe<TEventArgs>(string listenKey, Action<TEventArgs> callback)
            where TEventArgs : UserDataEventArgs
            => (IUserDataClient)HandleUnsubscribe(() => Client.Unsubscribe(listenKey, callback));

        public virtual void HandleListenKeyChange(string oldListenKey, string newListenKey)
        {
            Client.HandleListenKeyChange(oldListenKey, newListenKey);

            Publisher.Unsubscribe(Client, oldListenKey);

            Publisher.Subscribe(Client, newListenKey);
        }

        #endregion Public Methods
    }
}
