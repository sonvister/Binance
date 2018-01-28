using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Binance.WebSocket.Events;

namespace Binance.WebSocket
{
    /// <summary>
    /// WebSocket layer to add support for combined streams.
    /// </summary>
    public interface IWebSocketStream
    {
        /// <summary>
        /// The low-level web socket client.
        /// </summary>
        IWebSocketClient Client { get; }

        /// <summary>
        /// Get the subscribed streams.
        /// </summary>
        IEnumerable<string> SubscribedStreams { get; }

        /// <summary>
        /// Get flag indicating if using combined streams.
        /// </summary>
        bool IsCombined { get; }

        /// <summary>
        /// Subscribe a callback to a stream.
        /// This can be done while streaming, subscribing a new stream does not
        /// take effect until the stream operation is cancelled and restarted.
        /// </summary>
        /// <param name="stream">The stream name.</param>
        /// <param name="callback">The callback.</param>
        void Subscribe(string stream, Action<WebSocketStreamEventArgs> callback);

        /// <summary>
        /// Unsubscribe a callback from a stream.
        /// This can be done while streaming, unsubscribing a stream does not
        /// take effect until the stream operation is cancelled and restarted.
        /// </summary>
        /// <param name="stream">The stream name.</param>
        /// <param name="callback">The callback.</param>
        void Unsubscribe(string stream, Action<WebSocketStreamEventArgs> callback);

        /// <summary>
        /// Initiate a web socket connection and begin receiving messages.
        /// Runtime exceptions are thrown by this method and must be handled
        /// by the caller, otherwise the <see cref="Task"/> continues receiving
        /// and processing messages until the token is canceled.
        /// Open/Close events are provided by <see cref="IWebSocketClient"/>.
        /// </summary>
        /// <param name="token">The cancellation token (required to cancel operation).</param>
        /// <returns></returns>
        Task StreamAsync(CancellationToken token);
    }
}
