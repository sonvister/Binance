using System.Linq;
using Binance.Cache.Events;
using Binance.Market;

// ReSharper disable once CheckNamespace
namespace Binance
{
    public static class AggregateTradesCacheEventArgsExtensions
    {
        /// <summary>
        /// Get latest trade.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static AggregateTrade LatestTrade(this AggregateTradesCacheEventArgs args)
        {
            return args.Trades.LastOrDefault();
        }
    }
}
