using Binance;
using Binance.Api.WebSocket;
using Binance.Api.WebSocket.Events;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceConsoleApp.Controllers
{
    public class LiveUserData : IHandleCommand
    {
        public Task<bool> HandleAsync(string command, CancellationToken token = default)
        {
            if (!command.StartsWith("live ", StringComparison.OrdinalIgnoreCase))
                return Task.FromResult(false);

            var args = command.Split(' ');

            string endpoint = "";
            if (args.Length > 1)
            {
                endpoint = args[1];
            }

            string symbol = Symbol.BTC_USDT;
            if (args.Length > 2)
            {
                symbol = args[2];
            }

            if (!endpoint.Equals("account", StringComparison.OrdinalIgnoreCase)
                && !endpoint.Equals("user", StringComparison.OrdinalIgnoreCase))
                return Task.FromResult(false);

            if (Program._liveTask != null)
            {
                lock (Program._consoleSync)
                {
                    Console.WriteLine($"! A live task is currently active ...use 'live off' to disable.");
                }
                return Task.FromResult(true);
            }

            if (Program._user == null)
            {
                Program.PrintApiNotice();
                return Task.FromResult(true);
            }

            Program._liveTokenSource = new CancellationTokenSource();

            Program._userDataClient = Program._serviceProvider.GetService<IUserDataWebSocketClient>();
            Program._userDataClient.AccountUpdate += OnAccountUpdateEvent;
            Program._userDataClient.OrderUpdate += OnOrderUpdateEvent;
            Program._userDataClient.TradeUpdate += OnTradeUpdateEvent;

            Program._liveTask = Task.Run(() =>
            {
                Program._userDataClient.SubscribeAsync(Program._user, Program._liveTokenSource.Token);
            });

            lock (Program._consoleSync)
            {
                Console.WriteLine();
                Console.WriteLine($"  ...live account feed enabled ...use 'live off' to disable.");
            }

            return Task.FromResult(true);
        }

        private void OnAccountUpdateEvent(object sender, AccountUpdateEventArgs e)
        {
            Program.Display(e.Account);
        }

        private void OnOrderUpdateEvent(object sender, OrderUpdateEventArgs e)
        {
            lock (Program._consoleSync)
            {
                Console.WriteLine();
                Console.WriteLine($"Order [{e.Order.Id}] update: {e.OrderExecutionType}");
                Program.Display(e.Order);
                Console.WriteLine();
            }
        }

        private void OnTradeUpdateEvent(object sender, TradeUpdateEventArgs e)
        {
            lock (Program._consoleSync)
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
