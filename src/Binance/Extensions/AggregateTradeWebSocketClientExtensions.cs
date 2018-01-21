using System;
using System.Threading;
using System.Threading.Tasks;
using Binance.WebSocket.Events;

// ReSharper disable once CheckNamespace
namespace Binance.WebSocket
{
    public static class AggregateTradeWebSocketClientExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="symbol"></param>
        public static void Subscribe(this IAggregateTradeWebSocketClient client, string symbol)
            => client.Subscribe(symbol, null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="symbol"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task StreamAsync(this IAggregateTradeWebSocketClient client, string symbol, CancellationToken token)
            => StreamAsync(client, symbol, null, token);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="symbol"></param>
        /// <param name="callback"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task StreamAsync(this IAggregateTradeWebSocketClient client, string symbol, Action<AggregateTradeEventArgs> callback, CancellationToken token)
        {
            Throw.IfNull(client, nameof(client));

            client.Subscribe(symbol, callback);

            return client.WebSocket.StreamAsync(token);
        }
    }
}
