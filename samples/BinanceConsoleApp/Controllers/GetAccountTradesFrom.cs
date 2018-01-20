using System;
using System.Threading;
using System.Threading.Tasks;
using Binance;
using System.Linq;
using Binance.Api;

namespace BinanceConsoleApp.Controllers
{
    internal class GetAccountTradesFrom : IHandleCommand
    {
        public async Task<bool> HandleAsync(string command, CancellationToken token = default)
        {
            if (!command.StartsWith("myTradesFrom ", StringComparison.OrdinalIgnoreCase) &&
                !command.Equals("myTradesFrom", StringComparison.OrdinalIgnoreCase))
                return false;

            if (Program.User == null)
            {
                Program.PrintApiNotice();
                return true;
            }

            var args = command.Split(' ');

            string symbol = Symbol.BTC_USDT;
            var fromId = BinanceApi.NullId;

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

            var trades = await Program.Api.GetAccountTradesAsync(Program.User, symbol, fromId, limit, token: token);

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
