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
        public static void Subscribe(this ISymbolStatisticsClient client)
            => client.Subscribe(null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public static void Subscribe(this ISymbolStatisticsClient client, string symbol)
            => client.Subscribe(symbol, null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        public static void Unsubscribe(this ISymbolStatisticsClient client)
            => client.Unsubscribe(null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="symbol"></param>
        public static void Unsubscribe(this ISymbolStatisticsClient client, string symbol)
            => client.Unsubscribe(symbol, null);
    }
}
