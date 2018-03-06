using System;
using Binance.Client;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace Binance.WebSocket
{
    /// <summary>
    /// The default <see cref="IAggregateTradeWebSocketClient"/> implementation.
    /// </summary>
    public class AggregateTradeWebSocketClient : AutoBinanceWebSocketClient<IWebSocketStream, IAggregateTradeClient, AggregateTradeEventArgs>, IAggregateTradeWebSocketClient
    {
        #region Public Events

        public event EventHandler<AggregateTradeEventArgs> AggregateTrade
        {
            add => Client.AggregateTrade += value;
            remove => Client.AggregateTrade -= value;
        }

        public event EventHandler<ErrorEventArgs> Error
        {
            add => Publisher.Controller.Error += value;
            remove => Publisher.Controller.Error -= value;
        }

        #endregion Public Events

        #region Constructors

        /// <summary>
        /// Default constructor provides default <see cref="IAggregateTradeClient"/>
        /// and default <see cref="IBinanceWebSocketStreamPublisher"/>, but no logger.
        /// </summary>
        public AggregateTradeWebSocketClient()
            : this(new AggregateTradeClient(), new BinanceWebSocketStreamPublisher())
        { }

        /// <summary>
        /// The DI constructor.
        /// </summary>
        /// <param name="client">The JSON client (required).</param>
        /// <param name="publisher">The web socket stream publisher (required).</param>
        /// <param name="logger">The logger (optional).</param>
        public AggregateTradeWebSocketClient(IAggregateTradeClient client, IBinanceWebSocketStreamPublisher publisher, ILogger<AggregateTradeWebSocketClient> logger = null)
            : base(client, publisher, logger)
        { }

        #endregion Construtors

        #region Public Methods

        public virtual IAggregateTradeClient Subscribe(string symbol, Action<AggregateTradeEventArgs> callback)
            => (IAggregateTradeClient)HandleSubscribe(() => Client.Subscribe(symbol, callback));

        public virtual IAggregateTradeClient Unsubscribe(string symbol, Action<AggregateTradeEventArgs> callback)
            => (IAggregateTradeClient)HandleUnsubscribe(() => Client.Unsubscribe(symbol, callback));

        #endregion Public Methods
    }
}
