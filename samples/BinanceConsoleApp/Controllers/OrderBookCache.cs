using Binance;
using Binance.Cache;
using Binance.Cache.Events;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceConsoleApp.Controllers
{
    public class OrderBookCache : IHandleCommand
    {
        public Task<bool> HandleAsync(string command, CancellationToken token = default)
        {
            if (!command.StartsWith("live ", StringComparison.OrdinalIgnoreCase)
                && !command.Equals("live", StringComparison.OrdinalIgnoreCase))
                return Task.FromResult(false);

            var args = command.Split(' ');

            string endpoint = "depth";
            if (args.Length > 1)
            {
                endpoint = args[1];
            }

            string symbol = Symbol.BTC_USDT;
            if (args.Length > 2)
            {
                symbol = args[2];
            }

            if (!endpoint.Equals("depth", StringComparison.OrdinalIgnoreCase)
                && !endpoint.Equals("book", StringComparison.OrdinalIgnoreCase))
                return Task.FromResult(false);

            if (Program._liveTask != null)
            {
                lock (Program._consoleSync)
                {
                    Console.WriteLine($"! A live task is currently active ...use 'live off' to disable.");
                }
                return Task.FromResult(true);
            }

            Program._liveTokenSource = new CancellationTokenSource();

            Program._orderBookCache = Program._serviceProvider.GetService<IOrderBookCache>();
            Program._orderBookCache.Update += OnOrderBookUpdated;

            Program._liveTask = Task.Run(() =>
            {
                Program._orderBookCache.SubscribeAsync(symbol, token: Program._liveTokenSource.Token);
            });

            lock (Program._consoleSync)
            {
                Console.WriteLine();
                Console.WriteLine($"  ...live order book enabled for symbol: {symbol} ...use 'live off' to disable.");
            }

            return Task.FromResult(true);
        }

        private void OnOrderBookUpdated(object sender, OrderBookCacheEventArgs e)
        {
            // NOTE: object 'sender' is IOrderBookCache (live order book)...
            //       e.OrderBook is a clone/snapshot of the live order book.
            var top = e.OrderBook.Top;

            lock (Program._consoleSync)
            {
                Console.WriteLine($"  {top.Symbol}  -  Bid: {top.Bid.Price.ToString(".00000000")}  |  {top.MidMarketPrice().ToString(".00000000")}  |  Ask: {top.Ask.Price.ToString(".00000000")}  -  Spread: {top.Spread().ToString(".00000000")}");
            }
        }
    }
}
