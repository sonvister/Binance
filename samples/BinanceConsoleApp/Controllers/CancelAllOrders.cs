using System;
using System.Threading;
using System.Threading.Tasks;
using Binance;

// ReSharper disable PossibleMultipleEnumeration

namespace BinanceConsoleApp.Controllers
{
    internal class CancelAllOrders : IHandleCommand
    {
        public async Task<bool> HandleAsync(string command, CancellationToken token = default)
        {
            if (!command.StartsWith("cancel ", StringComparison.OrdinalIgnoreCase) &&
                !command.Equals("cancel", StringComparison.OrdinalIgnoreCase))
                return false;

            if (Program.User == null)
            {
                Program.PrintApiNotice();
                return true;
            }

            var args = command.Split(' ');

            string symbol = null;
            if (args.Length > 1)
            {
                symbol = args[1];
            }

            lock (Program.ConsoleSync)
                Console.WriteLine("  Canceling all open orders...");

            await Program.Api.CancelAllOrdersAsync(Program.User, symbol, token: token);

            lock (Program.ConsoleSync)
            {
                Console.WriteLine("  Done (all open orders canceled).");
                Console.WriteLine();
            }

            return true;
        }
    }
}
