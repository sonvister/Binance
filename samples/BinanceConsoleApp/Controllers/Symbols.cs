using Binance;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceConsoleApp.Controllers
{
    internal class Symbols : IHandleCommand
    {
        public async Task<bool> HandleAsync(string command, CancellationToken token = default)
        {
            if (!command.Equals("symbols", StringComparison.OrdinalIgnoreCase))
                return false;

            var symbols = await Program.Api.SymbolsAsync(token);

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
