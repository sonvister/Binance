using System;
using System.Threading;
using System.Threading.Tasks;
using Binance.Api.WebSocket.Events;

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
            => client.SubscribeAsync(symbol, default, null, token);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="symbol"></param>
        /// <param name="limit"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task SubscribeAsync(this IDepthWebSocketClient cache, string symbol, int limit, CancellationToken token)
            => cache.SubscribeAsync(symbol, limit, null, token);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="symbol"></param>
        /// <param name="callback"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task SubscribeAsync(this IDepthWebSocketClient cache, string symbol, Action<DepthUpdateEventArgs> callback, CancellationToken token)
            => cache.SubscribeAsync(symbol, default, callback, token);
    }
}
