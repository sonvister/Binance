using System;
using Binance.Client;
using Binance.Client.Events;
using Binance.Stream;
using Binance.Utility;
using Microsoft.Extensions.Logging;

namespace Binance.Manager
{
    /// <summary>
    /// The default <see cref="IAggregateTradeClientManager"/> implementation.
    /// </summary>
    public class AggregateTradeClientManager : AggregateTradeClientManager<IJsonStream>, IAggregateTradeClientManager
    {
        #region Constructors

        /// <summary>
        /// Default constructor provides default <see cref="IAggregateTradeClient"/>,
        /// but no logger.
        /// </summary>
        /// <param name="controller">The JSON stream controller provider (required).</param>
        /// <param name="logger">The logger (optional).</param>
        public AggregateTradeClientManager(IJsonStreamController controller, ILogger<AggregateTradeClientManager> logger = null)
            : this(new AggregateTradeClient(), controller, logger)
        { }

        /// <summary>
        /// The DI constructor.
        /// </summary>
        /// <param name="client">The JSON client (required).</param>
        /// <param name="controller">The JSON stream controller (required).</param>
        /// <param name="logger">The logger (optional).</param>
        public AggregateTradeClientManager(IAggregateTradeClient client, IJsonStreamController controller, ILogger<AggregateTradeClientManager> logger = null)
            : base(client, controller, logger)
        { }

        #endregion Constructors
    }

    public class AggregateTradeClientManager<TStream> : JsonStreamClientManager<IAggregateTradeClient, TStream>, IAggregateTradeClientManager<TStream>
        where TStream : IJsonStream
    {
        #region Public Events

        public event EventHandler<AggregateTradeEventArgs> AggregateTrade
        {
            add => Client.AggregateTrade += value;
            remove => Client.AggregateTrade -= value;
        }

        #endregion Public Events

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="client">The JSON client (required).</param>
        /// <param name="controller">The JSON stream controller (required).</param>
        /// <param name="logger">The logger (optional).</param>
        public AggregateTradeClientManager(IAggregateTradeClient client, IJsonStreamController<TStream> controller, ILogger<AggregateTradeClientManager<TStream>> logger = null)
            : base(client, controller, logger)
        { }

        #endregion Constructors

        #region Public Methods

        public virtual IAggregateTradeClient Subscribe(string symbol, Action<AggregateTradeEventArgs> callback)
            => (IAggregateTradeClient)HandleSubscribe(() => Client.Subscribe(symbol, callback));

        public virtual IAggregateTradeClient Unsubscribe(string symbol, Action<AggregateTradeEventArgs> callback)
            => (IAggregateTradeClient)HandleUnsubscribe(() => Client.Unsubscribe(symbol, callback));

        public virtual new IAggregateTradeClient Unsubscribe() => (IAggregateTradeClient)base.Unsubscribe();

        #endregion Public Methods
    }
}
