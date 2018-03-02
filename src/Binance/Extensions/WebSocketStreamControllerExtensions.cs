using System.Threading;
using System.Threading.Tasks;
using Binance.Utility;

// ReSharper disable once CheckNamespace
namespace Binance.WebSocket
{
    public static class WebSocketStreamControllerExtensions
    {
        /// <summary>
        /// Wait until web socket is open.
        /// </summary>
        /// <param name="controller"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task WaitUntilWebSocketOpenAsync(this IWebSocketStreamController controller, CancellationToken token = default)
            => controller.Stream.WebSocket.WaitUntilOpenAsync(token);
    }
}
