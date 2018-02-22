using System;
using System.IO;
using System.Linq;
using Binance;
using Binance.Application;
using Binance.Cache;
using Binance.Cache.Events;
using Binance.Market;
using Binance.Utility;
using Binance.Stream;
using Binance.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Binance.WebSocket.Manager;

// ReSharper disable AccessToDisposedClosure

namespace BinancePriceChart
{
    /// <summary>
    /// Demonstrate how to maintain an aggregate trades cache for a symbol
    /// and respond to real-time aggregate trade events.
    /// </summary>
    internal class Program
    {
        private static void Main()
        {
            ExampleMain();
            //AdvancedExampleMain();
            //CombinedStreamsExample.ExampleMain();
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

                // Get configuration settings.
                var symbol = configuration.GetSection("PriceChart")?["Symbol"] ?? Symbol.BTC_USDT;

                var interval = CandlestickInterval.Minute;
                try { interval = configuration.GetSection("PriceChart")?["Interval"].ToCandlestickInterval() ?? CandlestickInterval.Minute; }
                catch { /* ignored */ }

                var limit = 25;
                try { limit = Convert.ToInt32(configuration.GetSection("PriceChart")?["Limit"] ?? "25"); }
                catch { /* ignored */ }

                // Initialize manager.
                using (var manager = services.GetService<ICandlestickWebSocketClientManager>())
                {
                    // Initialize cache.
                    var cache = services.GetService<ICandlestickCache>();
                    cache.Client = manager; // use manager as client.

                    // Subscribe cache to symbol and interval with limit and callback.
                    cache.Subscribe(symbol, interval, limit, Display);

                    _message = "...press any key to continue.";
                    Console.ReadKey(true); // wait for user input.
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

                // Get configuration settings.
                var symbol = configuration.GetSection("PriceChart")?["Symbol"] ?? Symbol.BTC_USDT;

                var interval = CandlestickInterval.Minute;
                try { interval = configuration.GetSection("PriceChart")?["Interval"].ToCandlestickInterval() ?? CandlestickInterval.Minute; }
                catch { /* ignored */ }

                var limit = 25;
                try { limit = Convert.ToInt32(configuration.GetSection("PriceChart")?["Limit"] ?? "25"); }
                catch { /* ignored */ }

                // Initialize cache.
                var cache = services.GetService<ICandlestickCache>();

                // Initialize stream.
                var webSocket = services.GetService<IBinanceWebSocketStream>();

                // Initialize controller.
                using (var controller = new RetryTaskController(webSocket.StreamAsync, HandleError))
                {
                    // Subscribe cache to symbol and interval with limit and callback.
                    cache.Subscribe(symbol, interval, limit, Display);

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
                var client = services.GetService<ICandlestickWebSocketClient>();

                cache.Client = client; // link [new] client to cache.

                // Initialize controller.
                using (var controller = new RetryTaskController(client.StreamAsync, HandleError))
                {
                    // Subscribe cache to symbol and interval with limit and callback.
                    //cache.Subscribe(symbol, interval, limit, Display);
                    // NOTE: Cache is already subscribed to symbol (above).

                    // NOTE: With IJsonStreamClient, stream is automagically subscribed.

                    // Begin streaming.
                    controller.Begin();

                    _message = "...press any key to exit.";
                    Console.ReadKey(true); // wait for user input.
                }
                /////////////////////////////////////////////////////////////////*/
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

        private static void Display(CandlestickCacheEventArgs args)
        {
            lock (_sync)
            {
                Console.SetCursorPosition(0, 0);
                foreach (var candlestick in args.Candlesticks.Reverse())
                {
                    Console.WriteLine($"  {candlestick.Symbol} - O: {candlestick.Open:0.00000000} | H: {candlestick.High:0.00000000} | L: {candlestick.Low:0.00000000} | C: {candlestick.Close:0.00000000} | V: {candlestick.Volume:0.00} - [{candlestick.OpenTime.ToTimestamp()}]".PadRight(119));
                }
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
