using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Binance;
using Binance.Api;
using Binance.Application;
using Binance.Cache;
using Binance.Market;
using Binance.Utility;
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
        private static async Task Main()
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
                    .AddBinance()
                    .AddLogging(builder => builder.SetMinimumLevel(LogLevel.Trace))
                    .BuildServiceProvider();

                // Configure logging.
                services.GetService<ILoggerFactory>()
                    .AddFile(configuration.GetSection("Logging:File"));

                // Get configuration settings.
                var symbols = configuration.GetSection("Statistics:Symbols").Get<string[]>() ?? new string[] { Symbol.BTC_USDT };

                var cache = services.GetService<ISymbolStatisticsCache>();

                using (var controller = new RetryTaskController())
                {
                    var api = services.GetService<IBinanceApi>();

                    // Query and display the 24-hour statistics.
                    Display(await Get24HourStatisticsAsync(api, symbols));

                    // Monitor 24-hour statistics of a symbol and display updates in real-time.
                    if (symbols.Length == 1)
                    {
                        controller.Begin(
                            tkn => cache.SubscribeAndStreamAsync(symbols[0], evt => Display(evt.Statistics), tkn),
                            err => Console.WriteLine(err.Message));
                    }
                    else
                    {
                        // Alternative usage (if sharing IBinanceWebSocket for combined streams).
                        cache.Subscribe(evt => Display(evt.Statistics), symbols);
                        controller.Begin(
                            tkn => cache.StreamAsync(tkn),
                            err => Console.WriteLine(err.Message));
                    }

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

        private static async Task<SymbolStatistics[]> Get24HourStatisticsAsync(IBinanceApi api, params string[] symbols)
        {
            var statistics = new List<SymbolStatistics>();

            foreach (var symbol in symbols)
            {
                statistics.Add(await api.Get24HourStatisticsAsync(symbol));
            }

            return statistics.ToArray();
        }

        private static void Display(params SymbolStatistics[] statistics)
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
}
