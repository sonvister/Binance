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
    public class CandlestickCache : IHandleCommand
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

            if (!endpoint.Equals("kline", StringComparison.OrdinalIgnoreCase)
                && !endpoint.Equals("candle", StringComparison.OrdinalIgnoreCase))
                return Task.FromResult(false);

            if (Program._liveTask != null)
            {
                lock (Program._consoleSync)
                {
                    Console.WriteLine($"! A live task is currently active ...use 'live off' to disable.");
                }
                return Task.FromResult(true);
            }

            var interval = KlineInterval.Hour;
            if (args.Length > 3)
            {
                interval = args[3].ToKlineInterval();
            }

            Program._liveTokenSource = new CancellationTokenSource();

            Program._klineCache = Program._serviceProvider.GetService<ICandlesticksCache>();
            Program._klineCache.Client.Kline += OnKlineEvent;

            Program._liveTask = Task.Run(() =>
            {
                Program._klineCache.SubscribeAsync(symbol, interval, (e) => { Program.Display(e.Candlesticks.Last()); }, token: Program._liveTokenSource.Token);
            });

            lock (Program._consoleSync)
            {
                Console.WriteLine();
                Console.WriteLine($"  ...live kline feed enabled for symbol: {symbol}, interval: {interval} ...use 'live off' to disable.");
            }

            return Task.FromResult(true);
        }

        private void OnKlineEvent(object sender, KlineEventArgs e)
        {
            lock (Program._consoleSync)
            {
                Console.WriteLine($" Candlestick [{e.Candlestick.OpenTime}] - Is Final: {(e.IsFinal ? "YES" : "NO")}");
            }
        }
    }
}
