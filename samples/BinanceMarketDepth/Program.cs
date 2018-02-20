using System;
using System.IO;
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
                var limit = 10;
                var symbol = configuration.GetSection("OrderBook")?["Symbol"] ?? Symbol.BTC_USDT;
                try { limit = Convert.ToInt32(configuration.GetSection("OrderBook")?["Limit"]); }
                catch { /* ignored */ }

                // NOTE: Currently the Partial Book Depth Stream only supports limits of: 5, 10, or 20.
                if (limit > 10) limit = 20;
                else if (limit > 5) limit = 10;
                else if (limit > 0) limit = 5;

                // Initialize manager.
                var manager = services.GetService<IDepthWebSocketClientManager>();

                // Initialize cache.
                var cache = services.GetService<IOrderBookCache>();
                cache.Client = manager;

                // Subscribe cache to symbol with limit and callback.
                // NOTE: If no limit is provided (or limit = 0) then the order book is initialized with
                //       limit = 1000 and the diff. depth stream is used to keep order book up-to-date.
                cache.Subscribe(symbol, limit, Display);

                _message = "...press any key to continue.";
                Console.ReadKey(true); // wait for user input.
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                Console.WriteLine();
                Console.WriteLine("  ...press any key to close window.");
                Console.ReadKey(true);
            }
        }

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
                catch { /* ignored */ }

                // NOTE: Currently the Partial Book Depth Stream only supports limits of: 5, 10, or 20.
                if (limit > 10) limit = 20;
                else if (limit > 5) limit = 10;
                else if (limit > 0) limit = 5;

                // Initialize cache.
                var cache = services.GetService<IOrderBookCache>();
                
                // Initialize stream.
                var webSocket = services.GetService<IBinanceWebSocketStream>();

                using (var controller = new RetryTaskController(
                    tkn => webSocket.StreamAsync(tkn),
                    err => Console.WriteLine(err.Message)))
                {
                    // Subscribe cache to symbol with limit and callback.
                    // NOTE: If no limit is provided (or limit = 0) then the order book is initialized with
                    //       limit = 1000 and the diff. depth stream is used to keep order book up-to-date.
                    cache.Subscribe(symbol, limit, Display);

                    // Subscribe cache to stream (with observed streams).
                    webSocket.Subscribe(cache, cache.ObservedStreams);
                    // NOTE: This must be done after cache subscribe.

                    // Begin streaming.
                    controller.Begin();

                    _message = "...press any key to continue.";
                    Console.ReadKey(true); // wait for user input.
                }

                //*////////////////////////////////////////////////////////
                // Alternative usage (with an existing IJsonStreamClient).
                ///////////////////////////////////////////////////////////

                // Initialize stream/client.
                var client = services.GetService<IDepthWebSocketClient>();

                cache.Client = client; // link [new] client to cache.

                // Initialize controller.
                using (var controller = new RetryTaskController(
                    tkn => client.StreamAsync(tkn),
                    err => Console.WriteLine(err.Message)))
                {
                    // Subscribe cache to symbol with limit and callback.
                    //cache.Subscribe(symbol, limit, Display);
                    // NOTE: Cache is already subscribed to symbol (above).

                    // NOTE: With IJsonStreamClient, stream is automagically subscribed.

                    // Begin streaming.
                    controller.Begin();

                    _message = "...press any key to exit.";
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

        private static void Display(OrderBookCacheEventArgs args)
        {
            Console.SetCursorPosition(0, 0);
            args.OrderBook.Print(Console.Out, 10); // limit to 10.

            Console.WriteLine();
            Console.WriteLine(_message.PadRight(119));
        }
    }
}
