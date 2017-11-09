using System;
using System.Threading;
using System.Threading.Tasks;
using Binance.Api.WebSocket.Events;

namespace Binance.Api.WebSocket
{
    public interface IWebSocketClient : IDisposable
    {
        /// <summary>
        /// The message event.
        /// </summary>
        event EventHandler<WebSocketClientMessageEventArgs> Message;

        /// <summary>
        /// Connect web socket and receive messages.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task OpenAsync(Uri uri, CancellationToken token);
    }
}
