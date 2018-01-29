using System;
using System.Threading;
using System.Threading.Tasks;
using Binance.Cache.Events;

// ReSharper disable once CheckNamespace
namespace Binance.Cache
{
    public static class OrderBookCacheExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public static void Subscribe(this IOrderBookCache cache, string symbol)
            => cache.Subscribe(symbol, default, null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="symbol"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public static void Subscribe(this IOrderBookCache cache, string symbol, int limit)
            => cache.Subscribe(symbol, limit, null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="symbol"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static void Subscribe(this IOrderBookCache cache, string symbol, Action<OrderBookCacheEventArgs> callback)
            => cache.Subscribe(symbol, default, callback);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task StreamAsync(this IOrderBookCache cache, CancellationToken token)
        {
            Throw.IfNull(cache, nameof(cache));

            return cache.Client.WebSocket.StreamAsync(token);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="symbol"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task SubscribeAndStreamAsync(this IOrderBookCache cache, string symbol, CancellationToken token)
            => SubscribeAndStreamAsync(cache, symbol, default, null, token);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="symbol"></param>
        /// <param name="limit"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task SubscribeAndStreamAsync(this IOrderBookCache cache, string symbol, int limit, CancellationToken token)
            => SubscribeAndStreamAsync(cache, symbol, limit, null, token);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="symbol"></param>
        /// <param name="callback"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task SubscribeAndStreamAsync(this IOrderBookCache cache, string symbol, Action<OrderBookCacheEventArgs> callback, CancellationToken token)
            => SubscribeAndStreamAsync(cache, symbol, default, callback, token);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="symbol"></param>
        /// <param name="limit"></param>
        /// <param name="callback"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task SubscribeAndStreamAsync(this IOrderBookCache cache, string symbol, int limit, Action<OrderBookCacheEventArgs> callback, CancellationToken token)
        {
            Throw.IfNull(cache, nameof(cache));

            cache.Subscribe(symbol, limit, callback);

            return StreamAsync(cache, token);
        }
    }
}
