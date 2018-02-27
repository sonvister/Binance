using System.Threading;
using System.Threading.Tasks;
using Binance.WebSocket.Manager;

// ReSharper disable once CheckNamespace
namespace Binance.WebSocket
{
    public static class WebSocketControllerManagerExtensions
    {
        /// <summary>
        /// Wait until web socket is open.
        /// </summary>
        /// <param name="manager"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task WaitUntilWebSocketOpenAsync(this IWebSocketControllerManager manager, CancellationToken token = default)
            => manager.Controller.Stream.WebSocket.WaitUntilOpenAsync(token);
    }
}
