using System;
using System.Threading;
using System.Threading.Tasks;
using Binance.Api.WebSocket.Events;
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
        public static void Subscribe(this ICandlestickWebSocketClient client, string symbol, CandlestickInterval interval)
            => client.Subscribe(symbol, interval, null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="symbol"></param>
        /// <param name="interval"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task StreamAsync(this ICandlestickWebSocketClient client, string symbol, CandlestickInterval interval, CancellationToken token)
            => StreamAsync(client, symbol, interval, null, token);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="symbol"></param>
        /// <param name="interval"></param>
        /// <param name="callback"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task StreamAsync(this ICandlestickWebSocketClient client, string symbol, CandlestickInterval interval, Action<CandlestickEventArgs> callback, CancellationToken token)
        {
            Throw.IfNull(client, nameof(client));

            client.Subscribe(symbol, interval, callback);

            return client.WebSocket.StreamAsync(token);
        }
    }
}
