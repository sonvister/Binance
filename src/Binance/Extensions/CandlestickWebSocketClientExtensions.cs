using System.Threading;
using System.Threading.Tasks;
using Binance.Market;

// ReSharper disable once CheckNamespace
namespace Binance.Api.WebSocket
{
    public static class CandlestickWebSocketClientExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="symbol"></param>
        /// <param name="interval"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task SubscribeAsync(this ICandlestickWebSocketClient client, string symbol, CandlestickInterval interval, CancellationToken token)
            => client.SubscribeAsync(symbol, interval, null, token);
    }
}
