using Binance.WebSocket;
using Microsoft.Extensions.Logging;

namespace Binance.Utility
{
    public class WebSocketStreamController : WebSocketStreamController<IWebSocketStream>, IWebSocketStreamController
    {
        public WebSocketStreamController(IWebSocketStream stream, ILogger<WebSocketStreamController> logger = null)
            : base(stream, logger)
        { }
    }

    public abstract class WebSocketStreamController<TStream> : JsonStreamController<TStream>, IWebSocketStreamController<TStream>
        where TStream : IWebSocketStream
    {
        /// <summary>
        /// Contstructor.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="logger"></param>
        protected WebSocketStreamController(TStream stream, ILogger<WebSocketStreamController<TStream>> logger = null)
            : base(stream, logger)
        { }
    }
}
