using System;
using System.Linq;
using System.Threading.Tasks;
using Binance;
using Binance.Account.Orders;
using Binance.Api;
using Binance.Cache;
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

            /*
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
                    order.Validate();

                    await api.TestPlaceAsync(order);

                    Console.WriteLine("Test Order Successful!");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Test Order Failed: \"{e.Message}\"");
                }
            }
            */

            using (var webSocketManager =  new AggregateTradeWebSocketClientManager())
            {
                webSocketManager.Error += (s, e) => { Console.WriteLine(e.Exception.Message); };

                webSocketManager.Subscribe(Symbol.BTC_USDT, evt =>
                {
                    var side = evt.Trade.IsBuyerMaker ? "SELL" : "BUY ";

                    Console.WriteLine($"{evt.Trade.Symbol} {side} {evt.Trade.Quantity} @ {evt.Trade.Price}");
                });

                Console.ReadKey(true);
            }

            using (var webSocketManager = new DepthWebSocketCacheManager())
            {
                webSocketManager.Error += (s, e) => { Console.WriteLine(e.Exception.Message); };

                webSocketManager.Subscribe(Symbol.BTC_USDT, evt =>
                {
                    Symbol symbol = evt.OrderBook.Symbol; // use implicit conversion.

                    var minBidPrice = evt.OrderBook.Bids.Last().Price;
                    var maxAskPrice = evt.OrderBook.Asks.Last().Price;

                    Console.WriteLine($"Bid Quantity: {evt.OrderBook.Depth(minBidPrice)} {symbol.BaseAsset} - " +
                                      $"Ask Quantity: {evt.OrderBook.Depth(maxAskPrice)} {symbol.BaseAsset}");
                });

                Console.ReadKey(true);
            }
        }
    }
}
