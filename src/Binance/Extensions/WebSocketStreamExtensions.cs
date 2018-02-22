// ReSharper disable once CheckNamespace
namespace Binance.WebSocket
{
    public static class WebSocketStreamExtensions
    {
        /// <summary>
        /// Get flag indicating if using combined streams.
        /// </summary>
        /// <param name="webSocket"></param>
        /// <returns></returns>
        public static bool IsCombined(this IWebSocketStream webSocket)
        {
            return webSocket is BinanceWebSocketStream binanceWebSocketStream
                && binanceWebSocketStream.IsCombined;
        }
    }
}
