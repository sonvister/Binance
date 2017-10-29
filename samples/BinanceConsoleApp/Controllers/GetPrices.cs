using System;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceConsoleApp.Controllers
{
    internal class GetPrices : IHandleCommand
    {
        public async Task<bool> HandleAsync(string command, CancellationToken token = default)
        {
            if (!command.Equals("prices", StringComparison.OrdinalIgnoreCase))
                return false;

            var prices = await Program.Api.GetPricesAsync(token);

            lock (Program.ConsoleSync)
            {
                Console.WriteLine();
                foreach (var price in prices)
                {
                    Console.WriteLine($"  {price.Symbol.PadLeft(8)}: {price.Value}");
                }
                Console.WriteLine();
            }

            return true;
        }
    }
}
