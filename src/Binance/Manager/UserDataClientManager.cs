using System;
using Binance.Api;
using Binance.Client;
using Binance.Client.Events;
using Binance.Stream;
using Binance.Utility;
using Microsoft.Extensions.Logging;

namespace Binance.Manager
{
    /// <summary>
    /// The default <see cref="IUserDataClientManager"/> implementation.
    /// </summary>
    public class UserDataClientManager : UserDataClientManager<IJsonStream>, IUserDataClientManager
    {
        #region Constructors

        /// <summary>
        /// Default constructor provides default <see cref="IUserDataClient"/>,
        /// but no logger.
        /// </summary>
        /// <param name="controller">The JSON stream controller provider (required).</param>
        /// <param name="logger">The logger (optional).</param>
        public UserDataClientManager(IJsonStreamController controller, ILogger<UserDataClientManager> logger = null)
            : this(new UserDataClient(), controller, logger)
        { }

        /// <summary>
        /// The DI constructor.
        /// </summary>
        /// <param name="client">The JSON client (required).</param>
        /// <param name="controller">The JSON stream controller (required).</param>
        /// <param name="logger">The logger (optional).</param>
        public UserDataClientManager(IUserDataClient client, IJsonStreamController controller, ILogger<UserDataClientManager> logger = null)
            : base(client, controller, logger)
        { }

        #endregion Constructors
    }

    public class UserDataClientManager<TStream> : JsonStreamClientManager<IUserDataClient, TStream>, IUserDataClientManager<TStream>
        where TStream : IJsonStream
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
        /// Constructor.
        /// </summary>
        /// <param name="client">The JSON client (required).</param>
        /// <param name="controller">The JSON stream controller (required).</param>
        /// <param name="logger">The logger (optional).</param>
        public UserDataClientManager(IUserDataClient client, IJsonStreamController<TStream> controller, ILogger<UserDataClientManager<TStream>> logger = null)
            : base(client, controller, logger)
        {
            Watchdog.Interval = TimeSpan.FromHours(8);
        }

        #endregion Constructors

        #region Public Methods

        public virtual IUserDataClient Subscribe<TEventArgs>(string listenKey, IBinanceApiUser user, Action<TEventArgs> callback)
            where TEventArgs : UserDataEventArgs
            => (IUserDataClient)HandleSubscribe(() => Client.Subscribe(listenKey, user, callback));

        public virtual IUserDataClient Unsubscribe<TEventArgs>(string listenKey, Action<TEventArgs> callback)
            where TEventArgs : UserDataEventArgs
            => (IUserDataClient)HandleUnsubscribe(() => Client.Unsubscribe(listenKey, callback));

        public virtual new IUserDataClient Unsubscribe() => (IUserDataClient)base.Unsubscribe();

        public virtual void HandleListenKeyChange(string oldListenKey, string newListenKey)
            => Client.HandleListenKeyChange(oldListenKey, newListenKey);

        #endregion Public Methods
    }
}
