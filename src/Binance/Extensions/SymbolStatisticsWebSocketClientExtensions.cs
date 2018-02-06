using System;
using System.Threading;
using System.Threading.Tasks;
using Binance.WebSocket.Events;

// ReSharper disable once CheckNamespace
namespace Binance.WebSocket
{
    public static class SymbolStatisticsWebSocketClientExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static void Subscribe(this ISymbolStatisticsWebSocketClient client)
            => client.Subscribe(null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public static void Subscribe(this ISymbolStatisticsWebSocketClient client, string symbol)
            => client.Subscribe(symbol, null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        public static void Unsubscribe(this ISymbolStatisticsWebSocketClient client)
            => client.Unsubscribe(null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="symbol"></param>
        public static void Unsubscribe(this ISymbolStatisticsWebSocketClient client, string symbol)
            => client.Unsubscribe(symbol, null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task StreamAsync(this ISymbolStatisticsWebSocketClient client, CancellationToken token)
        {
            Throw.IfNull(client, nameof(client));

            return client.WebSocket.StreamAsync(token);
        }
    }
}
