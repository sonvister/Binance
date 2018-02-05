using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Binance;

namespace BinanceConsoleApp.Controllers
{
    internal class GetTrades : IHandleCommand
    {
        public async Task<bool> HandleAsync(string command, CancellationToken token = default)
        {
            if (!command.StartsWith("trades ", StringComparison.OrdinalIgnoreCase) &&
                !command.Equals("trades", StringComparison.OrdinalIgnoreCase))
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

            // TODO: If live trades cache is active (for symbol), get cached data.
            //if (Program.TradeCache != null && Program.TradeCache.Trades.FirstOrDefault()?.Symbol == symbol)
            //    trades = Program.TradeCache.Trades.Reverse().Take(limit); // get local cache.

            var trades = (await Program.Api.GetTradesAsync(symbol, limit, token))
                    .Reverse().ToArray();

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
