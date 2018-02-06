using System;
using System.Threading;
using System.Threading.Tasks;
using Binance;
using System.Linq;

namespace BinanceConsoleApp.Controllers
{
    internal class GetAggregateTradesFrom : IHandleCommand
    {
        public async Task<bool> HandleAsync(string command, CancellationToken token = default)
        {
            if (!command.StartsWith("aggTradesFrom ", StringComparison.OrdinalIgnoreCase) &&
                !command.Equals("aggTradesFrom", StringComparison.OrdinalIgnoreCase))
                return false;

            var args = command.Split(' ');

            if (args.Length < 2)
            {
                lock (Program.ConsoleSync)
                {
                    Console.WriteLine("An aggregate trade ID is required.");
                }
            }

            string symbol = Symbol.BTC_USDT;
            var limit = 10;

            if (!long.TryParse(args[1], out var fromId))
            {
                symbol = args[1];
                fromId = 0;

                if (args.Length > 2)
                {
                    long.TryParse(args[2], out fromId);
                }
            }
            else
            {
                if (args.Length > 2)
                {
                    int.TryParse(args[2], out limit);
                }
            }

            if (args.Length > 3)
            {
                int.TryParse(args[3], out limit);
            }

            var trades = (await Program.Api.GetAggregateTradesFromAsync(symbol, fromId, limit, token))
                .Reverse().ToArray();

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
