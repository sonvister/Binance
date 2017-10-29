using Binance;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceConsoleApp.Controllers
{
    internal class GetTime : IHandleCommand
    {
        public async Task<bool> HandleAsync(string command, CancellationToken token = default)
        {
            if (!command.Equals("time", StringComparison.OrdinalIgnoreCase))
                return false;

            var time = await Program.Api.GetTimeAsync(token);

            lock (Program.ConsoleSync)
            {
                Console.WriteLine($"  {time.Kind.ToString().ToUpper()} Time: {time}  [Local: {time.ToLocalTime()}]");
                Console.WriteLine();
            }

            return true;
        }
    }
}
