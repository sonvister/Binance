using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Binance;
using Binance.Cache;
using Binance.Market;
using Binance.WebSocket.Events;
using Microsoft.Extensions.DependencyInjection;

namespace BinanceConsoleApp.Controllers
{
    internal class LiveCandlesticks : IHandleCommand
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

            if (!endpoint.Equals("kLines", StringComparison.OrdinalIgnoreCase)
                && !endpoint.Equals("candles", StringComparison.OrdinalIgnoreCase))
                return false;

            if (Program.LiveTask != null)
            {
                Program.LiveTokenSource.Cancel();
                await Program.LiveTask;
                Program.LiveTokenSource.Dispose();
            }

            var interval = CandlestickInterval.Hour;
            if (args.Length > 3)
            {
                interval = args[3].ToCandlestickInterval();
            }

            Program.LiveTokenSource = new CancellationTokenSource();

            if (Program.CandlestickCache == null)
            {
                Program.CandlestickCache = Program.ServiceProvider.GetService<ICandlestickCache>();
                Program.CandlestickCache.Client.Candlestick += OnCandlestickEvent;
            }
            else
            {
                Program.CandlestickCache.Unsubscribe();
            }

            Program.CandlestickCache.Subscribe(symbol, interval, evt => { Program.Display(evt.Candlesticks.Last()); });

            Program.LiveTask = Program.CandlestickCache.StreamAsync(Program.LiveTokenSource.Token);

            lock (Program.ConsoleSync)
            {
                Console.WriteLine();
                Console.WriteLine($"  ...live candlestick feed enabled for symbol: {symbol}, interval: {interval} ...use 'live off' to disable.");
            }

            return true;
        }

        private static void OnCandlestickEvent(object sender, CandlestickEventArgs e)
        {
            lock (Program.ConsoleSync)
            {
                Console.WriteLine($" Candlestick [{e.Candlestick.OpenTime}] - Is Final: {(e.IsFinal ? "YES" : "NO")}");
            }
        }
    }
}
