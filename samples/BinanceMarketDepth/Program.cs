using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Binance;
using Binance.Application;
using Binance.Cache;
using Binance.Cache.Events;
using Binance.Stream;
using Binance.Utility;
using Binance.WebSocket;
using Binance.WebSocket.Manager;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// ReSharper disable AccessToDisposedClosure

namespace BinanceMarketDepth
{
    /// <summary>
    /// Demonstrate how to maintain an order book cache for a symbol
    /// and respond to real-time depth-of-market update events.
    /// </summary>
    internal class Program
    {
        private static async Task Main()
        {
            ExampleMain();
            //AdvancedExampleMain();

            //await MultiCacheExample.ExampleMain();
            //await MultiCacheCombinedStreamsExample.ExampleMain();
            //CombinedStreamsExample.ExampleMain();

            await Task.CompletedTask;
        }

        /// <summary>
        /// Example using cache and manager.
        /// </summary>
        private static void ExampleMain()
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

                // Get configuration settings.
                var symbols = configuration.GetSection("OrderBook:Symbols").Get<string[]>()
                    ?? new string[] { Symbol.BTC_USDT };

                var limit = 10;
                try { limit = Convert.ToInt32(configuration.GetSection("OrderBook")?["Limit"]); }
                catch { /* ignore */ }

                // NOTE: Currently the Partial Book Depth Stream only supports limits of: 5, 10, or 20.
                if (limit > 10) limit = 20;
                else if (limit > 5) limit = 10;
                else if (limit > 0) limit = 5;

                // Initialize manager.
                using (var manager = services.GetService<IDepthWebSocketCacheManager>())
                {
                    // Add error event handler.
                    manager.Error += (s, e) => Console.WriteLine(e.Exception.Message);

                    foreach (var symbol in symbols)
                    {
                        // Subscribe cache to symbol with limit and callback.
                        // NOTE: If no limit is provided (or limit = 0) then the order book is initialized with
                        //       limit = 1000 and the diff. depth stream is used to keep order book up-to-date.
                        manager.Subscribe(symbol, limit, Display);

                        lock (_sync)
                        {
                            _message = symbol == symbols.Last()
                                ? $"Symbol: \"{symbol}\" ...press any key to exit."
                                : $"Symbol: \"{symbol}\" ...press any key to continue.";
                        }
                        Console.ReadKey(true); // wait for user input.

                        manager.Unsubscribe();
                    }
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

        /// <summary>
        /// Example using cache, web socket stream (or client), and controller.
        /// </summary>
        // ReSharper disable once UnusedMember.Local
        private static void AdvancedExampleMain()
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

                // Get configuration settings.
                var limit = 10;
                var symbol = configuration.GetSection("OrderBook")?["Symbol"] ?? Symbol.BTC_USDT;
                try { limit = Convert.ToInt32(configuration.GetSection("OrderBook")?["Limit"]); }
                catch { /* ignore */ }

                // NOTE: Currently the Partial Book Depth Stream only supports limits of: 5, 10, or 20.
                if (limit > 10) limit = 20;
                else if (limit > 5) limit = 10;
                else if (limit > 0) limit = 5;

                // Initialize cache.
                var cache = services.GetService<IOrderBookCache>();
                
                // Initialize stream.
                var webSocket = services.GetService<IBinanceWebSocketStream>();

                using (var controller = new RetryTaskController(webSocket.StreamAsync))
                {
                    controller.Error += (s, e) => HandleError(e.Exception);

                    // Subscribe cache to symbol with limit and callback.
                    // NOTE: If no limit is provided (or limit = 0) then the order book is initialized with
                    //       limit = 1000 and the diff. depth stream is used to keep order book up-to-date.
                    cache.Subscribe(symbol, limit, Display);

                    // Subscribe cache to stream (with observed streams).
                    webSocket.Subscribe(cache, cache.ObservedStreams);
                    // NOTE: This must be done after cache subscribe.

                    // Begin streaming.
                    controller.Begin();

                    lock (_sync)
                    {
                        _message = "...press any key to continue.";
                    }
                    Console.ReadKey(true); // wait for user input.
                }

                //*////////////////////////////////////////////////////////
                // Alternative usage (with an existing IJsonStreamClient).
                ///////////////////////////////////////////////////////////

                // Initialize stream/client.
                var client = services.GetService<IDepthWebSocketClient>();

                cache.Client = client; // link [new] client to cache.

                // Initialize controller.
                using (var controller = new RetryTaskController(client.StreamAsync))
                {
                    controller.Error += (s, e) => HandleError(e.Exception);

                    // Subscribe cache to symbol with limit and callback.
                    //cache.Subscribe(symbol, limit, Display);
                    // NOTE: Cache is already subscribed to symbol (above).

                    // NOTE: With IJsonStreamClient, stream is automagically subscribed.

                    // Begin streaming.
                    controller.Begin();

                    lock (_sync)
                    {
                        _message = "...press any key to exit.";
                    }
                    Console.ReadKey(true); // wait for user input.
                }
                /////////////////////////////////////////////////////////*/
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine();
                Console.WriteLine("  ...press any key to close window.");
                Console.ReadKey(true);
            }
        }

        private static string _message;

        // ReSharper disable once InconsistentNaming
        private static readonly object _sync = new object();

        private static void Display(OrderBookCacheEventArgs args)
        {
            lock (_sync)
            {
                Console.SetCursorPosition(0, 0);
                args.OrderBook.Print(Console.Out, 10); // limit to 10.

                Console.WriteLine();
                Console.WriteLine(_message.PadRight(119));
            }
        }

        private static void HandleError(Exception e)
        {
            lock (_sync)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
