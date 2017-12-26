using System;
using System.Threading;
using System.Threading.Tasks;
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
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task SubscribeAsync(this ICandlestickCache cache, string symbol, CandlestickInterval interval, CancellationToken token)
            => cache.SubscribeAsync(symbol, interval, default, null, token);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="symbol"></param>
        /// <param name="interval"></param>
        /// <param name="limit"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task SubscribeAsync(this ICandlestickCache cache, string symbol, CandlestickInterval interval, int limit, CancellationToken token)
            => cache.SubscribeAsync(symbol, interval, limit, null, token);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="symbol"></param>
        /// <param name="interval"></param>
        /// <param name="callback"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task SubscribeAsync(this ICandlestickCache cache, string symbol, CandlestickInterval interval, Action<CandlestickCacheEventArgs> callback, CancellationToken token)
            => cache.SubscribeAsync(symbol, interval, default, callback, token);
    }
}
