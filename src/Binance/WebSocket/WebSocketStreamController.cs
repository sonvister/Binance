using System;
using Binance.Manager;

namespace Binance.WebSocket
{
    public abstract class WebSocketStreamController : JsonStreamController<IWebSocketStream>, IWebSocketStreamController
    {
        /// <summary>
        /// Contstructor.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="onError"></param>
        public WebSocketStreamController(IWebSocketStream stream, Action<Exception> onError = null)
            : base(stream, onError)
        { }
    }
}
