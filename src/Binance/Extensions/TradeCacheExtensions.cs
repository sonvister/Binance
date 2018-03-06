using System;
using Binance.Cache.Events;
using Binance.Client;

// ReSharper disable once CheckNamespace
namespace Binance.Cache
{
    public static class TradeCacheExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public static void Subscribe<TClient>(this ITradeCache<TClient> cache, string symbol)
            where TClient : ITradeClient
            => cache.Subscribe(symbol, default, null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="symbol"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public static void Subscribe<TClient>(this ITradeCache<TClient> cache, string symbol, int limit)
            where TClient : ITradeClient
            => cache.Subscribe(symbol, limit, null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="symbol"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static void Subscribe<TClient>(this ITradeCache<TClient> cache, string symbol, Action<TradeCacheEventArgs> callback)
            where TClient : ITradeClient
            => cache.Subscribe(symbol, default, callback);
    }
}
