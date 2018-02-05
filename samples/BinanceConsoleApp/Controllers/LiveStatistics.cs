using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Binance;
using Binance.Cache;
using Binance.WebSocket;
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

            if (!endpoint.Equals("stats", StringComparison.OrdinalIgnoreCase))
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

            if (Program.StatsClient == null)
            {
                Program.StatsClient = Program.ServiceProvider.GetService<ISymbolStatisticsWebSocketClient>();
            }

            if (enable)
            {
                Program.StatsClient.Subscribe(symbol, evt => { Program.Display(evt.Statistics); });
            }
            else
            {
                Program.StatsClient.Unsubscribe(symbol);
            }

            if (Program.StatsClient.WebSocket.SubscribedStreams.Any())
            {
                Program.LiveTask = Program.StatsClient.StreamAsync(Program.LiveTokenSource.Token);
            }

            lock (Program.ConsoleSync)
            {
                Console.WriteLine();
                Console.WriteLine($"  ...live statistics feed enabled for symbol: {symbol} ...use 'live off' to disable.");
            }

            return true;
        }
    }
}
