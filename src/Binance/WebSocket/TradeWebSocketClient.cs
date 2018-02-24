using System;
using Binance.Client;
using Binance.Client.Events;
using Microsoft.Extensions.Logging;

namespace Binance.WebSocket
{
    /// <summary>
    /// The default <see cref="ITradeWebSocketClient"/> implementation.
    /// </summary>
    public class TradeWebSocketClient : BinanceWebSocketClient<ITradeClient, TradeEventArgs>, ITradeWebSocketClient
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
        /// Default constructor provides default <see cref="ITradeClient"/>
        /// and default <see cref="IBinanceWebSocketStream"/>, but no logger.
        /// </summary>
        public TradeWebSocketClient()
            : this(new TradeClient(), new BinanceWebSocketStream())
        { }

        /// <summary>
        /// The DI constructor.
        /// </summary>
        /// <param name="client">The JSON client (required).</param>
        /// <param name="stream">The web socket stream (required).</param>
        /// <param name="logger">The logger (optional).</param>
        public TradeWebSocketClient(ITradeClient client, IBinanceWebSocketStream stream, ILogger<TradeWebSocketClient> logger = null)
            : base(client, stream, logger)
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
