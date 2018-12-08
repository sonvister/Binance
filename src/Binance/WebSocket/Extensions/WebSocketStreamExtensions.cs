using System.Threading;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace Binance.WebSocket
{
    public static class WebSocketStreamExtensions
    {
        /// <summary>
        /// Get flag indicating if using combined streams.
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public static bool IsCombined(this IWebSocketStream stream)
        {
            return stream is BinanceWebSocketStream binanceWebSocketStream
                && binanceWebSocketStream.IsCombined;
        }

        /// <summary>
        /// Wait until web socket is open.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task WaitUntilWebSocketOpenAsync(this IWebSocketStream stream, CancellationToken token = default)
            => stream.WebSocket.WaitUntilOpenAsync(token);
    }
}
