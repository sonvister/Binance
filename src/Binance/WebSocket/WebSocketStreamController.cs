using System;
using Binance.Manager;

namespace Binance.WebSocket
{
    public sealed class WebSocketStreamController : JsonStreamController<IWebSocketStream>, IWebSocketStreamController
    {
        /// <summary>
        /// Contstructor.
        /// </summary>
        /// <param name="webSocket"></param>
        /// <param name="onError"></param>
        public WebSocketStreamController(IWebSocketStream webSocket, Action<Exception> onError = null)
            : base(webSocket, onError)
        { }
    }
}
