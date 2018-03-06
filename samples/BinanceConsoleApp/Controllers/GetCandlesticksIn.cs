using System;
using System.Threading;
using System.Threading.Tasks;
using Binance;

namespace BinanceConsoleApp.Controllers
{
    internal class GetCandlesticksIn : IHandleCommand
    {
        public async Task<bool> HandleAsync(string command, CancellationToken token = default)
        {
            if (!command.StartsWith("candlesIn ", StringComparison.OrdinalIgnoreCase) &&
                !command.Equals("candlesIn", StringComparison.OrdinalIgnoreCase) &&
                !command.StartsWith("kLinesIn ", StringComparison.OrdinalIgnoreCase) &&
                !command.Equals("kLinesIn", StringComparison.OrdinalIgnoreCase))
                return false;

            var args = command.Split(' ');

            if (args.Length < 5)
            {
                lock (Program.ConsoleSync)
                {
                    Console.WriteLine("A symbol, interval, start time, and end time are required.");
                }
            }

            var symbol = args[1];

            var interval =args[2].ToCandlestickInterval();

            long.TryParse(args[3], out var startTime);

            long.TryParse(args[4], out var endTime);

            var candlesticks = await Program.Api.GetCandlesticksAsync(symbol, interval, (startTime, endTime), token: token);

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
