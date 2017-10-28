using Binance;
using Binance.Market;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceConsoleApp.Controllers
{
    public class GetCandlesticks : IHandleCommand
    {
        public async Task<bool> HandleAsync(string command, CancellationToken token = default)
        {
            if (!command.StartsWith("candles ", StringComparison.OrdinalIgnoreCase)
                && !command.Equals("candles", StringComparison.OrdinalIgnoreCase)
                && !command.StartsWith("klines ", StringComparison.OrdinalIgnoreCase)
                && !command.Equals("klines", StringComparison.OrdinalIgnoreCase))
                return false;

            var args = command.Split(' ');

            string symbol = Symbol.BTC_USDT;
            if (args.Length > 1)
            {
                symbol = args[1];
            }

            var interval = KlineInterval.Hour;
            if (args.Length > 2)
            {
                interval = args[2].ToKlineInterval();
            }

            int limit = 10;
            if (args.Length > 3)
            {
                int.TryParse(args[3], out limit);
            }

            IEnumerable<Candlestick> candlesticks = null;

            // If live order book is active (for symbol), get cached data.
            if (Program._klineCache != null && Program._klineCache.Candlesticks.FirstOrDefault()?.Symbol == symbol)
                candlesticks = Program._klineCache.Candlesticks.Reverse().Take(limit); // get local cache.

            if (candlesticks == null)
                candlesticks = await Program._api.GetCandlesticksAsync(symbol, interval, limit, token: token);

            lock (Program._consoleSync)
            {
                Console.WriteLine();
                foreach (var candlestick in candlesticks)
                {
                    Program.Display(candlestick);
                }
                Console.WriteLine();
            }

            return true;
        }
    }
}
