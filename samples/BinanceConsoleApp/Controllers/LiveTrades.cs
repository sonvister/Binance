using System;
using System.Threading;
using System.Threading.Tasks;
using Binance;
using Binance.Client;
using Binance.Client.Events;

namespace BinanceConsoleApp.Controllers
{
    internal class LiveTrades : IHandleCommand
    {
        public Task<bool> HandleAsync(string command, CancellationToken token = default)
        {
            if (!command.StartsWith("live ", StringComparison.OrdinalIgnoreCase))
                return Task.FromResult(false);

            var args = command.Split(' ');

            var endpoint = string.Empty;
            if (args.Length > 1)
            {
                endpoint = args[1];
            }

            if (!endpoint.Equals("trades", StringComparison.OrdinalIgnoreCase))
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
                Program.ClientManager.TradeClient.Subscribe(symbol, Display);

                lock (Program.ConsoleSync)
                {
                    Console.WriteLine();
                    Console.WriteLine($"  ...live trades feed ENABLED for symbol: {symbol}");
                    Console.WriteLine();
                }
            }
            else // disable.
            {
                Program.ClientManager.TradeClient.Unsubscribe(symbol);

                lock (Program.ConsoleSync)
                {
                    Console.WriteLine();
                    Console.WriteLine($"  ...live trades feed DISABLED for symbol: {symbol}");
                    Console.WriteLine();
                }
            }

            return Task.FromResult(true);
        }

        private static void Display(TradeEventArgs args)
            => Program.Display(args.Trade);
    }
}
