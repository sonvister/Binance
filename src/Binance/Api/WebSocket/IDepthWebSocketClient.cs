using System;
using System.Threading;
using System.Threading.Tasks;
using Binance.Api.WebSocket.Events;

namespace Binance.Api.WebSocket
{
    /// <summary>
    /// The depth client ...what makes order book synchronization possible.
    /// </summary>
    public interface IDepthWebSocketClient : IBinanceWebSocketClient
    {
        #region Public Events

        /// <summary>
        /// The depth update event.
        /// </summary>
        event EventHandler<DepthUpdateEventArgs> DepthUpdate;

        #endregion Public Events

        #region Public Methods

        /// <summary>
        /// Subscribe to the specified symbol and begin receiving depth update
        /// events. Awaits on this method will not return until the token is
        /// canceled, this <see cref="IDepthWebSocketClient"/> is disposed, or
        /// an exception occurs.
        /// </summary>
        /// <param name="symbol">The symbol.</param>
        /// <param name="limit">The limit (optional, uses partial depth stream). Valid values are: 5, 10, or 20.</param>
        /// <param name="callback">An event callback.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns><see cref="Task"/></returns>
        Task SubscribeAsync(string symbol, int limit, Action<DepthUpdateEventArgs> callback, CancellationToken token);

        #endregion Public Methods
    }
}
