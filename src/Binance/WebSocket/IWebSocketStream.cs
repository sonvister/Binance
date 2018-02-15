using Binance.Stream;

namespace Binance.WebSocket
{
    /// <summary>
    /// A web socket JSON stream implementation.
    /// </summary>
    public interface IWebSocketStream : IJsonStream
    {
        /// <summary>
        /// The low-level web socket client.
        /// </summary>
        IWebSocketClient WebSocket { get; }
    }
}
