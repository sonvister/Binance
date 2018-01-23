using System;
using Binance.WebSocket.Events;
using Binance.Market;

namespace Binance.WebSocket
{
    public interface ICandlestickWebSocketClient : IBinanceWebSocketClient
    {
        #region Events

        /// <summary>
        /// The candlestick event.
        /// </summary>
        event EventHandler<CandlestickEventArgs> Candlestick;

        #endregion Events

        #region Methods

        /// <summary>
        /// Subscribe to the specified symbol (for use with combined streams).
        /// Call <see cref="IWebSocketStream"/> StreamAsync to begin streaming.
        /// </summary>
        /// <param name="symbol">The symbol to subscribe.</param>
        /// <param name="interval">The interval.</param>
        /// <param name="callback">An event callback.</param>
        void Subscribe(string symbol, CandlestickInterval interval, Action<CandlestickEventArgs> callback);

        #endregion Methods
    }
}
