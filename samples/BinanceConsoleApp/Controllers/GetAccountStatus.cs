using System;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceConsoleApp.Controllers
{
    internal class GetAccountStatus : IHandleCommand
    {
        public async Task<bool> HandleAsync(string command, CancellationToken token = default)
        {
            if (!command.Equals("status", StringComparison.OrdinalIgnoreCase))
                return false;

            var status = await Program.Api.GetAccountStatusAsync(Program.User, token);

            lock (Program.ConsoleSync)
            {
                Console.WriteLine();
                Console.WriteLine($"Account Status: \"{status}\"");
            }

            return true;
        }
    }
}
