using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Binance;

namespace BinanceConsoleApp.Controllers
{
    internal class GetOrdersIn : IHandleCommand
    {
        public async Task<bool> HandleAsync(string command, CancellationToken token = default)
        {
            if (!command.StartsWith("ordersIn ", StringComparison.OrdinalIgnoreCase) &&
                !command.Equals("ordersIn", StringComparison.OrdinalIgnoreCase))
                return false;

            if (Program.User == null)
            {
                Program.PrintApiNotice();
                return true;
            }

            var args = command.Split(' ');

            if (args.Length < 2)
            {
                lock (Program.ConsoleSync)
                {
                    Console.WriteLine("A symbol is required.");
                    return true;
                }
            }

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

            var orders = (await Program.Api.GetOrdersAsync(Program.User, symbol, (startTime, endTime), token: token))
                .Reverse().ToArray();

            lock (Program.ConsoleSync)
            {
                Console.WriteLine();
                // ReSharper disable once PossibleMultipleEnumeration
                if (!orders.Any())
                {
                    Console.WriteLine("  [None]");
                }
                else
                {
                    // ReSharper disable once PossibleMultipleEnumeration
                    foreach (var order in orders)
                    {
                        Program.Display(order);
                    }
                }
                Console.WriteLine();
            }

            return true;
        }
    }
}
