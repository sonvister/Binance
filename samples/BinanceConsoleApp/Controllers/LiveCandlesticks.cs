using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Binance;
using Binance.Cache;
using Binance.Market;
using Binance.WebSocket;
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

            if (!endpoint.Equals("kLines", StringComparison.OrdinalIgnoreCase)
                && !endpoint.Equals("candles", StringComparison.OrdinalIgnoreCase))
                return false;

            string symbol = Symbol.BTC_USDT;
            if (args.Length > 2)
            {
                symbol = args[2];
            }

            var interval = CandlestickInterval.Hour;
            if (args.Length > 3)
            {
                interval = args[3].ToCandlestickInterval();
            }

            bool enable = true;
            if (args.Length > 4)
            {
                if (args[4].Equals("off", StringComparison.OrdinalIgnoreCase))
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

            if (Program.CandlestickClient == null)
            {
                Program.CandlestickClient = Program.ServiceProvider.GetService<ICandlestickWebSocketClient>();
                Program.CandlestickClient.Candlestick += OnCandlestickEvent;
            }

            if (enable)
            {
                Program.CandlestickClient.Subscribe(symbol, interval, evt => { Program.Display(evt.Candlestick); });
            }
            else
            {
                Program.CandlestickClient.Unsubscribe(symbol, interval);
            }

            if (Program.CandlestickClient.WebSocket.SubscribedStreams.Any())
            {
                Program.LiveTask = Program.CandlestickClient.StreamAsync(Program.LiveTokenSource.Token);
            }

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
