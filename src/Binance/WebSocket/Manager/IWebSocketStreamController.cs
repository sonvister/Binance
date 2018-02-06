using Binance.Utility;

namespace Binance.WebSocket.Manager
{
    public interface IWebSocketStreamController : ITaskController
    {
        /// <summary>
        /// Get the web socket stream.
        /// </summary>
        IWebSocketStream WebSocket { get; }
    }
}
