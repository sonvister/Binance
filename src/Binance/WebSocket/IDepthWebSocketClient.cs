using System;
using Binance.WebSocket.Events;

namespace Binance.WebSocket
{
    /// <summary>
    /// The depth client ...what makes order book synchronization possible.
    /// </summary>
    public interface IDepthWebSocketClient : IBinanceWebSocketClient
    {
        /// <summary>
        /// The depth update event.
        /// </summary>
        event EventHandler<DepthUpdateEventArgs> DepthUpdate;

        /// <summary>
        /// Subscribe to the specified symbol (for use with combined streams).
        /// Call <see cref="IWebSocketStream"/> StreamAsync to begin streaming.
        /// </summary>
        /// <param name="symbol">The symbol to subscribe.</param>
        /// <param name="limit">The limit (optional, uses partial depth stream). Valid values are: 5, 10, or 20.</param>
        /// <param name="callback">An event callback.</param>
        void Subscribe(string symbol, int limit, Action<DepthUpdateEventArgs> callback);

        /// <summary>
        /// Unsubscribe a callback from symbol events. If no callback is
        /// specified, then unsubscribe symbol (all callbacks).
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="limit">The limit (optional, uses partial depth stream). Valid values are: 5, 10, or 20.</param>
        /// <param name="callback"></param>
        void Unsubscribe(string symbol, int limit, Action<DepthUpdateEventArgs> callback);
    }
}
