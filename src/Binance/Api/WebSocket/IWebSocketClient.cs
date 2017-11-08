using System;
using System.Threading;
using System.Threading.Tasks;

namespace Binance.Api.WebSocket
{
    public interface IWebSocketClient : IDisposable
    {
        /// <summary>
        /// Connect web socket.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task ConnectAsync(Uri uri, CancellationToken token);

        /// <summary>
        /// Receive messages and post to callback.
        /// </summary>
        /// <param name="onMessage"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task ReceiveAsync(Action<string> onMessage, CancellationToken token);
    }
}
