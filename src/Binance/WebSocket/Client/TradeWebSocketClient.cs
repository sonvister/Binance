using System;
using Binance.Client;
using Binance.Client.Events;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace Binance.WebSocket
{
    /// <summary>
    /// The default <see cref="ITradeWebSocketClient"/> implementation.
    /// </summary>
    public class TradeWebSocketClient : BinanceWebSocketClient<IWebSocketStream, ITradeClient, TradeEventArgs>, ITradeWebSocketClient
    {
        #region Public Events

        public event EventHandler<TradeEventArgs> Trade
        {
            add => Client.Trade += value;
            remove => Client.Trade -= value;
        }

        public event EventHandler<ErrorEventArgs> Error
        {
            add => Publisher.Controller.Error += value;
            remove => Publisher.Controller.Error -= value;
        }

        #endregion Public Events

        #region Constructors

        /// <summary>
        /// Default constructor provides default <see cref="ITradeClient"/>
        /// and default <see cref="IBinanceWebSocketStreamPublisher"/>, but no logger.
        /// </summary>
        public TradeWebSocketClient()
            : this(new TradeClient(), new BinanceWebSocketStreamPublisher())
        { }

        /// <summary>
        /// The DI constructor.
        /// </summary>
        /// <param name="client">The JSON client (required).</param>
        /// <param name="publisher">The web socket stream publisher (required).</param>
        /// <param name="logger">The logger (optional).</param>
        public TradeWebSocketClient(ITradeClient client, IBinanceWebSocketStreamPublisher publisher, ILogger<TradeWebSocketClient> logger = null)
            : base(client, publisher, logger)
        { }

        #endregion Construtors

        #region Public Methods

        public virtual ITradeClient Subscribe(string symbol, Action<TradeEventArgs> callback)
            => (ITradeClient)HandleSubscribe(() => Client.Subscribe(symbol, callback));

        public virtual ITradeClient Unsubscribe(string symbol, Action<TradeEventArgs> callback)
            => (ITradeClient)HandleUnsubscribe(() => Client.Unsubscribe(symbol, callback));

        #endregion Public Methods
    }
}
