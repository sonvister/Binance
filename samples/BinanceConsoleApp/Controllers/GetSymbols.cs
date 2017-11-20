using System;
using System.Threading;
using System.Threading.Tasks;
using Binance.Api;

namespace BinanceConsoleApp.Controllers
{
    internal class GetSymbols : IHandleCommand
    {
        public async Task<bool> HandleAsync(string command, CancellationToken token = default)
        {
            if (!command.Equals("symbols", StringComparison.OrdinalIgnoreCase) &&
                !command.Equals("pairs", StringComparison.OrdinalIgnoreCase))
                return false;

            var symbols = await Program.Api.SymbolsAsync(token);
            //var symbols = await Program.Api.GetSymbolsAsync(token); // ...too slow.

            lock (Program.ConsoleSync)
            {
                Console.WriteLine();
                Console.WriteLine(string.Join(", ", symbols));
                Console.WriteLine();
            }

            return true;
        }
    }
}
