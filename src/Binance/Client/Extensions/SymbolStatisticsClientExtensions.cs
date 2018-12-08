// ReSharper disable once CheckNamespace
namespace Binance.Client
{
    public static class SymbolStatisticsClientExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <returns></returns>
        public static ISymbolStatisticsClient Subscribe(this ISymbolStatisticsClient client)
            => client.Subscribe(null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public static ISymbolStatisticsClient Subscribe(this ISymbolStatisticsClient client, string symbol)
            => client.Subscribe(null, symbol);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="symbols"></param>
        public static ISymbolStatisticsClient Subscribe(this ISymbolStatisticsClient client, params string[] symbols)
            => client.Subscribe(null, symbols);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        public static ISymbolStatisticsClient Unsubscribe(this ISymbolStatisticsClient client)
            => client.Unsubscribe(null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="symbol"></param>
        public static ISymbolStatisticsClient Unsubscribe(this ISymbolStatisticsClient client, string symbol)
            => client.Unsubscribe(null, symbol);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="symbols"></param>
        /// <returns></returns>
        public static ISymbolStatisticsClient Unsubscribe(this ISymbolStatisticsClient client, params string[] symbols)
            => client.Unsubscribe(null, symbols);
    }
}
