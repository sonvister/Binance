using System;
using System.Threading;
using System.Threading.Tasks;
using Binance.Api.WebSocket.Events;

// ReSharper disable once CheckNamespace
namespace Binance.Api.WebSocket
{
    public static class TradesWebSocketClientExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public static void Subscribe(this ITradeWebSocketClient client, string symbol)
            => client.Subscribe(symbol, null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="symbol"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task StreamAsync(this ITradeWebSocketClient client, string symbol, CancellationToken token)
            => StreamAsync(client, symbol, null, token);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="symbol"></param>
        /// <param name="callback"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task StreamAsync(this ITradeWebSocketClient client, string symbol, Action<TradeEventArgs> callback, CancellationToken token)
        {
            Throw.IfNull(client, nameof(client));

            client.Subscribe(symbol, callback);

            return client.WebSocket.StreamAsync(token);
        }
    }
}
