using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Binance;

namespace BinanceConsoleApp.Controllers
{
    internal class IntegrationTests : IHandleCommand
    {
        public async Task<bool> HandleAsync(string command, CancellationToken token = default)
        {
            if (!command.StartsWith("run ", StringComparison.OrdinalIgnoreCase) &&
                !command.Equals("run", StringComparison.OrdinalIgnoreCase))
                return false;

            var symbol = Symbol.BTC_USDT;
            var endTime = DateTime.UtcNow.Subtract(TimeSpan.FromDays(1));


            ///////////////////////////////////////////////////////////////////
            var valid = symbol.IsPriceQuantityValid(5000.01m, 0.1m);
            var _valid = symbol.IsPriceQuantityValid(50000.01m, 0.1m);
            var __valid = symbol.IsPriceQuantityValid(50.01m, 0.1m);

            lock (Program.ConsoleSync)
            {
                Console.WriteLine();
                Console.WriteLine($"Price/Quantity Valid: {valid}");
                Console.WriteLine($"Price/Quantity Valid: {_valid}");
                Console.WriteLine($"Price/Quantity Valid: {__valid}");
            }
            ///////////////////////////////////////////////////////////////////


            /*/////////////////////////////////////////////////////////////////
            var price = await Program.Api.GetAvgPriceAsync(symbol, token);

            lock (Program.ConsoleSync)
            {
                Console.WriteLine();
                Program.Display(price);
            }
            /////////////////////////////////////////////////////////////////*/


            /*/////////////////////////////////////////////////////////////////
            var aggTrades = (await Program.Api.GetAggregateTradesAsync(symbol, endTime.Subtract(TimeSpan.FromMinutes(1)), endTime, token))
                .Reverse().ToArray();

            lock (Program.ConsoleSync)
            {
                Console.WriteLine();
                Console.WriteLine($"Aggregate Trades ({symbol}):");

                // ReSharper disable once PossibleMultipleEnumeration
                if (!aggTrades.Any())
                {
                    Console.WriteLine("  [None]");
                }
                else
                {
                    // ReSharper disable once PossibleMultipleEnumeration
                    foreach (var trade in aggTrades)
                    {
                        Program.Display(trade);
                    }
                }
            }
            /////////////////////////////////////////////////////////////////*/


            /*/////////////////////////////////////////////////////////////////
            var trades = (await Program.Api.GetAccountTradesAsync(Program.User, symbol, endTime.Subtract(TimeSpan.FromHours(24)), endTime, token: token))
                .Reverse().ToArray();

            lock (Program.ConsoleSync)
            {
                Console.WriteLine();
                Console.WriteLine($"Account Trades ({symbol}):");

                // ReSharper disable once PossibleMultipleEnumeration
                if (!trades.Any())
                {
                    Console.WriteLine("  [None]");
                }
                else
                {
                    // ReSharper disable once PossibleMultipleEnumeration
                    foreach (var trade in trades)
                    {
                        Program.Display(trade);
                    }
                }
            }
            /////////////////////////////////////////////////////////////////*/


            /*/////////////////////////////////////////////////////////////////
            var orders = await Program.Api
                .GetOrdersAsync(Program.User, symbol, endTime.Subtract(TimeSpan.FromHours(24)), endTime, token: token);

            lock (Program.ConsoleSync)
            {
                Console.WriteLine();
                Console.WriteLine($"Orders ({symbol}):");

                // ReSharper disable once PossibleMultipleEnumeration
                if (!orders.Any())
                {
                    Console.WriteLine("  [None]");
                }
                else
                {
                    // ReSharper disable once PossibleMultipleEnumeration
                    foreach (var order in orders)
                    {
                        Program.Display(order);
                    }
                }
            }
            /////////////////////////////////////////////////////////////////*/


            return true;
        }
    }
}
