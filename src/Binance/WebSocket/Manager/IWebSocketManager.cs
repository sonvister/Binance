using System;

namespace Binance.WebSocket.Manager
{
    public interface IWebSocketManager
    {
        /// <summary>
        /// The error event. Raised when exceptions occur from client task
        /// controller actions or from client adapter subscribe/unsubscribe
        /// (async) operations (if applicable).
        /// </summary>
        event EventHandler<WebSocketManagerErrorEventArgs> Error;
    }
}
