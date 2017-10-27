using Binance.Trades;
using Binance.Trades.Cache;
using System.Linq;

namespace Binance
{
    public static class AggregateTradesCacheEventArgsExtensions
    {
        /// <summary>
        /// Get latest trade.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static AggregateTrade GetLatestTrade(this AggregateTradesCacheEventArgs args)
        {
            return args.Trades.LastOrDefault();
        }
    }
}
