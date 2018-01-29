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
    internal class MultiCacheExampleCombinedStreams
    {
        private static async Task ExampleMain()
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
                    .AddBinance()
                    // Use a single web socket stream (combined streams).
                    .AddSingleton<IWebSocketStream, BinanceWebSocketStream>()
                    .AddLogging(builder => builder.SetMinimumLevel(LogLevel.Trace))
                    .BuildServiceProvider();

                // Configure logging.
                services.GetService<ILoggerFactory>()
                    .AddFile(configuration.GetSection("Logging:File"));

                var limit = 5; // set to 0 to use diff. depth stream (instead of partial depth stream).

                var api = services.GetService<IBinanceApi>();

                var btcCache = services.GetService<IOrderBookCache>();
                var ethCache = services.GetService<IOrderBookCache>();

                using (var controller = new RetryTaskController())
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

                    // Verify we are using a shared/combined stream (not necessary).
                    if (!btcCache.Client.WebSocket.IsCombined || btcCache.Client.WebSocket != ethCache.Client.WebSocket)
                        throw new Exception(":(");

                    controller.Begin(
                        tkn => btcCache.StreamAsync(tkn),
                        err => Console.WriteLine(err.Message));

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
