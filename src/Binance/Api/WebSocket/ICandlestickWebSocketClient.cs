using System;
using System.Threading;
using System.Threading.Tasks;
using Binance.Api.WebSocket.Events;
using Binance.Market;

namespace Binance.Api.WebSocket
{
    public interface ICandlestickWebSocketClient : IDisposable
    {
        #region Public Events

        /// <summary>
        /// The candlestick event.
        /// </summary>
        event EventHandler<CandlestickEventArgs> Candlestick;

        #endregion Public Events

        #region Public Methods

        /// <summary>
        /// Subscribe to the specified symbol and begin receiving candlestick
        /// events. Awaits on this method will not return until the token is
        /// canceled, this <see cref="ICandlestickWebSocketClient"/> is disposed,
        /// or an exception occurs.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <param name="interval">The interval.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns><see cref="Task"/></returns>
        Task SubscribeAsync(string symbol, CandlestickInterval interval, CancellationToken token);

        /// <summary>
        /// Subscribe to the specified symbol and begin receiving candlestick
        /// events. Awaits on this method will not return until the token is
        /// canceled, this <see cref="ICandlestickWebSocketClient"/> is disposed,
        /// or an exception occurs.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <param name="interval">The interval.</param>
        /// <param name="callback">An event callback.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns><see cref="Task"/></returns>
        Task SubscribeAsync(string symbol, CandlestickInterval interval, Action<CandlestickEventArgs> callback, CancellationToken token);

        #endregion Public Methods
    }
}
