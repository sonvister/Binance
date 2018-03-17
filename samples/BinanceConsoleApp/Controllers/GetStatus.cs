using System;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceConsoleApp.Controllers
{
    internal class GetStatus : IHandleCommand
    {
        public async Task<bool> HandleAsync(string command, CancellationToken token = default)
        {
            if (!command.Equals("status", StringComparison.OrdinalIgnoreCase) &&
                !command.StartsWith("status ", StringComparison.OrdinalIgnoreCase))
                return false;

            var args = command.Split(' ');

            if (args.Length < 2 || args[1].Equals("account", StringComparison.OrdinalIgnoreCase))
            {
                var status = await Program.Api.GetAccountStatusAsync(Program.User, token: token);

                lock (Program.ConsoleSync)
                {
                    Console.WriteLine();
                    Console.WriteLine($"Status [Account]: \"{status}\"");
                }
            }
            else if (args.Length > 1 && args[1].Equals("system", StringComparison.OrdinalIgnoreCase))
            {
                var status = await Program.Api.GetSystemStatusAsync(token);

                lock (Program.ConsoleSync)
                {
                    Console.WriteLine();
                    Console.WriteLine($"Status [System]: \"{status}\"");
                }
            }

            return true;
        }
    }
}
