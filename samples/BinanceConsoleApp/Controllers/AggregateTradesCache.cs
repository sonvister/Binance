using Binance;
using Binance.Cache;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceConsoleApp.Controllers
{
    public class AggregateTradesCache : IHandleCommand
    {
        public Task<bool> HandleAsync(string command, CancellationToken token = default)
        {
            if (!command.StartsWith("live ", StringComparison.OrdinalIgnoreCase))
                return Task.FromResult(false);

            var args = command.Split(' ');

            string endpoint = "";
            if (args.Length > 1)
            {
                endpoint = args[1];
            }

            string symbol = Symbol.BTC_USDT;
            if (args.Length > 2)
            {
                symbol = args[2];
            }

            if (!endpoint.Equals("trades", StringComparison.OrdinalIgnoreCase))
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

            Program._tradesCache = Program._serviceProvider.GetService<IAggregateTradesCache>();

            Program._liveTask = Task.Run(() =>
            {
                Program._tradesCache.SubscribeAsync(symbol, (e) => { Program.Display(e.LatestTrade()); }, token: Program._liveTokenSource.Token);
            });

            lock (Program._consoleSync)
            {
                Console.WriteLine();
                Console.WriteLine($"  ...live trades feed enabled for symbol: {symbol} ...use 'live off' to disable.");
            }

            return Task.FromResult(true);
        }
    }
}
