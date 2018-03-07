using System;
using Binance.Client;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace Binance.WebSocket
{
    /// <summary>
    /// The default <see cref="ICandlestickWebSocketClient"/> implementation.
    /// </summary>
    public class CandlestickWebSocketClient : AutoBinanceWebSocketClient<IWebSocketStream, ICandlestickClient, CandlestickEventArgs>, ICandlestickWebSocketClient
    {
        #region Public Events

        public event EventHandler<CandlestickEventArgs> Candlestick
        {
            add => Client.Candlestick += value;
            remove => Client.Candlestick -= value;
        }

        public event EventHandler<ErrorEventArgs> Error
        {
            add => Publisher.Controller.Error += value;
            remove => Publisher.Controller.Error -= value;
        }

        #endregion Public Events

        #region Constructors

        /// <summary>
        /// Default constructor provides default <see cref="ICandlestickClient"/>
        /// and default <see cref="IBinanceWebSocketStreamPublisher"/>, but no logger.
        /// </summary>
        public CandlestickWebSocketClient()
            : this(new CandlestickClient(), new BinanceWebSocketStreamPublisher())
        { }

        /// <summary>
        /// The DI constructor.
        /// </summary>
        /// <param name="client">The JSON client (required).</param>
        /// <param name="publisher">The web socket stream publisher (required).</param>
        /// <param name="logger">The logger (optional).</param>
        public CandlestickWebSocketClient(ICandlestickClient client, IBinanceWebSocketStreamPublisher publisher, ILogger<CandlestickWebSocketClient> logger = null)
            : base(client, publisher, logger)
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
