using System;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceConsoleApp.Controllers
{
    public class Ping : IHandleCommand
    {
        public async Task<bool> HandleAsync(string command, CancellationToken token = default)
        {
            if (!command.Equals("ping", StringComparison.OrdinalIgnoreCase))
                return false;

            var isSuccessful = await Program._api.PingAsync(token);

            lock (Program._consoleSync)
            {
                Console.WriteLine($"  Ping: {(isSuccessful ? "SUCCESSFUL" : "FAILED")}");
                Console.WriteLine();
            }

            return true;
        }
    }
}
