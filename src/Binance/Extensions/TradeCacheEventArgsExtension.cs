using System.Linq;

// ReSharper disable once CheckNamespace
namespace Binance.Cache
{
    public static class TradeCacheEventArgsExtensions
    {
        /// <summary>
        /// Get latest trade.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public static Trade LatestTrade(this TradeCacheEventArgs args)
        {
            return args.Trades.LastOrDefault();
        }
    }
}
