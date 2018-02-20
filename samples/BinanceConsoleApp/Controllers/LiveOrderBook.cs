using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Binance;
using Binance.Client;
using Binance.Client.Events;
using Binance.Market;

namespace BinanceConsoleApp.Controllers
{
    internal class LiveOrderBook : IHandleCommand
    {
        public Task<bool> HandleAsync(string command, CancellationToken token = default)
        {
            if (!command.StartsWith("live ", StringComparison.OrdinalIgnoreCase) &&
                !command.Equals("live", StringComparison.OrdinalIgnoreCase))
                return Task.FromResult(false);

            var args = command.Split(' ');

            var endpoint = "depth";
            if (args.Length > 1)
            {
                endpoint = args[1];
            }

            if (!endpoint.Equals("depth", StringComparison.OrdinalIgnoreCase)
                && !endpoint.Equals("book", StringComparison.OrdinalIgnoreCase))
                return Task.FromResult(false);

            string symbol = Symbol.BTC_USDT;
            if (args.Length > 2)
            {
                symbol = args[2];
                if (!Symbol.IsValid(symbol))
                {
                    lock (Program.ConsoleSync)
                    {
                        Console.WriteLine($"  Invalid symbol: \"{symbol}\"");
                    }
                    return Task.FromResult(true);
                }
            }

            var enable = true;
            if (args.Length > 3)
            {
                if (args[3].Equals("off", StringComparison.OrdinalIgnoreCase))
                    enable = false;
            }

            if (enable)
            {
                Program.ClientManager.DepthClient.Subscribe(symbol, 5, Display);

                lock (Program.ConsoleSync)
                {
                    Console.WriteLine();
                    Console.WriteLine($"  ...live order book (depth) ENABLED for symbol: {symbol}");
                    Console.WriteLine();
                }
            }
            else
            {
                Program.ClientManager.DepthClient.Unsubscribe(symbol, 5);

                lock (Program.ConsoleSync)
                {
                    Console.WriteLine();
                    Console.WriteLine($"  ...live order book (depth) DISABLED for symbol: {symbol}");
                    Console.WriteLine();
                }
            }

            return Task.FromResult(true);
        }

        private static void Display(DepthUpdateEventArgs e)
        {
            var top = OrderBookTop.Create(e.Symbol, e.Bids.First(), e.Asks.First());

            lock (Program.ConsoleSync)
            {
                Console.WriteLine($"  {top.Symbol}  -  Bid: {top.Bid.Price:.00000000}  |  {top.MidMarketPrice():.00000000}  |  Ask: {top.Ask.Price:.00000000}  -  Spread: {top.Spread():.00000000}");
            }
        }
    }
}
