using System;
using System.Linq;
using System.Threading.Tasks;
using Binance;
using Binance.Cache;
using Binance.WebSocket;

// ReSharper disable once CheckNamespace
namespace BinanceConsoleApp
{
    public class ReadMeExample
    {
        public static async Task ExampleMain(string[] args)
        {
            // Initialize REST API client.
            var api = new BinanceApi();

            // Check connectivity.
            if (await api.PingAsync())
            {
                Console.WriteLine("Successful!");
            }


            // Create user with API-Key and API-Secret.
            using (var user = new BinanceApiUser("<API-Key>", "<API-Secret>"))
            {
                // Create a client (MARKET) order.
                var order = new MarketOrder(user)
                {
                    Symbol = Symbol.BTC_USDT,
                    Side = OrderSide.Buy,
                    Quantity = 0.01m
                };

                try
                {
                    // Validate client order.
                    order.Validate();

                    // Send the TEST order.
                    await api.TestPlaceAsync(order);

                    Console.WriteLine("Test Order Successful!");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Test Order Failed: \"{e.Message}\"");
                }
            }


            // Initialize web socket client (with automatic streaming enabled).
            var webSocketClient = new AggregateTradeWebSocketClient();

            // Handle error events.
            webSocketClient.Error += (s, e) => { Console.WriteLine(e.Exception.Message); };

            // Subscribe callback to BTC/USDT (automatically begin streaming).
            webSocketClient.Subscribe(Symbol.BTC_USDT, evt =>
            {
                var side = evt.Trade.IsBuyerMaker ? "SELL" : "BUY ";
                
                // Handle aggregate trade events.
                Console.WriteLine($"{evt.Trade.Symbol} {side} {evt.Trade.Quantity} @ {evt.Trade.Price}");
            });

            Console.ReadKey(true); // wait for user input.

            // Unsubscribe from symbol (automatically end streaming).
            webSocketClient.Unsubscribe();


            // Initiatlize web socket cache (with automatic streaming enabled).
            var webSocketCache = new DepthWebSocketCache();

            // Handle error events.
            webSocketCache.Error += (s, e) => { Console.WriteLine(e.Exception.Message); };

            // Subscribe callback to symbol (automatically begin streaming).
            webSocketCache.Subscribe(Symbol.BTC_USDT, evt =>
            {
                // Get symbol from cache (update cache if a symbol is missing).
                var symbol = Symbol.Cache.Get(evt.OrderBook.Symbol);

                var minBidPrice = evt.OrderBook.Bids.Last().Price;
                var maxAskPrice = evt.OrderBook.Asks.Last().Price;

                // Handle order book update events.
                Console.WriteLine($"Bid Quantity: {evt.OrderBook.Depth(minBidPrice)} {symbol.BaseAsset} - " +
                                  $"Ask Quantity: {evt.OrderBook.Depth(maxAskPrice)} {symbol.BaseAsset}");
            });

            Console.ReadKey(true); // wait for user input.

            // Unsubscribe from symbol (automatically end streaming).
            webSocketCache.Unsubscribe();
        }
    }
}
