using System;
using System.Threading;
using System.Threading.Tasks;

namespace Binance.Api.WebSocket
{
    public interface IWebSocketClient : IDisposable
    {
        /// <summary>
        /// Connect web socket, receive messages, and post to callback.
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="onMessage"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task OpenAsync(Uri uri, Action<string> onMessage, CancellationToken token);
    }
}
