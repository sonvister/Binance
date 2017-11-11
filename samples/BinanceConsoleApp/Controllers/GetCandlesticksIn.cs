using System;
using System.Threading;
using System.Threading.Tasks;
using Binance;
using Binance.Market;

namespace BinanceConsoleApp.Controllers
{
    internal class GetCandlesticksIn : IHandleCommand
    {
        public async Task<bool> HandleAsync(string command, CancellationToken token = default)
        {
            if (!command.StartsWith("candlesIn ", StringComparison.OrdinalIgnoreCase)
                && !command.Equals("candlesIn", StringComparison.OrdinalIgnoreCase)
                && !command.StartsWith("klinesIn ", StringComparison.OrdinalIgnoreCase)
                && !command.Equals("klinesIn", StringComparison.OrdinalIgnoreCase))
                return false;

            var args = command.Split(' ');

            var symbol = Symbol.BTC_USDT;
            if (args.Length > 1)
            {
                symbol = args[1];
            }

            var interval = CandlestickInterval.Hour;
            if (args.Length > 2)
            {
                interval = args[2].ToCandlestickInterval();
            }

            long startTime = 0;
            if (args.Length > 3)
            {
                long.TryParse(args[3], out startTime);
            }

            long endTime = 0;
            if (args.Length > 4)
            {
                long.TryParse(args[4], out endTime);
            }

            var candlesticks = await Program.Api.GetCandlesticksAsync(symbol, interval, startTime: startTime, endTime: endTime, token: token);

            lock (Program.ConsoleSync)
            {
                Console.WriteLine();
                foreach (var candlestick in candlesticks)
                {
                    Program.Display(candlestick);
                }
                Console.WriteLine();
            }

            return true;
        }
    }
}
