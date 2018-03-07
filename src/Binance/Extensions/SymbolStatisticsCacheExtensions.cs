using Binance.Client;

// ReSharper disable once CheckNamespace
namespace Binance.Cache
{
    public static class SymbolStatisticsCacheExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="cache"></param>
        /// <returns></returns>
        public static void Subscribe<TClient>(this ISymbolStatisticsCache<TClient> cache)
            where TClient : ISymbolStatisticsClient
            => cache.Subscribe(null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public static void Subscribe<TClient>(this ISymbolStatisticsCache<TClient> cache, string symbol)
            where TClient : ISymbolStatisticsClient
            => cache.Subscribe(null, symbol);
    }
}
