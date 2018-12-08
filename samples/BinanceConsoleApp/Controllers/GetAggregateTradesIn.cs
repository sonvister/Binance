using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Binance;

namespace BinanceConsoleApp.Controllers
{
    internal class GetAggregateTradesIn : IHandleCommand
    {
        public async Task<bool> HandleAsync(string command, CancellationToken token = default)
        {
            if (!command.StartsWith("aggTradesIn ", StringComparison.OrdinalIgnoreCase) &&
                !command.Equals("aggTradesIn", StringComparison.OrdinalIgnoreCase))
                return false;

            var args = command.Split(' ');

            if (args.Length < 4)
            {
                lock (Program.ConsoleSync)
                {
                    Console.WriteLine("A symbol, start time, and end time are required.");
                    return true;
                }
            }

            var symbol = args[1];

            long.TryParse(args[2], out var startTime);

            long.TryParse(args[3], out var endTime);

            var trades = await Program.Api.GetAggregateTradesAsync(symbol, (startTime, endTime), token);

            lock (Program.ConsoleSync)
            {
                Console.WriteLine();
                // ReSharper disable once PossibleMultipleEnumeration
                if (!trades.Any())
                {
                    Console.WriteLine("  [None]");
                }
                else
                {
                    // ReSharper disable once PossibleMultipleEnumeration
                    foreach (var trade in trades.Reverse())
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
