using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Binance;

// ReSharper disable PossibleMultipleEnumeration

namespace BinanceConsoleApp.Controllers
{
    internal class GetDeposits : IHandleCommand
    {
        public async Task<bool> HandleAsync(string command, CancellationToken token = default)
        {
            if (!command.StartsWith("deposits ", StringComparison.OrdinalIgnoreCase)
                && !command.Equals("deposits", StringComparison.OrdinalIgnoreCase))
                return false;

            if (Program.User == null)
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

            var deposits = await Program.Api.GetDepositsAsync(Program.User, asset, token: token);

            lock (Program.ConsoleSync)
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
                        Console.WriteLine($"  {deposit.Time().ToLocalTime()} - {deposit.Asset.PadLeft(4)} - {deposit.Amount:0.00000000} - Status: {deposit.Status}");
                    }
                }
                Console.WriteLine();
            }

            return true;
        }
    }
}
