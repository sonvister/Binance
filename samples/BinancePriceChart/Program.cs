using System;
using System.IO;
using System.Linq;
using Binance;
using Binance.Application;
using Binance.Cache;
using Binance.Cache.Events;
using Binance.Client;
using Binance.Market;
using Binance.Utility;
using Binance.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

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

                    // Use alternative, low-level, web socket client implementation.
                    //.AddTransient<IWebSocketClient, WebSocket4NetClient>()
                    //.AddTransient<IWebSocketClient, WebSocketSharpClient>()

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
                catch { /* ignore */ }

                var limit = 25;
                try { limit = Convert.ToInt32(configuration.GetSection("PriceChart")?["Limit"] ?? "25"); }
                catch { /* ignore */ }

                // Initialize cache.
                var cache = services.GetService<ICandlestickWebSocketCache>();

                // Add error event handler.
                cache.Error += (s, e) => Console.WriteLine(e.Exception.Message);

                // Subscribe cache to symbol and interval with limit and callback.
                cache.Subscribe(symbol, interval, limit, Display);

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
                catch { /* ignore */ }

                var limit = 25;
                try { limit = Convert.ToInt32(configuration.GetSection("PriceChart")?["Limit"] ?? "25"); }
                catch { /* ignore */ }

                // Initialize cache.
                var cache = services.GetService<ICandlestickCache>();

                // Initialize stream.
                var webSocket = services.GetService<IBinanceWebSocketStream>();

                // Initialize controller.
                using (var controller = new RetryTaskController(webSocket.StreamAsync))
                {
                    controller.Error += (s, e) => HandleError(e.Exception);

                    // Subscribe cache to symbol and interval with limit and callback.
                    cache.Subscribe(symbol, interval, limit, Display);

                    // Set web socket URI using cache subscribed streams.
                    webSocket.Uri = BinanceWebSocketStream.CreateUri(cache);
                    // NOTE: This must be done after cache subscribe.

                    // Route stream messages to cache.
                    webSocket.Message += (s, e) => cache.HandleMessage(e.Subject, e.Json);

                    // Begin streaming.
                    controller.Begin();

                    lock (_sync) _message = "...press any key to continue.";
                    Console.ReadKey(true); // wait for user input.
                }

                //*/////////////////////////////////////////
                // Alternative usage (with an IJsonClient).
                ////////////////////////////////////////////

                // Initialize publisher/client.
                var client = services.GetService<ICandlestickClient>();

                cache.Client = client; // link [new] client to cache.

                // Initialize controller.
                using (var controller = new RetryTaskController(webSocket.StreamAsync))
                {
                    controller.Error += (s, e) => HandleError(e.Exception);

                    // Subscribe cache to symbol and interval with limit and callback.
                    //cache.Subscribe(symbol, interval, limit, Display);
                    // NOTE: Cache is already subscribed to symbol (above).

                    // Begin streaming.
                    controller.Begin();

                    lock (_sync) _message = "(alternative usage) ...press any key to exit.";
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
