using System;
using System.Threading;
using System.Threading.Tasks;
using Binance;
using Binance.WebSocket;
using Binance.WebSocket.Manager;

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

            if (!endpoint.Equals("aggTrades", StringComparison.OrdinalIgnoreCase))
                return false;

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
                    return true;
                }
            }

            var enable = true;
            if (args.Length > 3)
            {
                if (args[3].Equals("off", StringComparison.OrdinalIgnoreCase))
                    enable = false;
            }

            if (enable)
            {
                Program.ClientManager.AggregateTradeClient.Subscribe(symbol, evt => { Program.Display(evt.Trade); });

                // Optionally, wait for asynchronous client adapter operation to complete.
                await ((IBinanceWebSocketClientAdapter)Program.ClientManager.AggregateTradeClient).Task;

                lock (Program.ConsoleSync)
                {
                    Console.WriteLine();
                    Console.WriteLine($"  ...live aggregate trades feed ENABLED for symbol: {symbol}");
                    Console.WriteLine();
                }
            }
            else // disable.
            {
                Program.ClientManager.AggregateTradeClient.Unsubscribe(symbol);

                // Optionally, wait for asynchronous client adapter operation to complete.
                await ((IBinanceWebSocketClientAdapter)Program.ClientManager.AggregateTradeClient).Task;

                lock (Program.ConsoleSync)
                {
                    Console.WriteLine();
                    Console.WriteLine($"  ...live aggregate trades feed DISABLED for symbol: {symbol}");
                    Console.WriteLine();
                }
            }

            return true;
        }
    }
}
