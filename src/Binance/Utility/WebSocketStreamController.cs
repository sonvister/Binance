using Binance.WebSocket;
using Microsoft.Extensions.Logging;

namespace Binance.Utility
{
    public abstract class WebSocketStreamController : JsonStreamController<IWebSocketStream>, IWebSocketStreamController
    {
        /// <summary>
        /// Contstructor.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="logger"></param>
        public WebSocketStreamController(IWebSocketStream stream, ILogger<WebSocketStreamController> logger = null)
            : base(stream, logger)
        { }
    }
}
