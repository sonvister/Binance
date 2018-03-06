using System.Threading;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace Binance.WebSocket
{
    public static class WebSocketPublisherClientExtensions
    {
        /// <summary>
        /// Wait until web socket is open.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task WaitUntilWebSocketOpenAsync(this IWebSocketPublisherClient client, CancellationToken token = default)
            => client.Publisher.Controller.Stream.WaitUntilWebSocketOpenAsync(token);
    }
}
