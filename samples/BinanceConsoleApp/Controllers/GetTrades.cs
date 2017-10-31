using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Binance;

// ReSharper disable PossibleMultipleEnumeration

namespace BinanceConsoleApp.Controllers
{
    internal class GetTrades : IHandleCommand
    {
        public async Task<bool> HandleAsync(string command, CancellationToken token = default)
        {
            if (!command.StartsWith("mytrades ", StringComparison.OrdinalIgnoreCase)
                && !command.Equals("mytrades", StringComparison.OrdinalIgnoreCase))
                return false;

            if (Program.User == null)
            {
                Program.PrintApiNotice();
                return true;
            }

            var args = command.Split(' ');

            var symbol = Symbol.BTC_USDT;
            var limit = 10;

            if (args.Length > 1)
            {
                if (!int.TryParse(args[1], out limit))
                {
                    symbol = args[1];
                    limit = 10;
                }
            }

            if (args.Length > 2)
            {
                if (!int.TryParse(args[2], out limit))
                {
                    limit = 10;
                }
            }

            var trades = await Program.Api.GetTradesAsync(Program.User, symbol, limit: limit, token: token);

            lock (Program.ConsoleSync)
            {
                Console.WriteLine();
                if (!trades.Any())
                {
                    Console.WriteLine("[None]");
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
