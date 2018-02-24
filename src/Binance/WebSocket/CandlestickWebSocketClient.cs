using System;
using Binance.Client;
using Binance.Client.Events;
using Binance.Market;
using Microsoft.Extensions.Logging;

namespace Binance.WebSocket
{
    /// <summary>
    /// The default <see cref="ICandlestickWebSocketClient"/> implementation.
    /// </summary>
    public class CandlestickWebSocketClient : BinanceWebSocketClient<ICandlestickClient, CandlestickEventArgs>, ICandlestickWebSocketClient
    {
        #region Public Events

        public event EventHandler<CandlestickEventArgs> Candlestick
        {
            add => Client.Candlestick += value;
            remove => Client.Candlestick -= value;
        }

        #endregion Public Events

        #region Constructors

        /// <summary>
        /// Default constructor provides default <see cref="ICandlestickClient"/>
        /// and default <see cref="IBinanceWebSocketStream"/>, but no logger.
        /// </summary>
        public CandlestickWebSocketClient()
            : this(new CandlestickClient(), new BinanceWebSocketStream())
        { }

        /// <summary>
        /// The DI constructor.
        /// </summary>
        /// <param name="client">The JSON client (required).</param>
        /// <param name="stream">The web socket stream (required).</param>
        /// <param name="logger">The logger (optional).</param>
        public CandlestickWebSocketClient(ICandlestickClient client, IBinanceWebSocketStream stream, ILogger<CandlestickWebSocketClient> logger = null)
            : base(client, stream, logger)
        { }

        #endregion Construtors

        #region Public Methods

        public virtual ICandlestickClient Subscribe(string symbol, CandlestickInterval interval, Action<CandlestickEventArgs> callback)
            => (ICandlestickClient)HandleSubscribe(() => Client.Subscribe(symbol, interval, callback));

        public virtual ICandlestickClient Unsubscribe(string symbol, CandlestickInterval interval, Action<CandlestickEventArgs> callback)
            => (ICandlestickClient)HandleUnsubscribe(() => Client.Unsubscribe(symbol, interval, callback));

        #endregion Public Methods
    }
}
