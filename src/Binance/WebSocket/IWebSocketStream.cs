using System;
using Binance.Producer;

namespace Binance.WebSocket
{
    /// <summary>
    /// A web socket JSON stream implementation.
    /// </summary>
    public interface IWebSocketStream : IBufferedJsonStream<IWebSocketClient>
    {
        /// <summary>
        /// Get or set the URI.
        /// </summary>
        Uri Uri { get; set; }

        /// <summary>
        /// The low-level web socket client.
        /// </summary>
        IWebSocketClient WebSocket { get; }
    }
}
