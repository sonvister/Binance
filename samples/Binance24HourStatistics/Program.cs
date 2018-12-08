using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Binance;
using Binance.Application;
using Binance.Cache;
using Binance.Client;
using Binance.Utility;
using Binance.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// ReSharper disable AccessToDisposedClosure

namespace Binance24HourStatistics
{
    /// <summary>
    /// Demonstrate how to maintain a 24-hour statistics cache for multiple
    /// symbols and respond to real-time 24-hour statistics update events.
    /// </summary>
    internal class Program
    {
        private static Task Main()
        {
            ExampleMain();
            //AdvancedExampleMain();
            //CombinedStreamsExample.AdvancedExampleMain();
            //AllSymbolsExample.ExampleMain();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Example using cache and manager.
        /// </summary>
        /// <returns></returns>
        private static void ExampleMain()
        {
            try
            {
                // Load configuration.
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", false, false)
                    .Build();

                // Configure services.
                var services = new ServiceCollection()
                    .AddBinance() // add default Binance services.
                    .AddLogging(builder => builder // configure logging.
                        .SetMinimumLevel(LogLevel.Trace)
                        .AddFile(configuration.GetSection("Logging:File")))

                    // Use alternative, low-level, web socket client implementation.
                    //.AddTransient<IWebSocketClient, WebSocket4NetClient>()
                    //.AddTransient<IWebSocketClient, WebSocketSharpClient>()

                    .BuildServiceProvider();

                // Get configuration settings.
                var symbols = configuration.GetSection("Statistics:Symbols").Get<string[]>()
                    ?? new string[] { Symbol.BTC_USDT };

                // Initialize cache.
                var cache = services.GetService<ISymbolStatisticsWebSocketCache>();

                // Add error event handler.
                cache.Error += (s, e) => Console.WriteLine(e.Exception.Message);

                try
                {
                    // Subscribe cache to symbols (automatically begin streaming).
                    cache.Subscribe(Display, symbols);

                    Console.ReadKey(true); // wait for user input.
                }
                finally
                {
                    // Unsubscribe cache (automatically end streaming).
                    cache.Unsubscribe();
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
        /// <returns></returns>
        // ReSharper disable once UnusedMember.Local
        private static void AdvancedExampleMain()
        {
            try
            {
                // Load configuration.
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", false, false)
                    .Build();

                // Configure services.
                var services = new ServiceCollection()
                    .AddBinance() // add default Binance services.
                    .AddLogging(builder => builder // configure logging.
                        .SetMinimumLevel(LogLevel.Trace)
                        .AddFile(configuration.GetSection("Logging:File")))
                    .BuildServiceProvider();

                // Get configuration settings.
                var symbols = configuration.GetSection("Statistics:Symbols").Get<string[]>()
                    ?? new string[] { Symbol.BTC_USDT };

                // Initialize cache.
                var cache = services.GetService<ISymbolStatisticsCache>();
                
                // Initialize web socket stream.
                var webSocket = services.GetService<IBinanceWebSocketStream>();

                using (var controller = new RetryTaskController(webSocket.StreamAsync))
                {
                    controller.Error += (s, e) => HandleError(e.Exception);

                    // Subscribe cache to symbols.
                    cache.Subscribe(Display, symbols);

                    // Set web socket URI using cache subscribed streams.
                    webSocket.Uri = BinanceWebSocketStream.CreateUri(cache);
                    // NOTE: This must be done after cache subscribe.

                    // Route stream messages to cache.
                    webSocket.Message += (s, e) => cache.HandleMessage(e.Subject, e.Json);

                    // Begin streaming.
                    controller.Begin();

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

        // ReSharper disable once UnusedMember.Local
        private static async Task<SymbolStatistics[]> Get24HourStatisticsAsync(IBinanceApi api, params string[] symbols)
        {
            var statistics = new List<SymbolStatistics>();

            foreach (var symbol in symbols)
            {
                statistics.Add(await api.Get24HourStatisticsAsync(symbol));
            }

            return statistics.ToArray();
        }

        // ReSharper disable once InconsistentNaming
        private static readonly object _sync = new object();

        // ReSharper disable once UnusedMember.Local
        private static void Display(SymbolStatisticsEventArgs args)
            => Display(args.Statistics);

        private static void Display(SymbolStatisticsCacheEventArgs args)
            => Display(args.Statistics);

        private static void Display(SymbolStatistics[] statistics)
        {
            lock (_sync)
            {
                Console.SetCursorPosition(0, 0);

                foreach (var stats in statistics)
                {
                    Console.WriteLine($"  24-hour statistics for {stats.Symbol}:");
                    Console.WriteLine($"    %: {stats.PriceChangePercent:0.00} | O: {stats.OpenPrice:0.00000000} | H: {stats.HighPrice:0.00000000} | L: {stats.LowPrice:0.00000000} | V: {stats.Volume:0.}");
                    Console.WriteLine($"    Bid: {stats.BidPrice:0.00000000} | Last: {stats.LastPrice:0.00000000} | Ask: {stats.AskPrice:0.00000000} | Avg: {stats.WeightedAveragePrice:0.00000000}");
                    Console.WriteLine();
                }

                Console.WriteLine("...press any key to exit.");
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
