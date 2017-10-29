using Binance;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceConsoleApp.Controllers
{
    internal class Get24HourStatistics : IHandleCommand
    {
        public async Task<bool> HandleAsync(string command, CancellationToken token = default)
        {
            if (!command.StartsWith("stats ", StringComparison.OrdinalIgnoreCase)
                && !command.Equals("stats", StringComparison.OrdinalIgnoreCase))
                return false;

            var args = command.Split(' ');

            var symbol = Symbol.BTC_USDT;
            if (args.Length > 1)
            {
                symbol = args[1];
            }

            var stats = await Program.Api.Get24HourStatisticsAsync(symbol, token);

            lock (Program.ConsoleSync)
            {
                Console.WriteLine();
                Console.WriteLine($"  24-hour statistics for {stats.Symbol}:");
                Console.WriteLine($"    %: {stats.PriceChangePercent:0.00} | O: {stats.OpenPrice:0.00000000} | H: {stats.HighPrice:0.00000000} | L: {stats.LowPrice:0.00000000} | V: {stats.Volume:0.}");
                Console.WriteLine($"    Bid: {stats.BidPrice:0.00000000} | Last: {stats.LastPrice:0.00000000} | Ask: {stats.AskPrice:0.00000000} | Avg: {stats.WeightedAveragePrice:0.00000000}");
                Console.WriteLine();
            }

            return true;
        }
    }
}
