using System;

namespace Binance.WebSocket
{
    public interface IBinanceWebSocketClient
    {
        /// <summary>
        /// The web socket client open event.
        /// </summary>
        event EventHandler<EventArgs> Open;

        /// <summary>
        /// The web socket client close event.
        /// </summary>
        event EventHandler<EventArgs> Close;

        /// <summary>
        /// The web socket stream.
        /// </summary>
        IWebSocketStream WebSocket { get; }
    }
}
