using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Binance;
using Binance.Cache;
using Binance.Market;
using Binance.WebSocket;
using Binance.WebSocket.Events;
using Microsoft.Extensions.DependencyInjection;

namespace BinanceConsoleApp.Controllers
{
    internal class LiveOrderBook : IHandleCommand
    {
        public async Task<bool> HandleAsync(string command, CancellationToken token = default)
        {
            if (!command.StartsWith("live ", StringComparison.OrdinalIgnoreCase) &&
                !command.Equals("live", StringComparison.OrdinalIgnoreCase))
                return false;

            var args = command.Split(' ');

            var endpoint = "depth";
            if (args.Length > 1)
            {
                endpoint = args[1];
            }

            if (!endpoint.Equals("depth", StringComparison.OrdinalIgnoreCase)
                && !endpoint.Equals("book", StringComparison.OrdinalIgnoreCase))
                return false;

            string symbol = Symbol.BTC_USDT;
            if (args.Length > 2)
            {
                symbol = args[2];
            }

            bool enable = true;
            if (args.Length > 3)
            {
                if (args[3].Equals("off", StringComparison.OrdinalIgnoreCase))
                    enable = false;
            }

            if (Program.LiveTask != null)
            {
                Program.LiveTokenSource.Cancel();
                if (!Program.LiveTask.IsCompleted)
                    await Program.LiveTask;
                Program.LiveTokenSource.Dispose();
            }

            Program.LiveTokenSource = new CancellationTokenSource();

            if (Program.OrderBookClient == null)
            {
                Program.OrderBookClient = Program.ServiceProvider.GetService<IDepthWebSocketClient>();
                Program.OrderBookClient.DepthUpdate += OnOrderBookUpdated;
            }

            if (enable)
            {
                Program.OrderBookClient.Subscribe(symbol, 5);
            }
            else
            {
                Program.OrderBookClient.Unsubscribe(symbol, 5);
            }

            if (Program.OrderBookClient.WebSocket.SubscribedStreams.Any())
            {
                Program.LiveTask = Program.OrderBookClient.StreamAsync(Program.LiveTokenSource.Token);
            }

            lock (Program.ConsoleSync)
            {
                Console.WriteLine();
                Console.WriteLine($"  ...live order book enabled for symbol: {symbol} ...use 'live off' to disable.");
            }

            return true;
        }

        private static void OnOrderBookUpdated(object sender, DepthUpdateEventArgs e)
        {
            var top = OrderBookTop.Create(e.Symbol, e.Bids.First(), e.Asks.First());

            lock (Program.ConsoleSync)
            {
                Console.WriteLine($"  {top.Symbol}  -  Bid: {top.Bid.Price:.00000000}  |  {top.MidMarketPrice():.00000000}  |  Ask: {top.Ask.Price:.00000000}  -  Spread: {top.Spread():.00000000}");
            }
        }
    }
}
