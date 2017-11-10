using System.Threading;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace Binance.Api.WebSocket
{
    public static class DepthWebSocketClientExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="symbol"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task SubscribeAsync(this IDepthWebSocketClient client, string symbol, CancellationToken token)
            => client.SubscribeAsync(symbol, null, token);
    }
}
