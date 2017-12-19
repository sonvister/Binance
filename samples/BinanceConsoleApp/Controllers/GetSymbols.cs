using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceConsoleApp.Controllers
{
    internal class GetSymbols : IHandleCommand
    {
        public async Task<bool> HandleAsync(string command, CancellationToken token = default)
        {
            if (!command.Equals("symbols", StringComparison.OrdinalIgnoreCase) &&
                !command.Equals("pairs", StringComparison.OrdinalIgnoreCase))
                return false;

            var symbols = await Program.Api.GetSymbolsAsync(token);
            //var symbols = await Program.Api.SymbolsAsync(token); // faster.

            lock (Program.ConsoleSync)
            {
                Console.WriteLine();
                Console.WriteLine(string.Join(", ", symbols.Select(s => s)));
                Console.WriteLine();
            }

            return true;
        }
    }
}
