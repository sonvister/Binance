using System;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceConsoleApp.Controllers
{
    internal class GetOrderBookTops : IHandleCommand
    {
        public async Task<bool> HandleAsync(string command, CancellationToken token = default)
        {
            if (!command.Equals("tops", StringComparison.OrdinalIgnoreCase))
                return false;

            var tops = await Program.Api.GetOrderBookTopsAsync(token);

            lock (Program.ConsoleSync)
            {
                Console.WriteLine();
                foreach (var top in tops)
                {
                    Program.Display(top);
                }
                Console.WriteLine();
            }

            return true;
        }
    }
}
