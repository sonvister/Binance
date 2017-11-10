using System;
using System.Threading;
using System.Threading.Tasks;
using Binance.Api.WebSocket.Events;

namespace Binance.Api.WebSocket
{
    /// <summary>
    /// A web socket implementation facade for <see cref="BinanceWebSocketClient"/>.
    /// </summary>
    public interface IWebSocketClient : IDisposable
    {
        /// <summary>
        /// The message received event.
        /// </summary>
        event EventHandler<WebSocketClientMessageEventArgs> Message;

        /// <summary>
        /// Connect web socket to URI and begin receiving messages.
        /// </summary>
        /// <param name="uri">The URI.</param>
        /// <param name="token">The cancellation token (required to abort operation).</param>
        /// <returns></returns>
        Task OpenAsync(Uri uri, CancellationToken token);
    }
}
