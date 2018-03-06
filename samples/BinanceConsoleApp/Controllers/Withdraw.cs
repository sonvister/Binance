using System;
using System.Threading;
using System.Threading.Tasks;
using Binance;

namespace BinanceConsoleApp.Controllers
{
    internal class Withdraw : IHandleCommand
    {
        public async Task<bool> HandleAsync(string command, CancellationToken token = default)
        {
            if (!command.StartsWith("withdraw ", StringComparison.OrdinalIgnoreCase))
                return false;

            var args = command.Split(' ');

            if (args.Length < 4)
            {
                lock (Program.ConsoleSync)
                    Console.WriteLine("An asset, address, and amount are required.");

                return true;
            }

            var asset = args[1];

            var address = args[2];

            if (!decimal.TryParse(args[3], out var amount) || amount <= 0)
            {
                lock (Program.ConsoleSync)
                    Console.WriteLine("An amount greater than 0 is required.");

                return true;
            }

            string name = null;
            if (args.Length > 4)
                name = args[4];

            await Program.Api.WithdrawAsync(new WithdrawRequest(Program.User)
            {
                Asset = asset,
                Address = address,
                Amount = amount,
                Name = name
            }, token: token);

            lock (Program.ConsoleSync)
            {
                Console.WriteLine();
                Console.WriteLine($"  Withdraw request successful: {amount} {asset} => {address}");
            }

            return true;
        }
    }
}
