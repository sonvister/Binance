using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Binance;
using Binance.Api.WebSocket.Events;
using Binance.Cache;
using Binance.Market;
using Microsoft.Extensions.DependencyInjection;

namespace BinanceConsoleApp.Controllers
{
    internal class LiveCandlesticks : IHandleCommand
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

            string symbol = Symbol.BTC_USDT;
            if (args.Length > 2)
            {
                symbol = args[2];
            }

            if (!endpoint.Equals("kLines", StringComparison.OrdinalIgnoreCase)
                && !endpoint.Equals("candles", StringComparison.OrdinalIgnoreCase))
                return Task.FromResult(false);

            if (Program.LiveTask != null)
            {
                lock (Program.ConsoleSync)
                {
                    Console.WriteLine("! A live task is currently active ...use 'live off' to disable.");
                }
                return Task.FromResult(true);
            }

            var interval = CandlestickInterval.Hour;
            if (args.Length > 3)
            {
                interval = args[3].ToCandlestickInterval();
            }

            Program.LiveTokenSource = new CancellationTokenSource();

            Program.CandlestickCache = Program.ServiceProvider.GetService<ICandlestickCache>();
            Program.CandlestickCache.Client.Candlestick += OnCandlestickEvent;

            Program.LiveTask = Task.Run(() =>
            {
                Program.CandlestickCache.StreamAsync(symbol, interval, e => { Program.Display(e.Candlesticks.Last()); }, Program.LiveTokenSource.Token);
            }, token);

            lock (Program.ConsoleSync)
            {
                Console.WriteLine();
                Console.WriteLine($"  ...live candlestick feed enabled for symbol: {symbol}, interval: {interval} ...use 'live off' to disable.");
            }

            return Task.FromResult(true);
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
