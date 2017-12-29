using System;
using System.Threading;
using System.Threading.Tasks;
using Binance;
using System.Linq;
using Binance.Api;

namespace BinanceConsoleApp.Controllers
{
    internal class GetTradesFrom : IHandleCommand
    {
        public async Task<bool> HandleAsync(string command, CancellationToken token = default)
        {
            if (!command.StartsWith("tradesFrom ", StringComparison.OrdinalIgnoreCase) &&
                !command.Equals("tradesFrom", StringComparison.OrdinalIgnoreCase))
                return false;

            var args = command.Split(' ');

            string symbol = Symbol.BTC_USDT;
            long fromId = BinanceApi.NullId;

            if (args.Length > 1)
            {
                if (!long.TryParse(args[1], out fromId))
                {
                    symbol = args[1];
                    fromId = BinanceApi.NullId;
                }
            }

            if (args.Length > 2)
            {
                long.TryParse(args[2], out fromId);
            }

            var limit = 10;
            if (args.Length > 3)
            {
                int.TryParse(args[3], out limit);
            }

            var trades = (await Program.Api.GetTradesFromAsync(Program.User, symbol, fromId, limit, token))
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
