using Binance;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceConsoleApp.Controllers
{
    public class Get24hrStats : IHandleCommand
    {
        public async Task<bool> HandleAsync(string command, CancellationToken token = default)
        {
            if (!command.StartsWith("stats ", StringComparison.OrdinalIgnoreCase)
                && !command.Equals("stats", StringComparison.OrdinalIgnoreCase))
                return false;

            var args = command.Split(' ');

            string symbol = Symbol.BTC_USDT;
            if (args.Length > 1)
            {
                symbol = args[1];
            }

            var stats = await Program._api.Get24hrStatsAsync(symbol, token);

            lock (Program._consoleSync)
            {
                Console.WriteLine();
                Console.WriteLine($"  24-hour statistics for {stats.Symbol}:");
                Console.WriteLine($"    %: {stats.PriceChangePercent.ToString("0.00")} | O: {stats.OpenPrice.ToString("0.00000000")} | H: {stats.HighPrice.ToString("0.00000000")} | L: {stats.LowPrice.ToString("0.00000000")} | V: {stats.Volume.ToString("0.")}");
                Console.WriteLine($"    Bid: {stats.BidPrice.ToString("0.00000000")} | Last: {stats.LastPrice.ToString("0.00000000")} | Ask: {stats.AskPrice.ToString("0.00000000")} | Avg: {stats.WeightedAveragePrice.ToString("0.00000000")}");
                Console.WriteLine();
            }

            return true;
        }
    }
}
