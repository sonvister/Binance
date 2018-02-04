using System;
using System.Threading;
using System.Threading.Tasks;
using Binance;
using Binance.Cache;
using Binance.Cache.Events;
using Microsoft.Extensions.DependencyInjection;

namespace BinanceConsoleApp.Controllers
{
    internal class LiveAggregateTrades : IHandleCommand
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

            if (!endpoint.Equals("aggTrades", StringComparison.OrdinalIgnoreCase))
                return false;

            if (Program.LiveTask != null)
            {
                Program.LiveTokenSource.Cancel();
                await Program.LiveTask;
                Program.LiveTokenSource.Dispose();
            }

            Program.LiveTokenSource = new CancellationTokenSource();

            if (Program.AggregateTradeCache == null)
            {
                Program.AggregateTradeCache = Program.ServiceProvider.GetService<IAggregateTradeCache>();
            }
            else
            {
                Program.AggregateTradeCache.Unsubscribe();
            }

            Program.AggregateTradeCache.Subscribe(symbol, 1, evt => { Program.Display(evt.LatestTrade()); });

            Program.LiveTask = Program.AggregateTradeCache.StreamAsync(Program.LiveTokenSource.Token);

            lock (Program.ConsoleSync)
            {
                Console.WriteLine();
                Console.WriteLine($"  ...live aggregate trades feed enabled for symbol: {symbol} ...use 'live off' to disable.");
            }

            return true;
        }
    }
}
