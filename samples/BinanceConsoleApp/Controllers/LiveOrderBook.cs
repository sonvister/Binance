using System;
using System.Threading;
using System.Threading.Tasks;
using Binance;
using Binance.Cache;
using Binance.Cache.Events;
using Binance.Market;
using Microsoft.Extensions.DependencyInjection;

namespace BinanceConsoleApp.Controllers
{
    internal class LiveOrderBook : IHandleCommand
    {
        public Task<bool> HandleAsync(string command, CancellationToken token = default)
        {
            if (!command.StartsWith("live ", StringComparison.OrdinalIgnoreCase)
                && !command.Equals("live", StringComparison.OrdinalIgnoreCase))
                return Task.FromResult(false);

            var args = command.Split(' ');

            var endpoint = "depth";
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

            if (Program.LiveTask != null)
            {
                lock (Program.ConsoleSync)
                {
                    Console.WriteLine("! A live task is currently active ...use 'live off' to disable.");
                }
                return Task.FromResult(true);
            }

            Program.LiveTokenSource = new CancellationTokenSource();

            Program.OrderBookCache = Program.ServiceProvider.GetService<IOrderBookCache>();
            Program.OrderBookCache.Update += OnOrderBookUpdated;

            Program.LiveTask = Task.Run(() =>
            {
                Program.OrderBookCache.SubscribeAsync(symbol, 5, Program.LiveTokenSource.Token);
            }, token);

            lock (Program.ConsoleSync)
            {
                Console.WriteLine();
                Console.WriteLine($"  ...live order book enabled for symbol: {symbol} ...use 'live off' to disable.");
            }

            return Task.FromResult(true);
        }

        private static void OnOrderBookUpdated(object sender, OrderBookCacheEventArgs e)
        {
            // NOTE: object 'sender' is IOrderBookCache (live order book)...
            //       e.OrderBook is a clone/snapshot of the live order book.
            var top = e.OrderBook.Top;
            if (top == null)
                return;

            lock (Program.ConsoleSync)
            {
                Console.WriteLine($"  {top.Symbol}  -  Bid: {top.Bid.Price:.00000000}  |  {top.MidMarketPrice():.00000000}  |  Ask: {top.Ask.Price:.00000000}  -  Spread: {top.Spread():.00000000}");
            }
        }
    }
}
