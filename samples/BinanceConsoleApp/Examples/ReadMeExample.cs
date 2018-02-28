using System;
using System.Threading.Tasks;
using Binance;
using Binance.Account.Orders;
using Binance.Api;
using Binance.WebSocket.Manager;

namespace BinanceConsoleApp
{
    public class ReadMeExample
    {
        public static async Task ExampleMain(string[] args)
        {
            var api = new BinanceApi();

            if (await api.PingAsync())
            {
                Console.WriteLine("Successful!");
            }

            using (var user = new BinanceApiUser("<API-Key>", "<API-Secret>"))
            {
                var order = new MarketOrder(user)
                {
                    Symbol = Symbol.BTC_USDT,
                    Side = OrderSide.Buy,
                    Quantity = 0.01m
                };

                try
                {
                    await api.TestPlaceAsync(order);
                    Console.WriteLine("Test Order Successful!");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Test Order Failed: \"{e.Message}\"");
                }
            }

            using (var webSocketManager =  new AggregateTradeWebSocketClientManager())
            {
                webSocketManager.Controller.Error += (s, e) => { Console.WriteLine(e.Exception.Message); };

                webSocketManager.Subscribe(Symbol.BTC_USDT, evt =>
                {
                    var side = evt.Trade.IsBuyerMaker ? "SELL" : "BUY ";
                    Console.WriteLine($"{evt.Trade.Symbol} {side} {evt.Trade.Quantity} @ {evt.Trade.Price}");
                });

                Console.ReadKey(true);
            }
        }
    }
}
