using System;
using System.Threading;
using System.Threading.Tasks;
using Binance.Api.WebSocket.Events;

namespace Binance.Api.WebSocket
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
        /// Get flag indicating if using combined streams.
        /// </summary>
        bool IsCombined { get; }

        /// <summary>
        /// Subscribe a callback to a stream.
        /// </summary>
        /// <param name="stream">The stream name.</param>
        /// <param name="callback">The callback.</param>
        void Subscribe(string stream, Action<WebSocketStreamEventArgs> callback);

        /// <summary>
        /// Unsubscribe a callback from a stream.
        /// </summary>
        /// <param name="stream">The stream name.</param>
        /// <param name="callback">The callback.</param>
        void Unsubscribe(string stream, Action<WebSocketStreamEventArgs> callback);

        /// <summary>
        /// Initiate a web socket connection and begin receiving messages (streaming).
        /// </summary>
        /// <param name="token">The cancellation token (required to cancel operation).</param>
        /// <returns></returns>
        Task StreamAsync(CancellationToken token);
    }
}
