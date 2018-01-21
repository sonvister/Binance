using System;
using Binance.WebSocket.Events;

namespace Binance.WebSocket
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
        /// Subscribe to the specified symbol (for use with combined streams).
        /// Call <see cref="IWebSocketStream"/> StreamAsync to begin streaming.
        /// </summary>
        /// <param name="symbol">The symbol to subscribe.</param>
        /// <param name="limit">The limit (optional, uses partial depth stream). Valid values are: 5, 10, or 20.</param>
        /// <param name="callback">An event callback.</param>
        void Subscribe(string symbol, int limit, Action<DepthUpdateEventArgs> callback);

        #endregion Public Methods
    }
}
