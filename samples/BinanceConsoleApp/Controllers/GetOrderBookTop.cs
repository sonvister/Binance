using System;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceConsoleApp.Controllers
{
    internal class GetOrderBookTop : IHandleCommand
    {
        public async Task<bool> HandleAsync(string command, CancellationToken token = default)
        {
            if (!command.StartsWith("top ", StringComparison.OrdinalIgnoreCase) &&
                !command.Equals("top", StringComparison.OrdinalIgnoreCase))
                return false;

            var args = command.Split(' ');

            string symbol = null;
            if (args.Length > 1)
            {
                symbol = args[1];
            }

            if (string.IsNullOrWhiteSpace(symbol))
            {
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
            }
            else
            {
                var top = await Program.Api.GetOrderBookTopAsync(symbol, token);
                Program.Display(top);
            }

            return true;
        }
    }
}
