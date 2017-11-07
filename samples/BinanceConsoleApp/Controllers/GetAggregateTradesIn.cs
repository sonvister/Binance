using System;
using System.Threading;
using System.Threading.Tasks;
using Binance;
using System.Linq;

namespace BinanceConsoleApp.Controllers
{
    internal class GetAggregateTradesIn : IHandleCommand
    {
        public async Task<bool> HandleAsync(string command, CancellationToken token = default)
        {
            if (!command.StartsWith("tradesIn ", StringComparison.OrdinalIgnoreCase)
                && !command.Equals("tradesIn", StringComparison.OrdinalIgnoreCase))
                return false;

            var args = command.Split(' ');

            var symbol = Symbol.BTC_USDT;
            if (args.Length > 1)
            {
                symbol = args[1];
            }

            long startTime = 0;
            if (args.Length > 2)
            {
                long.TryParse(args[2], out startTime);
            }

            long endTime = 0;
            if (args.Length > 3)
            {
                long.TryParse(args[3], out endTime);
            }

            var trades = (await Program.Api.GetAggregateTradesInAsync(symbol, startTime, endTime, token)).Reverse();

            lock (Program.ConsoleSync)
            {
                Console.WriteLine();
                if (!trades.Any())
                {
                    Console.WriteLine("  [None]");
                }
                else
                {
                    foreach (var trade in trades)
                    {
                        Program.Display(trade);
                    }
                }
                Console.WriteLine();
            }

            return true;
        }
    }
}
