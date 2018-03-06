using System;
using System.Threading;
using System.Threading.Tasks;
using Binance.Producer;

namespace Binance.WebSocket
{
    /// <summary>
    /// A low-level web socket client JSON provider interface.
    /// </summary>
    public interface IWebSocketClient : IJsonProducer
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
        /// Get the flag indicating if the client is streaming.
        /// </summary>
        bool IsStreaming { get; }

        /// <summary>
        /// Get the flag indicating if the client is connected (open).
        /// </summary>
        bool IsOpen { get; }

        /// <summary>
        /// Connect web socket to URI and begin receiving messages.
        /// Runtime exceptions are thrown by this method and must be handled
        /// by the caller, otherwise the <see cref="Task"/> continues receiving
        /// and processing messages until the token is canceled.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="token">The cancellation token (required to cancel operation).</param>
        /// <returns></returns>
        Task StreamAsync(Uri uri, CancellationToken token);
    }
}
