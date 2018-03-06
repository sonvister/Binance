using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Binance;
using Binance.Application;
using Binance.Cache;
using Binance.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// ReSharper disable AccessToDisposedClosure

namespace Binance24HourStatistics
{
    /// <summary>
    /// Demonstrate how to monitor all symbol statistics
    /// and display symbols with highest % price change.
    /// </summary>
    internal class AllSymbolsExample
    {
        public static void ExampleMain()
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

                // Initialize cache (w/ internal controller).
                var cache = services.GetService<ISymbolStatisticsWebSocketCache>();

                // Add error event handler.
                cache.Error += (s, e) => Console.WriteLine(e.Exception.Message);

                try
                {
                    // Subscribe cache to symbols (automatically begin streaming).
                    cache.Subscribe(Display);

                    _message = "...press any key to exit.";
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

        private static string _message;

        // ReSharper disable once InconsistentNaming
        private static readonly object _sync = new object();

        private static Task _displayTask = Task.CompletedTask;

        private static void Display(SymbolStatisticsCacheEventArgs args)
        {
            lock (_sync)
            {
                if (_displayTask.IsCompleted)
                {
                    // Delay to allow multiple data updates between display updates.
                    _displayTask = Task.Delay(250)
                        .ContinueWith(_ =>
                        {
                            SymbolStatistics[] latestStatistics = args.Statistics;

                            Console.SetCursorPosition(0, 0);

                            // Display top 5 symbols with highest % price change.
                            foreach (var stats in latestStatistics.OrderBy(s => s.PriceChangePercent).Reverse().Take(5))
                            {
                                Console.WriteLine($"  24-hour statistics for {stats.Symbol}:".PadRight(119));
                                Console.WriteLine($"    %: {stats.PriceChangePercent:0.00} | O: {stats.OpenPrice:0.00000000} | H: {stats.HighPrice:0.00000000} | L: {stats.LowPrice:0.00000000} | V: {stats.Volume:0.}".PadRight(119));
                                Console.WriteLine($"    Bid: {stats.BidPrice:0.00000000} | Last: {stats.LastPrice:0.00000000} | Ask: {stats.AskPrice:0.00000000} | Avg: {stats.WeightedAveragePrice:0.00000000}".PadRight(119));
                                Console.WriteLine();
                            }

                            Console.WriteLine(_message.PadRight(119));
                        });
                }
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
