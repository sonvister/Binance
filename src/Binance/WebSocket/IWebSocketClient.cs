using System;
using System.Threading;
using System.Threading.Tasks;
using Binance.WebSocket.Events;

namespace Binance.WebSocket
{
    /// <summary>
    /// A web socket client interface for <see cref="IWebSocketStream"/>.
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
        event EventHandler<WebSocketClientEventArgs> Message;

        /// <summary>
        /// The close event.
        /// </summary>
        event EventHandler<EventArgs> Close;

        /// <summary>
        /// Get the flag indicating if the client is streaming.
        /// </summary>
        bool IsStreaming { get; }

        /// <summary>
        /// Connect web socket to URI and begin receiving messages.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="token">The cancellation token (required to cancel operation).</param>
        /// <returns></returns>
        Task StreamAsync(Uri uri, CancellationToken token);
    }
}
