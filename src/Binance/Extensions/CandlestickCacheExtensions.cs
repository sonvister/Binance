using System;
using Binance.Cache.Events;
using Binance.Market;

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
        public static void Subscribe(this ICandlestickCache cache, string symbol, CandlestickInterval interval)
            => cache.Subscribe(symbol, interval, default, null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="symbol"></param>
        /// <param name="interval"></param>
        /// <param name="limit"></param>
        /// <returns></returns>
        public static void Subscribe(this ICandlestickCache cache, string symbol, CandlestickInterval interval, int limit)
            => cache.Subscribe(symbol, interval, limit, null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="symbol"></param>
        /// <param name="interval"></param>
        /// <param name="callback"></param>
        /// <returns></returns>
        public static void Subscribe(this ICandlestickCache cache, string symbol, CandlestickInterval interval, Action<CandlestickCacheEventArgs> callback)
            => cache.Subscribe(symbol, interval, default, callback);
    }
}
