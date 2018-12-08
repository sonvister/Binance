using System;
using System.IO;
using System.Threading.Tasks;
using Binance;
using Binance.Application;
using Binance.Cache;
using Binance.Utility;
using Binance.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// ReSharper disable AccessToDisposedClosure

namespace BinanceMarketDepth
{
    /// <summary>
    /// Demonstrate how to maintain multiple order book caches
    /// and respond to real-time depth-of-market update events.
    /// </summary>
    internal class MultiCacheExample
    {
        /// <summary>
        /// Example with multiple controllers not using combined streams.
        /// </summary>
        /// <returns></returns>
        public static async Task ExampleMain()
        {
            try
            {
                // Load configuration.
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", true, false)
                    .Build();

                // Configure services.
                var services = new ServiceCollection()
                    .AddBinance() // add default Binance services.
                    .AddLogging(builder => builder // configure logging.
                        .SetMinimumLevel(LogLevel.Trace)
                        .AddFile(configuration.GetSection("Logging:File")))
                    .BuildServiceProvider();

                Console.Clear(); // clear the display.

                const int limit = 5;

                var api = services.GetService<IBinanceApi>();

                // Query and display the order books.
                var btcOrderBook = await api.GetOrderBookAsync(Symbol.BTC_USDT, limit);
                var ethOrderBook = await api.GetOrderBookAsync(Symbol.ETH_BTC, limit);
                Display(btcOrderBook, ethOrderBook);

                // Create cache.
                var btcCache = services.GetService<IOrderBookCache>();
                var ethCache = services.GetService<IOrderBookCache>();

                // Create web socket streams.
                var webSocket1 = services.GetService<IBinanceWebSocketStream>();
                var webSocket2 = services.GetService<IBinanceWebSocketStream>();
                // NOTE: IBinanceWebSocketStream must be setup as Transient with DI (default).

                // Initialize controllers.
                using (var controller1 = new RetryTaskController(webSocket1.StreamAsync))
                using (var controller2 = new RetryTaskController(webSocket2.StreamAsync))
                {
                    controller1.Error += (s, e) => HandleError(e.Exception);
                    controller2.Error += (s, e) => HandleError(e.Exception);

                    btcCache.Subscribe(Symbol.BTC_USDT, limit,
                        evt =>
                        {
                            btcOrderBook = evt.OrderBook;
                            Display(btcOrderBook, ethOrderBook);
                        });

                    ethCache.Subscribe(Symbol.ETH_BTC, limit,
                        evt =>
                        {
                            ethOrderBook = evt.OrderBook;
                            Display(btcOrderBook, ethOrderBook);
                        });

                    // Set web socket URI using cache subscribed streams.
                    webSocket1.Uri = BinanceWebSocketStream.CreateUri(btcCache);
                    webSocket2.Uri = BinanceWebSocketStream.CreateUri(ethCache);
                    // NOTE: This must be done after cache subscribe.

                    // Route stream messages to cache.
                    webSocket1.Message += (s, e) => btcCache.HandleMessage(e.Subject, e.Json);
                    webSocket2.Message += (s, e) => ethCache.HandleMessage(e.Subject, e.Json);

                    // Begin streaming.
                    controller1.Begin();
                    controller2.Begin();

                    // Verify we are NOT using a shared/combined stream (not necessary).
                    if (webSocket1.IsCombined() || webSocket2.IsCombined() || webSocket1 == webSocket2)
                        throw new Exception("You ARE using combined streams :(");

                    Console.ReadKey(true);
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine();
                Console.WriteLine("  ...press any key to close window.");
                Console.ReadKey(true);
            }
        }

        // ReSharper disable once InconsistentNaming
        private static readonly object _displaySync = new object();

        private static void Display(params OrderBook[] orderBooks)
        {
            lock (_displaySync)
            {
                Console.SetCursorPosition(0, 0);

                foreach (var orderBook in orderBooks)
                {
                    orderBook.Print(Console.Out);
                    Console.WriteLine();
                }

                Console.WriteLine("...press any key to exit.");
            }
        }

        private static void HandleError(Exception e)
        {
            lock (_displaySync)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
