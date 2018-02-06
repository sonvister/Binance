using System;
using System.Threading;
using System.Threading.Tasks;
using Binance;
using Binance.Market;
using Binance.WebSocket;
using Binance.WebSocket.Events;

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

            if (!endpoint.Equals("kLines", StringComparison.OrdinalIgnoreCase)
                && !endpoint.Equals("candles", StringComparison.OrdinalIgnoreCase))
                return Task.FromResult(false);

            string symbol = Symbol.BTC_USDT;
            if (args.Length > 2)
            {
                symbol = args[2];
                if (!Symbol.IsValid(symbol))
                {
                    lock (Program.ConsoleSync)
                    {
                        Console.WriteLine($"  Invalid symbol: \"{symbol}\"");
                    }
                    return Task.FromResult(true);
                }
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

            if (enable)
            {
                Program.ClientManager.CandlestickClient.Subscribe(symbol, interval, evt => { Program.Display(evt.Candlestick); });
                //Program.ClientManager.CandlestickClient.Candlestick += OnCandlestickEvent; // TODO

                lock (Program.ConsoleSync)
                {
                    Console.WriteLine();
                    Console.WriteLine($"  ...live candlestick feed ENABLED for symbol: {symbol}, interval: {interval}");
                    Console.WriteLine();
                }
            }
            else
            {
                Program.ClientManager.CandlestickClient.Unsubscribe(symbol, interval);

                lock (Program.ConsoleSync)
                {
                    Console.WriteLine();
                    Console.WriteLine($"  ...live candlestick feed DISABLED for symbol: {symbol}, interval: {interval}");
                    Console.WriteLine();
                }
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
