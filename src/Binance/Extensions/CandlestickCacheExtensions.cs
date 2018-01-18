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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="symbol"></param>
        /// <param name="interval"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task StreamAsync(this ICandlestickCache cache, string symbol, CandlestickInterval interval, CancellationToken token)
            => StreamAsync(cache, symbol, interval, default, null, token);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="symbol"></param>
        /// <param name="interval"></param>
        /// <param name="limit"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task StreamAsync(this ICandlestickCache cache, string symbol, CandlestickInterval interval, int limit, CancellationToken token)
            => StreamAsync(cache, symbol, interval, limit, null, token);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="symbol"></param>
        /// <param name="interval"></param>
        /// <param name="callback"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task StreamAsync(this ICandlestickCache cache, string symbol, CandlestickInterval interval, Action<CandlestickCacheEventArgs> callback, CancellationToken token)
            => StreamAsync(cache, symbol, interval, default, callback, token);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="symbol"></param>
        /// <param name="interval"></param>
        /// <param name="limit"></param>
        /// <param name="callback"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task StreamAsync(this ICandlestickCache cache, string symbol, CandlestickInterval interval, int limit, Action<CandlestickCacheEventArgs> callback, CancellationToken token)
        {
            Throw.IfNull(cache, nameof(cache));

            cache.Subscribe(symbol, interval, limit, callback);

            return cache.Client.WebSocket.StreamAsync(token);
        }
    }
}
