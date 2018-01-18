using System;
using Binance.Api.WebSocket.Events;
using Binance.Market;

namespace Binance.Api.WebSocket
{
    public interface ICandlestickWebSocketClient : IBinanceWebSocketClient
    {
        #region Public Events

        /// <summary>
        /// The candlestick event.
        /// </summary>
        event EventHandler<CandlestickEventArgs> Candlestick;

        #endregion Public Events

        #region Public Methods

        /// <summary>
        /// Subscribe to the specified symbol (for use with combined streams).
        /// Call <see cref="IWebSocketStream"/> StreamAsync to begin streaming.
        /// </summary>
        /// <param name="symbol">The symbol to subscribe.</param>
        /// <param name="interval">The interval.</param>
        /// <param name="callback">An event callback.</param>
        void Subscribe(string symbol, CandlestickInterval interval, Action<CandlestickEventArgs> callback);

        #endregion Public Methods
    }
}
