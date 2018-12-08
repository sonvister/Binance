using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Binance;

namespace BinanceConsoleApp.Controllers
{
    internal class GetAssets : IHandleCommand
    {
        public async Task<bool> HandleAsync(string command, CancellationToken token = default)
        {
            if (!command.StartsWith("assets ", StringComparison.OrdinalIgnoreCase) &&
                !command.Equals("assets", StringComparison.OrdinalIgnoreCase))
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

            var assets = Asset.Cache.GetAll().OrderBy(a => a.Symbol);

            lock (Program.ConsoleSync)
            {
                Console.WriteLine();
                Console.WriteLine(string.Join(", ", assets));
                Console.WriteLine();
            }

            return true;
        }
    }
}
