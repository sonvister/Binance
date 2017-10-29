using System;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceConsoleApp.Controllers
{
    internal class GetAccountInfo : IHandleCommand
    {
        public async Task<bool> HandleAsync(string command, CancellationToken token = default)
        {
            if (!command.Equals("account", StringComparison.OrdinalIgnoreCase)
                && !command.Equals("balances", StringComparison.OrdinalIgnoreCase))
                return false;

            if (Program.User == null)
            {
                Program.PrintApiNotice();
                return true;
            }

            var account = await Program.Api.GetAccountInfoAsync(Program.User, token: token);

            Program.Display(account);

            return true;
        }
    }
}
