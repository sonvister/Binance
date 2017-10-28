using Binance;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceConsoleApp.Controllers
{
    public class GetTime : IHandleCommand
    {
        public async Task<bool> HandleAsync(string command, CancellationToken token = default)
        {
            if (!command.Equals("time", StringComparison.OrdinalIgnoreCase))
                return false;

            var time = await Program._api.GetTimeAsync(token);

            lock (Program._consoleSync)
            {
                Console.WriteLine($"  {time.Kind.ToString().ToUpper()} Time: {time}  [Local: {time.ToLocalTime()}]");
                Console.WriteLine();
            }

            return true;
        }
    }
}
