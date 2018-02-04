using System;
using System.Threading;
using System.Threading.Tasks;
using Binance;
using Binance.Cache;
using Microsoft.Extensions.DependencyInjection;

namespace BinanceConsoleApp.Controllers
{
    internal class LiveStatistics : IHandleCommand
    {
        public async Task<bool> HandleAsync(string command, CancellationToken token = default)
        {
            if (!command.StartsWith("live ", StringComparison.OrdinalIgnoreCase))
                return false;

            var args = command.Split(' ');

            var endpoint = string.Empty;
            if (args.Length > 1)
            {
                endpoint = args[1];
            }

            string symbol = Symbol.BTC_USDT;
            if (args.Length > 2)
            {
                symbol = args[2];
            }

            if (!endpoint.Equals("stats", StringComparison.OrdinalIgnoreCase))
                return false;

            if (Program.LiveTask != null)
            {
                Program.LiveTokenSource.Cancel();
                await Program.LiveTask;
                Program.LiveTokenSource.Dispose();
            }

            Program.LiveTokenSource = new CancellationTokenSource();

            if (Program.StatsCache == null)
            {
                Program.StatsCache = Program.ServiceProvider.GetService<ISymbolStatisticsCache>();
            }
            else
            {
                Program.StatsCache.Unsubscribe();
            }

            Program.StatsCache.Subscribe(evt => { Program.Display(evt.Statistics[0]); }, symbol);

            Program.LiveTask = Program.StatsCache.StreamAsync(Program.LiveTokenSource.Token);

            lock (Program.ConsoleSync)
            {
                Console.WriteLine();
                Console.WriteLine($"  ...live statistics feed enabled for symbol: {symbol} ...use 'live off' to disable.");
            }

            return true;
        }
    }
}
