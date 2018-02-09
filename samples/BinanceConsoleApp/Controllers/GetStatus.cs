using System;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceConsoleApp.Controllers
{
    internal class GetStatus : IHandleCommand
    {
        public async Task<bool> HandleAsync(string command, CancellationToken token = default)
        {
            if (!command.StartsWith("status ", StringComparison.OrdinalIgnoreCase))
                return false;

            var args = command.Split(' ');

            if (args.Length > 1 && args[1].Equals("account", StringComparison.OrdinalIgnoreCase))
            {
                var status = await Program.Api.GetAccountStatusAsync(Program.User, token);

                lock (Program.ConsoleSync)
                {
                    Console.WriteLine();
                    Console.WriteLine($"Account Status: \"{status}\"");
                }
            }
            else if (args.Length > 1 && args[1].Equals("system", StringComparison.OrdinalIgnoreCase))
            {
                var status = await Program.Api.GetSystemStatusAsync(token);

                lock (Program.ConsoleSync)
                {
                    Console.WriteLine();
                    Console.WriteLine($"System Status: \"{status}\"");
                }
            }
            else
            {
                lock (Program.ConsoleSync)
                {
                    Console.WriteLine($"Specify either 'account' or 'system' for status.");
                }
            }

            return true;
        }
    }
}
