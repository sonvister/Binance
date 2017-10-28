using System;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceConsoleApp.Controllers
{
    public class GetOrCancelOrder : IHandleCommand
    {
        public async Task<bool> HandleAsync(string command, CancellationToken token = default)
        {
            if (!command.StartsWith("order ", StringComparison.OrdinalIgnoreCase))
                return false;

            var args = command.Split(' ');

            if (args.Length < 3)
            {
                Console.WriteLine("A symbol and order ID are required.");
                return true;
            }

            var symbol = args[1];

            string clientOrderId = null;

            if (!long.TryParse(args[2], out var id))
            {
                clientOrderId = args[2];
            }
            else if (id < 0)
            {
                Console.WriteLine("An order ID not less than 0 is required.");
                return true;
            }

            if (args.Length > 3 && args[3].Equals("cancel", StringComparison.OrdinalIgnoreCase))
            {
                var cancelOrderId = clientOrderId != null
                    ? await Program._api.CancelOrderAsync(Program._user, symbol, clientOrderId, token: token)
                    : await Program._api.CancelOrderAsync(Program._user, symbol, id, token: token);

                lock (Program._consoleSync)
                {
                    Console.WriteLine();
                    Console.WriteLine($"Cancel Order ID: {cancelOrderId}");
                    Console.WriteLine();
                }
            }
            else
            {
                var order = clientOrderId != null
                    ? await Program._api.GetOrderAsync(Program._user, symbol, clientOrderId, token: token)
                    : await Program._api.GetOrderAsync(Program._user, symbol, id, token: token);

                lock (Program._consoleSync)
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
