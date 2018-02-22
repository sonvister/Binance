using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Binance;

namespace BinanceConsoleApp.Controllers
{
    internal class GetAggregateTrades : IHandleCommand
    {
        public async Task<bool> HandleAsync(string command, CancellationToken token = default)
        {
            if (!command.StartsWith("aggTrades ", StringComparison.OrdinalIgnoreCase) &&
                !command.Equals("aggTrades", StringComparison.OrdinalIgnoreCase))
                return false;

            var args = command.Split(' ');

            string symbol = Symbol.BTC_USDT;
            if (args.Length > 1)
            {
                symbol = args[1];
            }

            var limit = 10;
            if (args.Length > 2)
            {
                int.TryParse(args[2], out limit);
            }

            var trades = (await Program.Api.GetAggregateTradesAsync(symbol, limit, token)).Reverse();

            lock (Program.ConsoleSync)
            {
                Console.WriteLine();
                foreach (var trade in trades)
                {
                    Program.Display(trade);
                }
                Console.WriteLine();
            }

            return true;
        }
    }
}
