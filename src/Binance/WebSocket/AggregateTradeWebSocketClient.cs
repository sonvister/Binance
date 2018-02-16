using System;
using Binance.Client;
using Binance.Client.Events;
using Microsoft.Extensions.Logging;

namespace Binance.WebSocket
{
    /// <summary>
    /// The default <see cref="IAggregateTradeWebSocketClient"/> implementation.
    /// </summary>
    public class AggregateTradeWebSocketClient : BinanceWebSocketClient<IAggregateTradeClient, AggregateTradeEventArgs>, IAggregateTradeWebSocketClient
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
        /// Default constructor provides default <see cref="IAggregateTradeClient"/>
        /// and default <see cref="IBinanceWebSocketStream"/>, but no logger.
        /// </summary>
        public AggregateTradeWebSocketClient()
            : this(new AggregateTradeClient(), new BinanceWebSocketStream())
        { }

        /// <summary>
        /// The DI constructor.
        /// </summary>
        /// <param name="client">The JSON client (required).</param>
        /// <param name="stream">The web socket stream (required).</param>
        /// <param name="logger">The logger (optional).</param>
        public AggregateTradeWebSocketClient(IAggregateTradeClient client, IBinanceWebSocketStream stream, ILogger<AggregateTradeWebSocketClient> logger = null)
            : base(client, stream, logger)
        { }

        #endregion Construtors

        #region Public Methods

        public virtual void Subscribe(string symbol, Action<AggregateTradeEventArgs> callback)
            => HandleSubscribe(() => Client.Subscribe(symbol, callback));

        public virtual void Unsubscribe(string symbol, Action<AggregateTradeEventArgs> callback)
            => HandleUnsubscribe(() => Client.Unsubscribe(symbol, callback));

        #endregion Public Methods
    }
}
