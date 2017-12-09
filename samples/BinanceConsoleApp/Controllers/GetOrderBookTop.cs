using System;
using System.Threading;
using System.Threading.Tasks;
using Binance;

namespace BinanceConsoleApp.Controllers
{
    internal class GetOrderBookTop : IHandleCommand
    {
        public async Task<bool> HandleAsync(string command, CancellationToken token = default)
        {
            if (!command.StartsWith("top ", StringComparison.OrdinalIgnoreCase)
                && !command.Equals("top", StringComparison.OrdinalIgnoreCase))
                return false;

            var args = command.Split(' ');

            string symbol = Symbol.BTC_USDT;
            if (args.Length > 1)
            {
                symbol = args[1];
            }

            var top = await Program.Api.GetOrderBookTopAsync(symbol, token);
            Program.Display(top);

            return true;
        }
    }
}
