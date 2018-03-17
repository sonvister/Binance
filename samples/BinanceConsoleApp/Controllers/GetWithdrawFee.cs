using System;
using System.Threading;
using System.Threading.Tasks;
using Binance;

namespace BinanceConsoleApp.Controllers
{
    internal class GetWithdrawFee : IHandleCommand
    {
        public async Task<bool> HandleAsync(string command, CancellationToken token = default)
        {
            if (!command.StartsWith("withdrawFee ", StringComparison.OrdinalIgnoreCase) &&
                !command.Equals("withdrawFee", StringComparison.OrdinalIgnoreCase))
                return false;

            var args = command.Split(' ');

            string asset = Asset.BTC;
            if (args.Length > 1)
            {
                asset = args[1];
            }

            var fee = await Program.Api.GetWithdrawFeeAsync(Program.User, asset, token: token);

            lock (Program.ConsoleSync)
            {
                Console.WriteLine($"  {asset.ToUpperInvariant()} Withdraw Fee = {fee}");
            }

            return true;
        }
    }
}
