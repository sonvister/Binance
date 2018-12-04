using System;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceConsoleApp.Controllers
{
    internal class GetAvgPrice : IHandleCommand
    {
        public async Task<bool> HandleAsync(string command, CancellationToken token = default)
        {
            if (!command.StartsWith("avgPrice ", StringComparison.OrdinalIgnoreCase) &&
                !command.Equals("avgPrice", StringComparison.OrdinalIgnoreCase))
                return false;

            var args = command.Split(' ');

            if (args.Length < 2)
            {
                lock (Program.ConsoleSync)
                {
                    Console.WriteLine("A symbol is required.");
                    return true;
                }
            }

            var symbol = args[1];

            var price = await Program.Api.GetAvgPriceAsync(symbol, token);
            lock (Program.ConsoleSync)
            {
                Program.Display(price);
                Console.WriteLine();
            }

            return true;
        }
    }
}
