using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Binance;

namespace BinanceConsoleApp.Controllers
{
    internal class GetSymbols : IHandleCommand
    {
        public async Task<bool> HandleAsync(string command, CancellationToken token = default)
        {
            if (!command.StartsWith("symbols ", StringComparison.OrdinalIgnoreCase) &&
                !command.Equals("symbols", StringComparison.OrdinalIgnoreCase) &&
                !command.StartsWith("pairs ", StringComparison.OrdinalIgnoreCase) &&
                !command.Equals("pairs", StringComparison.OrdinalIgnoreCase))
                return false;

            var args = command.Split(' ');

            if (args.Length > 1)
            {
                if (args[1].Equals("refresh", StringComparison.OrdinalIgnoreCase) ||
                    args[1].Equals("update", StringComparison.OrdinalIgnoreCase))
                {
                    await Symbol.UpdateCacheAsync(Program.Api, token);
                }
                else
                {
                    Console.WriteLine($"Invalid command: \"{args[1]}\"");
                    return true;
                }
            }

            var symbols = Symbol.Cache.GetAll().OrderBy(s => s.ToString());
            //var symbols = await Program.Api.SymbolsAsync(token); // as string.

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
