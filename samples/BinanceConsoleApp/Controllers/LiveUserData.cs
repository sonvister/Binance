using System;
using System.Threading;
using System.Threading.Tasks;
using Binance.Client.Events;

namespace BinanceConsoleApp.Controllers
{
    internal class LiveUserData : IHandleCommand
    {
        public async Task<bool> HandleAsync(string command, CancellationToken token = default)
        {
            if (!command.StartsWith("live ", StringComparison.OrdinalIgnoreCase))
                return false;

            var args = command.Split(' ');

            var endpoint = string.Empty;
            if (args.Length > 1)
            {
                endpoint = args[1];
            }

            if (!endpoint.Equals("account", StringComparison.OrdinalIgnoreCase)
                && !endpoint.Equals("user", StringComparison.OrdinalIgnoreCase))
                return false;

            var enable = true;
            if (args.Length > 2)
            {
                if (args[2].Equals("off", StringComparison.OrdinalIgnoreCase))
                    enable = false;
            }

            if (enable)
            {
                await Program.UserDataManager.SubscribeAsync<AccountUpdateEventArgs>(Program.User, HandleAccountUpdateEvent);
                await Program.UserDataManager.SubscribeAsync<OrderUpdateEventArgs>(Program.User, HandleOrderUpdateEvent);
                await Program.UserDataManager.SubscribeAsync<AccountTradeUpdateEventArgs>(Program.User, HandleTradeUpdateEvent);

                lock (Program.ConsoleSync)
                {
                    Console.WriteLine();
                    Console.WriteLine($"  ...live user data feed ENABLED.");
                    Console.WriteLine();
                }
            }
            else // disable.
            {
                await Program.UserDataManager.UnsubscribeAsync<AccountUpdateEventArgs>(Program.User, HandleAccountUpdateEvent);
                await Program.UserDataManager.UnsubscribeAsync<OrderUpdateEventArgs>(Program.User, HandleOrderUpdateEvent);
                await Program.UserDataManager.UnsubscribeAsync<AccountTradeUpdateEventArgs>(Program.User, HandleTradeUpdateEvent);

                lock (Program.ConsoleSync)
                {
                    Console.WriteLine();
                    Console.WriteLine($"  ...live user data feed DISABLED.");
                    Console.WriteLine();
                }
            }

            return true;
        }

        private static void HandleAccountUpdateEvent(AccountUpdateEventArgs e)
        {
            Program.Display(e.AccountInfo);
        }

        private static void HandleOrderUpdateEvent(OrderUpdateEventArgs e)
        {
            lock (Program.ConsoleSync)
            {
                Console.WriteLine();
                Console.WriteLine($"Order [{e.Order.Id}] update: {e.OrderExecutionType}");
                Program.Display(e.Order);
                Console.WriteLine();
            }
        }

        private static void HandleTradeUpdateEvent(AccountTradeUpdateEventArgs e)
        {
            lock (Program.ConsoleSync)
            {
                Console.WriteLine();
                Console.WriteLine($"Order [{e.Order.Id}] update: {e.OrderExecutionType}");
                Program.Display(e.Order);
                Console.WriteLine();
                Program.Display(e.Trade);
                Console.WriteLine();
            }
        }
    }
}
