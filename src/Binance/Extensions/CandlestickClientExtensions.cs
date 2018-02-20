using Binance.Market;

// ReSharper disable once CheckNamespace
namespace Binance.Client
{
    public static class CandlestickClientExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="symbol"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        public static void Subscribe(this ICandlestickClient client, string symbol, CandlestickInterval interval)
            => client.Subscribe(symbol, interval,  null);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="client"></param>
        /// <param name="symbol"></param>
        /// <param name="interval"></param>
        public static void Unsubscribe(this ICandlestickClient client, string symbol, CandlestickInterval interval)
            => client.Unsubscribe(symbol, interval, null);
    }
}
