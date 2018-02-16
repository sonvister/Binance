using System;
using System.IO;
using System.Threading.Tasks;
using Binance;
using Binance.Api;
using Binance.Application;
using Binance.Cache;
using Binance.Market;
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
    internal class MultiCacheCombinedStreamsExample
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

                    // Use a single web socket stream (combined streams).
                    .AddSingleton<IBinanceWebSocketStream, BinanceWebSocketStream>()

                    .AddLogging(builder => builder.SetMinimumLevel(LogLevel.Trace))
                    .BuildServiceProvider();

                // Configure logging.
                services.GetService<ILoggerFactory>()
                    .AddFile(configuration.GetSection("Logging:File"));
                    // NOTE: Using ":" requires Microsoft.Extensions.Configuration.Binder.

                Console.Clear(); // clear the display.

                var limit = 5; // set to 0 to use diff. depth stream (instead of partial depth stream).

                var api = services.GetService<IBinanceApi>();

                // Create cache.
                var btcCache = services.GetService<IOrderBookCache>();
                var ethCache = services.GetService<IOrderBookCache>();

                // Create stream.
                var webSocket = services.GetService<IBinanceWebSocketStream>();

                // Initialize controller.
                using (var controller = new RetryTaskController(
                    tkn => webSocket.StreamAsync(tkn),
                    err => Console.WriteLine(err.Message)))
                {
                    // Query and display the order books.
                    var btcOrderBook = await api.GetOrderBookAsync(Symbol.BTC_USDT, limit);
                    var ethOrderBook = await api.GetOrderBookAsync(Symbol.ETH_BTC, limit);
                    Display(btcOrderBook, ethOrderBook);

                    ///////////////////////////////////////////////////////////////////////////
                    // Use one controller and a singleton IBinanceWebSocket (combined streams).
                    // NOTE: IWebSocketStream must be configured as Singleton in DI setup.

                    // Monitor order book and display updates in real-time.
                    btcCache.Subscribe(Symbol.BTC_USDT, limit,
                        evt =>
                        {
                            btcOrderBook = evt.OrderBook;
                            Display(btcOrderBook, ethOrderBook);
                        });

                    // Monitor order book and display updates in real-time.
                    ethCache.Subscribe(Symbol.ETH_BTC, limit,
                        evt =>
                        {
                            ethOrderBook = evt.OrderBook;
                            Display(btcOrderBook, ethOrderBook);
                        });

                    // Subscribe cache to stream (with observed streams).
                    webSocket.Subscribe(btcCache);
                    webSocket.Subscribe(ethCache);
                    // NOTE: This must be done after cache subscribe.

                    // Verify we are using a shared/combined stream (not necessary).
                    if (!webSocket.IsCombined())
                        throw new Exception("You are NOT using combined streams :(");

                    // Begin streaming.
                    controller.Begin();

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
                    orderBook.Print(Console.Out, 5);
                    Console.WriteLine();
                }

                Console.WriteLine("...press any key to exit.");
            }
        }
    }
}
