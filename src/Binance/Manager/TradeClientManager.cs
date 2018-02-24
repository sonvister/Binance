using System;
using Binance.Client;
using Binance.Client.Events;
using Binance.Stream;
using Microsoft.Extensions.Logging;

namespace Binance.Manager
{
    /// <summary>
    /// The default <see cref="ITradeClientManager"/> implementation.
    /// </summary>
    public class TradeClientManager : TradeClientManager<IJsonStream>, ITradeClientManager
    {
        #region Constructors

        /// <summary>
        /// Default constructor provides default <see cref="ITradeClient"/>,
        /// but no logger.
        /// </summary>
        /// <param name="controller">The JSON stream controller (required).</param>
        /// <param name="logger">The logger (optional).</param>
        public TradeClientManager(IJsonStreamController controller, ILogger<TradeClientManager> logger = null)
            : this(new TradeClient(), controller, logger)
        { }

        /// <summary>
        /// The DI constructor.
        /// </summary>
        /// <param name="client">The JSON client (required).</param>
        /// <param name="controller">The JSON stream controller (required).</param>
        /// <param name="logger">The logger (optional).</param>
        public TradeClientManager(ITradeClient client, IJsonStreamController controller, ILogger<TradeClientManager> logger = null)
            : base(client, controller, logger)
        { }

        #endregion Constructors
    }

    public class TradeClientManager<TStream> : JsonStreamClientManager<ITradeClient, TStream>, ITradeClientManager<TStream>
        where TStream : IJsonStream
    {
        #region Public Events

        public event EventHandler<TradeEventArgs> Trade
        {
            add => Client.Trade += value;
            remove => Client.Trade -= value;
        }

        #endregion Public Events

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="client">The JSON client (required).</param>
        /// <param name="controller">The JSON stream controller (required).</param>
        /// <param name="logger">The logger (optional).</param>
        public TradeClientManager(ITradeClient client, IJsonStreamController<TStream> controller, ILogger<TradeClientManager<TStream>> logger = null)
            : base(client, controller, logger)
        { }

        #endregion Constructors

        #region Public Methods

        public virtual ITradeClient Subscribe(string symbol, Action<TradeEventArgs> callback)
            => (ITradeClient)HandleSubscribe(() => Client.Subscribe(symbol, callback));

        public virtual ITradeClient Unsubscribe(string symbol, Action<TradeEventArgs> callback)
            => (ITradeClient)HandleUnsubscribe(() => Client.Unsubscribe(symbol, callback));

        public virtual new ITradeClient Unsubscribe() => (ITradeClient)base.Unsubscribe();

        #endregion Public Methods
    }
}
