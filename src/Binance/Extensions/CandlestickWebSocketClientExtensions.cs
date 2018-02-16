using System.Threading;
using System.Threading.Tasks;
using Binance.Market;

// ReSharper disable once CheckNamespace
namespace Binance.WebSocket
{
    public static class CandlestickWebSocketClientExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="symbol"></param>
        /// <param name="interval"></param>
        public static void Subscribe(this ICandlestickWebSocketClient client, string symbol, CandlestickInterval interval)
            => client.Subscribe(symbol, interval, null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="symbol"></param>
        /// <param name="interval"></param>
        public static void Unsubscribe(this ICandlestickWebSocketClient client, string symbol, CandlestickInterval interval)
            => client.Unsubscribe(symbol, interval, null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task StreamAsync(this ICandlestickWebSocketClient client, CancellationToken token)
        {
            Throw.IfNull(client, nameof(client));

            return client.Stream.StreamAsync(token);
        }
    }
}
