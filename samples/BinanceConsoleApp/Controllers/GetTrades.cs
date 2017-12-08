using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Binance;
using Binance.Api;
using Binance.Market;

namespace BinanceConsoleApp.Controllers
{
    internal class GetTrades : IHandleCommand
    {
        public async Task<bool> HandleAsync(string command, CancellationToken token = default)
        {
            if (!command.StartsWith("trades ", StringComparison.OrdinalIgnoreCase)
                && !command.Equals("trades", StringComparison.OrdinalIgnoreCase))
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

            IEnumerable<Trade> trades = null;

            if (trades == null)
                trades = (await Program.Api.GetTradesAsync(Program.User, symbol, BinanceApi.NullId, limit, token)).Reverse();

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
