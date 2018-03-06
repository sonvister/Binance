﻿using System;
using System.Linq;
using System.Threading.Tasks;
using Binance;
using Binance.Account.Orders;
using Binance.Api;
using Binance.Cache;
using Binance.WebSocket;

namespace BinanceConsoleApp
{
    public class ReadMeExample
    {
        public static async Task ExampleMain(string[] args)
        {
            // Initialize REST API.
            var api = new BinanceApi();

            // Check connectivity.
            if (await api.PingAsync())
            {
                Console.WriteLine("Successful!");
            }


            // Initialize user with API-Key and API-Secret (optional).
            using (var user = new BinanceApiUser("<API-Key>", "<API-Secret>"))
            {
                // Initialize a new client (MARKET) order.
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

                    // Place TEST order.
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

            // Add web socket controller error handler.
            webSocketClient.Error += (s, e) => { Console.WriteLine(e.Exception.Message); };

            // Subscribe callback to symbol (automatically begin streaming).
            webSocketClient.Subscribe(Symbol.BTC_USDT, evt =>
            {
                var side = evt.Trade.IsBuyerMaker ? "SELL" : "BUY ";

                Console.WriteLine($"{evt.Trade.Symbol} {side} {evt.Trade.Quantity} @ {evt.Trade.Price}");
            });

            Console.ReadKey(true); // wait for user input.

            // Unsubscribe from symbol (automatically end streaming).
            webSocketClient.Unsubscribe();


            // Initiatlize web socket cache (with automatic streaming enabled).
            var webSocketCache = new DepthWebSocketCache();

            // Add web socket controller error handler.
            webSocketCache.Error += (s, e) => { Console.WriteLine(e.Exception.Message); };

            // Subscribe callback to symbol (automatically begin streaming).
            webSocketCache.Subscribe(Symbol.BTC_USDT, evt =>
            {
                Symbol symbol = evt.OrderBook.Symbol; // use implicit conversion.

                var minBidPrice = evt.OrderBook.Bids.Last().Price;
                var maxAskPrice = evt.OrderBook.Asks.Last().Price;

                Console.WriteLine($"Bid Quantity: {evt.OrderBook.Depth(minBidPrice)} {symbol.BaseAsset} - " +
                                  $"Ask Quantity: {evt.OrderBook.Depth(maxAskPrice)} {symbol.BaseAsset}");
            });

            Console.ReadKey(true); // wait for user input.

            // Unsubscribe from symbol (automatically end streaming).
            webSocketCache.Unsubscribe();
        }
    }
}
