using System;
using System.Threading;
using System.Threading.Tasks;
using Binance.Api.WebSocket.Events;

namespace Binance.Api.WebSocket
{
    /// <summary>
    /// A web socket client interface for BinanceWebSocketClient.
    /// </summary>
    public interface IWebSocketClient
    {
        /// <summary>
        /// The open event.
        /// </summary>
        event EventHandler<EventArgs> Open;

        /// <summary>
        /// The message received event.
        /// </summary>
        event EventHandler<WebSocketClientMessageEventArgs> Message;

        /// <summary>
        /// The close event.
        /// </summary>
        event EventHandler<EventArgs> Close;

        /// <summary>
        /// Connect web socket to URI and begin receiving messages.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="token">The cancellation token (required to cancel operation).</param>
        /// <returns></returns>
        Task RunAsync(Uri uri, CancellationToken token);
    }
}
