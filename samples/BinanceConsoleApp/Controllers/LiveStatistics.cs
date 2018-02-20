using System;
using System.Threading;
using System.Threading.Tasks;
using Binance;
using Binance.Client;
using Binance.Client.Events;

namespace BinanceConsoleApp.Controllers
{
    internal class LiveStatistics : IHandleCommand
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

            if (!endpoint.Equals("stats", StringComparison.OrdinalIgnoreCase))
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
                Program.ClientManager.StatisticsClient.Subscribe(Display, symbol);

                lock (Program.ConsoleSync)
                {
                    Console.WriteLine();
                    Console.WriteLine($"  ...live statistics feed ENABLED for symbol: {symbol}");
                    Console.WriteLine();
                }
            }
            else
            {
                Program.ClientManager.StatisticsClient.Unsubscribe(symbol);

                lock (Program.ConsoleSync)
                {
                    Console.WriteLine();
                    Console.WriteLine($"  ...live statistics feed DISABLED for symbol: {symbol}");
                    Console.WriteLine();
                }
            }

            return Task.FromResult(true);
        }

        private static void Display(SymbolStatisticsEventArgs args)
            => Program.Display(args.Statistics);
    }
}
