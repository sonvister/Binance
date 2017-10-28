using System;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceConsoleApp.Controllers
{
    public class LiveFeedOff : IHandleCommand
    {
        public async Task<bool> HandleAsync(string command, CancellationToken token = default)
        {
            if (!command.StartsWith("live ", StringComparison.OrdinalIgnoreCase))
                return false;

            var args = command.Split(' ');

            string endpoint = "";
            if (args.Length > 1)
            {
                endpoint = args[1];
            }

            if (!endpoint.Equals("off", StringComparison.OrdinalIgnoreCase))
                return false;

            await Program.DisableLiveTask();

            return true;
        }
    }
}
