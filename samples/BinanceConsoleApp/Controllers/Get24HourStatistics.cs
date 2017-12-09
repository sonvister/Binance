using System;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceConsoleApp.Controllers
{
    internal class Get24HourStatistics : IHandleCommand
    {
        public async Task<bool> HandleAsync(string command, CancellationToken token = default)
        {
            if (!command.StartsWith("stats ", StringComparison.OrdinalIgnoreCase)
                && !command.Equals("stats", StringComparison.OrdinalIgnoreCase))
                return false;

            var args = command.Split(' ');

            string symbol = null;
            if (args.Length > 1)
            {
                symbol = args[1];
            }

            if (string.IsNullOrWhiteSpace(symbol))
            {
                var allStats = await Program.Api.Get24HourStatisticsAsync(token);

                foreach (var stats in allStats)
                {
                    Program.Display(stats);
                }
            }
            else
            {
                var stats = await Program.Api.Get24HourStatisticsAsync(symbol, token);
                Program.Display(stats);
            }

            return true;
        }
    }
}
