using System;
using System.Threading;
using System.Threading.Tasks;
using Binance;

namespace BinanceConsoleApp.Controllers
{
    internal class PlaceMarketOrder : IHandleCommand
    {
        public async Task<bool> HandleAsync(string command, CancellationToken token = default)
        {
            if (!command.StartsWith("market ", StringComparison.OrdinalIgnoreCase))
                return false;

            var args = command.Split(' ');

            if (args.Length < 4)
            {
                lock (Program.ConsoleSync)
                    Console.WriteLine("A side, symbol, and quantity are required.");
                return true;
            }

            if (!Enum.TryParse(typeof(OrderSide), args[1], true, out var side))
            {
                lock (Program.ConsoleSync)
                    Console.WriteLine("A valid order side is required ('buy' or 'sell').");
                return true;
            }

            var symbol = args[2];

            if (!decimal.TryParse(args[3], out var quantity) || quantity <= 0)
            {
                lock (Program.ConsoleSync)
                    Console.WriteLine("A quantity greater than 0 is required.");
                return true;
            }

            var clientOrder = new MarketOrder(Program.User)
            {
                Symbol = symbol,
                Side = (OrderSide)side,
                Quantity = quantity
            };

            if (Program.IsOrdersTestOnly)
            {
                await Program.Api.TestPlaceAsync(clientOrder, token: token);

                lock (Program.ConsoleSync)
                {
                    Console.WriteLine($"~ TEST ~ >> MARKET {clientOrder.Side} order (ID: {clientOrder.Id}) placed for {clientOrder.Quantity:0.00000000} {clientOrder.Symbol}");
                }
            }
            else
            {
                var order = await Program.Api.PlaceAsync(clientOrder, token: token);

                // ReSharper disable once InvertIf
                if (order != null)
                {
                    lock (Program.ConsoleSync)
                    {
                        Console.WriteLine($">> MARKET {order.Side} order (ID: {order.Id}) placed for {order.OriginalQuantity:0.00000000} {order.Symbol} @ {order.Price:0.00000000}");

                        foreach (var fill in order.Fills)
                        {
                            Console.WriteLine($"   {fill.Quantity:0.00000000} @ {fill.Price:0.00000000}  fee: {fill.Commission:0.00000000} {fill.CommissionAsset}  [Trade ID: {fill.TradeId}]");
                        }
                    }
                }
            }

            return true;
        }
    }
}
