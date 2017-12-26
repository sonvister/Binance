using System.Linq;
using Binance.Market;

// ReSharper disable once CheckNamespace
namespace Binance.Cache.Events
{
    public static class AggregateTradeCacheEventArgsExtensions
    {
        /// <summary>
        /// Get latest trade.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static AggregateTrade LatestTrade(this AggregateTradeCacheEventArgs args)
        {
            return args.Trades.LastOrDefault();
        }
    }
}
