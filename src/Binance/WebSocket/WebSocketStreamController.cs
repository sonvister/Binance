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
        public WebSocketStreamController(IWebSocketStream stream)
            : base(stream)
        { }
    }
}
