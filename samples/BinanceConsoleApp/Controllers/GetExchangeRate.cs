using System;
using System.Threading;
using System.Threading.Tasks;
using Binance;

namespace BinanceConsoleApp.Controllers
{
    internal class GetExchangeRate : IHandleCommand
    {
        public async Task<bool> HandleAsync(string command, CancellationToken token = default)
        {
            if (!command.StartsWith("rate ", StringComparison.OrdinalIgnoreCase))
                return false;

            var args = command.Split(' ');

            if (args.Length < 3)
            {
                lock (Program.ConsoleSync)
                {
                    Console.WriteLine("A base asset and quote asset are required.");
                }
            }

            var baseAsset = args[1];
            var quoteAsset = args[2];

            var exchangeRate = await Program.Api.GetExchangeRateAsync(baseAsset, quoteAsset, token);

            lock (Program.ConsoleSync)
            {
                Console.WriteLine($"  {(baseAsset + quoteAsset).ToUpperInvariant().PadLeft(8)}: {exchangeRate}");
                Console.WriteLine();
            }

            return true;
        }
    }
}
