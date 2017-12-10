using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Binance;

// ReSharper disable PossibleMultipleEnumeration

namespace BinanceConsoleApp.Controllers
{
    internal class GetWithdrawals : IHandleCommand
    {
        public async Task<bool> HandleAsync(string command, CancellationToken token = default)
        {
            if (!command.StartsWith("withdrawals ", StringComparison.OrdinalIgnoreCase)
                && !command.Equals("withdrawals", StringComparison.OrdinalIgnoreCase))
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

            var withdrawals = await Program.Api.GetWithdrawalsAsync(Program.User, asset, token: token);

            lock (Program.ConsoleSync)
            {
                Console.WriteLine();
                if (!withdrawals.Any())
                {
                    Console.WriteLine("[None]");
                }
                else
                {
                    foreach (var withdrawal in withdrawals)
                    {
                        Console.WriteLine($"  {withdrawal.Time().ToLocalTime()} - {withdrawal.Asset.PadLeft(4)} - {withdrawal.Amount:0.00000000} => {withdrawal.Address} - Status: {withdrawal.Status}");
                    }
                }
                Console.WriteLine();
            }

            return true;
        }
    }
}
