using Binance;
using Binance.Api.WebSocket.Events;
using Binance.Cache;
using Binance.Market;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceConsoleApp.Controllers
{
    internal class CandlestickCache : IHandleCommand
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

            if (!endpoint.Equals("kline", StringComparison.OrdinalIgnoreCase)
                && !endpoint.Equals("candle", StringComparison.OrdinalIgnoreCase))
                return Task.FromResult(false);

            if (Program.LiveTask != null)
            {
                lock (Program.ConsoleSync)
                {
                    Console.WriteLine("! A live task is currently active ...use 'live off' to disable.");
                }
                return Task.FromResult(true);
            }

            var interval = KlineInterval.Hour;
            if (args.Length > 3)
            {
                interval = args[3].ToKlineInterval();
            }

            Program.LiveTokenSource = new CancellationTokenSource();

            Program.KlineCache = Program.ServiceProvider.GetService<ICandlesticksCache>();
            Program.KlineCache.Client.Kline += OnKlineEvent;

            Program.LiveTask = Task.Run(() =>
            {
                Program.KlineCache.SubscribeAsync(symbol, interval, (e) => { Program.Display(e.Candlesticks.Last()); }, token: Program.LiveTokenSource.Token);
            }, token);

            lock (Program.ConsoleSync)
            {
                Console.WriteLine();
                Console.WriteLine($"  ...live kline feed enabled for symbol: {symbol}, interval: {interval} ...use 'live off' to disable.");
            }

            return Task.FromResult(true);
        }

        private void OnKlineEvent(object sender, KlineEventArgs e)
        {
            lock (Program.ConsoleSync)
            {
                Console.WriteLine($" Candlestick [{e.Candlestick.OpenTime}] - Is Final: {(e.IsFinal ? "YES" : "NO")}");
            }
        }
    }
}
