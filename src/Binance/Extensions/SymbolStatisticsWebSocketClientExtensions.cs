using System.Threading;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace Binance.WebSocket
{
    public static class SymbolStatisticsWebSocketClientExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task StreamAsync(this ISymbolStatisticsWebSocketClient client, CancellationToken token)
        {
            Throw.IfNull(client, nameof(client));

            return client.Stream.StreamAsync(token);
        }
    }
}
