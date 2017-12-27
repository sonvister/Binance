using System.Threading;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace Binance.Cache
{
    public static class SymbolStatisticsCacheExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task SubscribeAsync(this ISymbolStatisticsCache cache, CancellationToken token)
            => cache.SubscribeAsync(null, token);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="symbol"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task SubscribeAsync(this ISymbolStatisticsCache cache, string symbol, CancellationToken token)
            => cache.SubscribeAsync(symbol, null, token);
    }
}
