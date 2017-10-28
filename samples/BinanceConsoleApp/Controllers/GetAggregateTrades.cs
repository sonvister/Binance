using Binance;
using Binance.Market;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceConsoleApp.Controllers
{
    public class GetAggregateTrades : IHandleCommand
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

            int limit = 10;
            if (args.Length > 2)
            {
                int.TryParse(args[2], out limit);
            }

            IEnumerable<AggregateTrade> trades = null;

            // If live order book is active (for symbol), get cached data.
            if (Program._tradesCache != null && Program._tradesCache.Trades.FirstOrDefault()?.Symbol == symbol)
                trades = Program._tradesCache.Trades.Reverse().Take(limit); // get local cache.

            if (trades == null)
                trades = (await Program._api.GetAggregateTradesAsync(symbol, limit, token: token)).Reverse();

            lock (Program._consoleSync)
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
