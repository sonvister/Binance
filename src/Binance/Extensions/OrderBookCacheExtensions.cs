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
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task SubscribeAsync(this IOrderBookCache cache, string symbol, CancellationToken token)
            => cache.SubscribeAsync(symbol, default, null, token);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="symbol"></param>
        /// <param name="limit"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task SubscribeAsync(this IOrderBookCache cache, string symbol, int limit, CancellationToken token)
            => cache.SubscribeAsync(symbol, limit, null, token);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="symbol"></param>
        /// <param name="callback"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task SubscribeAsync(this IOrderBookCache cache, string symbol, Action<OrderBookCacheEventArgs> callback, CancellationToken token)
            => cache.SubscribeAsync(symbol, default, callback, token);
    }
}
