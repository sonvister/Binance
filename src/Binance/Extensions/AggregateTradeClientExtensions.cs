// ReSharper disable once CheckNamespace
namespace Binance.Client
{
    public static class AggregateTradeClientExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="symbol"></param>
        public static void Subscribe(this IAggregateTradeClient client, string symbol)
            => client.Subscribe(symbol, null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="symbol"></param>
        public static void Unsubscribe(this IAggregateTradeClient client, string symbol)
            => client.Unsubscribe(symbol, null);
    }
}
