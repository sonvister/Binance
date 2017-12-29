using System;
using System.Threading;
using System.Threading.Tasks;
using Binance;

namespace BinanceConsoleApp.Controllers
{
    internal class GetDepositAddress : IHandleCommand
    {
        public async Task<bool> HandleAsync(string command, CancellationToken token = default)
        {
            if (!command.StartsWith("address ", StringComparison.OrdinalIgnoreCase) &&
                !command.Equals("address", StringComparison.OrdinalIgnoreCase))
                return false;

            var args = command.Split(' ');

            string asset = Asset.BTC;
            if (args.Length > 1)
            {
                asset = args[1];
            }

            Program.Display(await Program.Api.GetDepositAddressAsync(Program.User, asset, token));

            return true;
        }
    }
}
