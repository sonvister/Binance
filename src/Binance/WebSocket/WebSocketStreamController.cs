using Binance.Manager;
using Microsoft.Extensions.Logging;

namespace Binance.WebSocket
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
