using System;
using System.Threading;
using System.Threading.Tasks;
using Binance.Api.WebSocket.Events;
using Binance.Market;

namespace Binance.Api.WebSocket
{
    public interface IKlineWebSocketClient : IDisposable
    {
        #region Public Events

        /// <summary>
        /// The kline event.
        /// </summary>
        event EventHandler<KlineEventArgs> Kline;

        #endregion Public Events

        #region Public Methods

        /// <summary>
        /// Subscribe to the specified symbol and begin receiving kline
        /// events. Awaits on this method will not return until the token is
        /// canceled, this <see cref="IKlineWebSocketClient"/> is disposed,
        /// or an exception occurs.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <param name="interval">The interval.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns><see cref="Task"/></returns>
        Task SubscribeAsync(string symbol, KlineInterval interval, CancellationToken token = default);

        /// <summary>
        /// Subscribe to the specified symbol and begin receiving kline
        /// events. Awaits on this method will not return until the token is
        /// canceled, this <see cref="IKlineWebSocketClient"/> is disposed,
        /// or an exception occurs.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <param name="interval">The interval.</param>
        /// <param name="callback">An event callback.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns><see cref="Task"/></returns>
        Task SubscribeAsync(string symbol, KlineInterval interval, Action<KlineEventArgs> callback, CancellationToken token = default);

        #endregion Public Methods
    }
}
