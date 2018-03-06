using System;
using Binance.Client;

// ReSharper disable once CheckNamespace
namespace Binance.Cache
{
    public static class CandlestickCacheExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="symbol"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        public static void Subscribe<TClient>(this ICandlestickCache<TClient> cache, string symbol, CandlestickInterval interval)
            where TClient : ICandlestickClient
            => cache.Subscribe(symbol, interval, default, null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="symbol"></param>
        /// <param name="interval"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public static void Subscribe<TClient>(this ICandlestickCache<TClient> cache, string symbol, CandlestickInterval interval, int limit)
            where TClient : ICandlestickClient
            => cache.Subscribe(symbol, interval, limit, null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="symbol"></param>
        /// <param name="interval"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static void Subscribe<TClient>(this ICandlestickCache<TClient> cache, string symbol, CandlestickInterval interval, Action<CandlestickCacheEventArgs> callback)
            where TClient : ICandlestickClient
            => cache.Subscribe(symbol, interval, default, callback);
    }
}
