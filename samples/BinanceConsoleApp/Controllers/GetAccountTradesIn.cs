using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Binance;

namespace BinanceConsoleApp.Controllers
{
    internal class GetAccountTradesIn : IHandleCommand
    {
        public async Task<bool> HandleAsync(string command, CancellationToken token = default)
        {
            if (!command.StartsWith("myTradesIn ", StringComparison.OrdinalIgnoreCase) &&
                !command.Equals("myTradesIn", StringComparison.OrdinalIgnoreCase))
                return false;

            if (Program.User == null)
            {
                Program.PrintApiNotice();
                return true;
            }

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

            var trades = (await Program.Api.GetAccountTradesAsync(Program.User, symbol, (startTime, endTime), token: token))
                .Reverse().ToArray();

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
