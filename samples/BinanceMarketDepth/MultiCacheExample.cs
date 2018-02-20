using System;
using System.IO;
using System.Threading.Tasks;
using Binance;
using Binance.Api;
using Binance.Application;
using Binance.Cache;
using Binance.Market;
using Binance.Stream;
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
                    .AddLogging(builder => builder.SetMinimumLevel(LogLevel.Trace))
                    .BuildServiceProvider();

                // Configure logging.
                services.GetService<ILoggerFactory>()
                    .AddFile(configuration.GetSection("Logging:File"));
                    // NOTE: Using ":" requires Microsoft.Extensions.Configuration.Binder.

                Console.Clear(); // clear the display.

                var limit = 5;

                var api = services.GetService<IBinanceApi>();

                // Query and display the order books.
                var btcOrderBook = await api.GetOrderBookAsync(Symbol.BTC_USDT, limit);
                var ethOrderBook = await api.GetOrderBookAsync(Symbol.ETH_BTC, limit);
                Display(btcOrderBook, ethOrderBook);

                // Create cache.
                var btcCache = services.GetService<IOrderBookCache>();
                var ethCache = services.GetService<IOrderBookCache>();

                // Create stream.
                var webSocket1 = services.GetService<IBinanceWebSocketStream>();
                var webSocket2 = services.GetService<IBinanceWebSocketStream>();
                // NOTE: IBinanceWebSocketStream must be setup as Transient with DI (default).

                // Initialize controllers.
                using (var controller1 = new RetryTaskController(webSocket1.StreamAsync, HandleError))
                using (var controller2 = new RetryTaskController(webSocket2.StreamAsync, HandleError))
                {
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

                    // Subscribe cache to stream (with observed streams).
                    webSocket1.Subscribe(btcCache, btcCache.ObservedStreams);
                    webSocket2.Subscribe(ethCache, ethCache.ObservedStreams);
                    // NOTE: This must be done after cache subscribe.

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
