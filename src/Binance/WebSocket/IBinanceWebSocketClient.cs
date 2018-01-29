using System;

namespace Binance.WebSocket
{
    public interface IBinanceWebSocketClient
    {
        /// <summary>
        /// The open event.
        /// </summary>
        event EventHandler<EventArgs> Open;

        /// <summary>
        /// The close event.
        /// </summary>
        event EventHandler<EventArgs> Close;

        /// <summary>
        /// The web socket stream.
        /// </summary>
        IWebSocketStream WebSocket { get; }
    }
}
