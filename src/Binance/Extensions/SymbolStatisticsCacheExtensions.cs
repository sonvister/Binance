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
        public static void Subscribe(this ISymbolStatisticsCache cache)
            => cache.Subscribe(null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cache"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public static void Subscribe(this ISymbolStatisticsCache cache, string symbol)
            => cache.Subscribe(null, symbol);
    }
}
