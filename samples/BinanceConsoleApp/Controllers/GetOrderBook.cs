using Binance;
using Binance.Market;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceConsoleApp.Controllers
{
    public class GetOrderBook : IHandleCommand
    {
        public async Task<bool> HandleAsync(string command, CancellationToken token = default)
        {
            if (!command.StartsWith("depth ", StringComparison.OrdinalIgnoreCase)
                && !command.Equals("depth", StringComparison.OrdinalIgnoreCase)
                && !command.StartsWith("book ", StringComparison.OrdinalIgnoreCase)
                && !command.Equals("book", StringComparison.OrdinalIgnoreCase))
                return false;

            var args = command.Split(' ');

            string symbol = Symbol.BTC_USDT;
            int limit = 10;

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
                int.TryParse(args[2], out limit);
            }

            OrderBook orderBook = null;

            // If live order book is active (for symbol), get cached data.
            if (Program._orderBookCache != null && Program._orderBookCache.OrderBook.Symbol == symbol)
                orderBook = Program._orderBookCache.OrderBook; // get local cache.

            // Query order book from API, if needed.
            if (orderBook == null)
                orderBook = await Program._api.GetOrderBookAsync(symbol, limit, token);

            lock (Program._consoleSync)
            {
                Console.WriteLine();
                orderBook.Print(Console.Out, limit);
                Console.WriteLine();
            }

            return true;
        }
    }
}
