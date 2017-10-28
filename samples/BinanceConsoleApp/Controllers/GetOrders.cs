using Binance;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceConsoleApp.Controllers
{
    public class GetOrders : IHandleCommand
    {
        public async Task<bool> HandleAsync(string command, CancellationToken token = default)
        {
            if (!command.StartsWith("orders ", StringComparison.OrdinalIgnoreCase)
                && !command.Equals("orders", StringComparison.OrdinalIgnoreCase))
                return false;

            if (Program._user == null)
            {
                Program.PrintApiNotice();
                return true;
            }

            var args = command.Split(' ');

            string symbol = Symbol.BTC_USDT;
            bool openOrders = false;
            int limit = 10;

            if (args.Length > 1)
            {
                if (!int.TryParse(args[1], out limit))
                {
                    symbol = args[1];
                    limit = 10;
                }
            }

            if (args.Length > 2)
            {
                if (!int.TryParse(args[2], out limit))
                {
                    if (args[2].Equals("open", StringComparison.OrdinalIgnoreCase))
                        openOrders = true;

                    limit = 10;
                }
            }

            var orders = openOrders
                ? await Program._api.GetOpenOrdersAsync(Program._user, symbol, token: token)
                : await Program._api.GetOrdersAsync(Program._user, symbol, limit: limit, token: token);

            lock (Program._consoleSync)
            {
                Console.WriteLine();
                if (!orders.Any())
                {
                    Console.WriteLine("[None]");
                }
                else
                {
                    foreach (var order in orders)
                    {
                        Program.Display(order);
                    }
                }
                Console.WriteLine();
            }

            return true;
        }
    }
}
