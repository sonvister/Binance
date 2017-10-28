using Binance;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceConsoleApp.Controllers
{
    public class Symbols : IHandleCommand
    {
        public async Task<bool> HandleAsync(string command, CancellationToken token = default)
        {
            if (!command.Equals("symbols", StringComparison.OrdinalIgnoreCase))
                return false;

            var args = command.Split(' ');

            var symbols = await Program._api.SymbolsAsync(token);

            lock (Program._consoleSync)
            {
                Console.WriteLine();
                Console.WriteLine(string.Join(", ", symbols));
                Console.WriteLine();
            }

            return true;
        }
    }
}
