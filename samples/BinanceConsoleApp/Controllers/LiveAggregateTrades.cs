using System;
using System.Threading;
using System.Threading.Tasks;
using Binance;
using Binance.Cache;
using Microsoft.Extensions.DependencyInjection;

namespace BinanceConsoleApp.Controllers
{
    internal class LiveAggregateTrades : IHandleCommand
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

            var symbol = Symbol.BTC_USDT;
            if (args.Length > 2)
            {
                symbol = args[2];
            }

            if (!endpoint.Equals("trades", StringComparison.OrdinalIgnoreCase))
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

            Program.TradesCache = Program.ServiceProvider.GetService<IAggregateTradesCache>();

            Program.LiveTask = Task.Run(() =>
            {
                Program.TradesCache.SubscribeAsync(symbol, e => { Program.Display(e.LatestTrade()); }, token: Program.LiveTokenSource.Token);
            }, token);

            lock (Program.ConsoleSync)
            {
                Console.WriteLine();
                Console.WriteLine($"  ...live trades feed enabled for symbol: {symbol} ...use 'live off' to disable.");
            }

            return Task.FromResult(true);
        }
    }
}
