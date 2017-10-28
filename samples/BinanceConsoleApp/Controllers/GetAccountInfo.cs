using System;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceConsoleApp.Controllers
{
    public class GetAccountInfo : IHandleCommand
    {
        public async Task<bool> HandleAsync(string command, CancellationToken token = default)
        {
            if (!command.Equals("account", StringComparison.OrdinalIgnoreCase)
                && !command.Equals("balances", StringComparison.OrdinalIgnoreCase))
                return false;

            if (Program._user == null)
            {
                Program.PrintApiNotice();
                return true;
            }

            var account = await Program._api.GetAccountAsync(Program._user, token: token);

            Program.Display(account);

            return true;
        }
    }
}
