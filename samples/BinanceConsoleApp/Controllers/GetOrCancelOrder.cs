using System;
using System.Threading;
using System.Threading.Tasks;
using Binance;

namespace BinanceConsoleApp.Controllers
{
    internal class GetOrCancelOrder : IHandleCommand
    {
        public async Task<bool> HandleAsync(string command, CancellationToken token = default)
        {
            if (!command.StartsWith("order ", StringComparison.OrdinalIgnoreCase))
                return false;

            var args = command.Split(' ');

            if (args.Length < 2)
            {
                Console.WriteLine("An order ID is required.");
                return true;
            }

            string symbol = Symbol.BTC_USDT;
            if (!long.TryParse(args[1], out var id))
            {
                symbol = args[1];
                id = BinanceApi.NullId;

                if (args.Length < 3)
                {
                    Console.WriteLine("A symbol and order ID are required.");
                    return true;
                }
            }

            string clientOrderId = null;

            if (args.Length > 2)
            {
                if (!long.TryParse(args[2], out id))
                {
                    clientOrderId = args[2];
                }
                else if (id < 0)
                {
                    Console.WriteLine("An order ID not less than 0 is required.");
                    return true;
                }
            }

            if (args.Length > 3 && args[3].Equals("cancel", StringComparison.OrdinalIgnoreCase))
            {
                var cancelOrderId = clientOrderId != null
                    ? await Program.Api.CancelOrderAsync(Program.User, symbol, clientOrderId, token: token)
                    : await Program.Api.CancelOrderAsync(Program.User, symbol, id, token: token);

                lock (Program.ConsoleSync)
                {
                    Console.WriteLine();
                    Console.WriteLine($"Cancel Order ID: {cancelOrderId}");
                    Console.WriteLine();
                }
            }
            else
            {
                var order = clientOrderId != null
                    ? await Program.Api.GetOrderAsync(Program.User, symbol, clientOrderId, token: token)
                    : await Program.Api.GetOrderAsync(Program.User, symbol, id, token: token);

                lock (Program.ConsoleSync)
                {
                    Console.WriteLine();
                    if (order == null)
                    {
                        Console.WriteLine("[Not Found]");
                    }
                    else
                    {
                        Program.Display(order);
                    }
                    Console.WriteLine();
                }
            }

            return true;
        }
    }
}
