using System;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceConsoleApp.Controllers
{
    internal class GetPrice : IHandleCommand
    {
        public async Task<bool> HandleAsync(string command, CancellationToken token = default)
        {
            if (!command.StartsWith("price ", StringComparison.OrdinalIgnoreCase) &&
                !command.Equals("price", StringComparison.OrdinalIgnoreCase))
                return false;

            var args = command.Split(' ');

            string symbol = null;
            if (args.Length > 1)
            {
                symbol = args[1];
            }

            if (string.IsNullOrWhiteSpace(symbol))
            {
                var prices = await Program.Api.GetPricesAsync(token);

                lock (Program.ConsoleSync)
                {
                    Console.WriteLine();
                    foreach (var price in prices)
                    {
                        Program.Display(price);
                    }
                    Console.WriteLine();
                }
            }
            else
            {
                var price = await Program.Api.GetPriceAsync(symbol, token);
                Program.Display(price);
                Console.WriteLine();
            }

            return true;
        }
    }
}
