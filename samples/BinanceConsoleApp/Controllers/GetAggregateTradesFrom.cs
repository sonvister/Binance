using Binance;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceConsoleApp.Controllers
{
    internal class GetAggregateTradesFrom : IHandleCommand
    {
        public async Task<bool> HandleAsync(string command, CancellationToken token = default)
        {
            if (!command.StartsWith("tradesFrom ", StringComparison.OrdinalIgnoreCase)
                && !command.Equals("tradesFrom", StringComparison.OrdinalIgnoreCase))
                return false;

            var args = command.Split(' ');

            string symbol = Symbol.BTC_USDT;
            if (args.Length > 1)
            {
                symbol = args[1];
            }

            long fromId = 0;
            if (args.Length > 2)
            {
                long.TryParse(args[2], out fromId);
            }

            int limit = 10;
            if (args.Length > 3)
            {
                int.TryParse(args[3], out limit);
            }

            var trades = await Program.Api.GetAggregateTradesFromAsync(symbol, fromId, limit, token: token);

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
