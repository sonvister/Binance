using System;
using Binance.Client;

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
        public static void Subscribe<TClient>(this IOrderBookCache<TClient> cache, string symbol)
            where TClient : IDepthClient
            => cache.Subscribe(symbol, default, null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="symbol"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public static void Subscribe<TClient>(this IOrderBookCache<TClient> cache, string symbol, int limit)
            where TClient : IDepthClient
            => cache.Subscribe(symbol, limit, null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="symbol"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static void Subscribe<TClient>(this IOrderBookCache<TClient> cache, string symbol, Action<OrderBookCacheEventArgs> callback)
            where TClient : IDepthClient
            => cache.Subscribe(symbol, default, callback);
    }
}
