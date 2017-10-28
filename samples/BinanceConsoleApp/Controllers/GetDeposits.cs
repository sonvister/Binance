using Binance;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceConsoleApp.Controllers
{
    public class GetDeposits : IHandleCommand
    {
        public async Task<bool> HandleAsync(string command, CancellationToken token = default)
        {
            if (!command.StartsWith("deposits ", StringComparison.OrdinalIgnoreCase)
                && !command.Equals("deposits", StringComparison.OrdinalIgnoreCase))
                return false;

            if (Program._user == null)
            {
                Program.PrintApiNotice();
                return true;
            }

            var args = command.Split(' ');

            string asset = null;
            if (args.Length > 1)
            {
                asset = args[1];
            }

            var deposits = await Program._api.GetDepositsAsync(Program._user, asset, token: token);

            lock (Program._consoleSync)
            {
                Console.WriteLine();
                if (!deposits.Any())
                {
                    Console.WriteLine("[None]");
                }
                else
                {
                    foreach (var deposit in deposits)
                    {
                        Console.WriteLine($"  {deposit.Time().ToLocalTime()} - {deposit.Asset.PadLeft(4)} - {deposit.Amount.ToString("0.00000000")} - Status: {deposit.Status}");
                    }
                }
                Console.WriteLine();
            }

            return true;
        }
    }
}
