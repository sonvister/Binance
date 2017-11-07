using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Binance;
using Binance.Api;
using Binance.Application;
using Binance.Cache;
using Binance.Market;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// ReSharper disable AccessToDisposedClosure

namespace BinanceTradeHistory
{
    /// <summary>
    /// Demonstrate how to maintain an aggregate trades cache for a symbol
    /// and respond to real-time aggregate trade events.
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
                    .AddJsonFile("appsettings.json", true, false)
                    .Build();

                // Configure services.
                var services = new ServiceCollection()
                    .AddBinance()
                    .AddLogging(builder => builder.SetMinimumLevel(LogLevel.Trace))
                    .BuildServiceProvider();

                // Configure logging.
                services.GetService<ILoggerFactory>()
                    .AddFile(configuration.GetSection("Logging").GetSection("File"));

                // Get configuration settings.
                var limit = 25;
                var symbol = configuration.GetSection("TradeHistory")?["Symbol"] ?? Symbol.BTC_USDT;
                try { limit = Convert.ToInt32(configuration.GetSection("TradeHistory")?["Limit"]); }
                catch { /* ignored */ }

                using (var api = services.GetService<IBinanceApi>())
                using (var cts = new CancellationTokenSource())
                {
                    // Query and display the latest aggregate trades for the symbol.
                    Display(await api.GetAggregateTradesAsync(symbol, limit, cts.Token));

                    // Monitor latest aggregate trades and display updates in real-time.
                    // ReSharper disable once MethodSupportsCancellation
                    var task = Task.Run(async () =>
                    {
                        while (!cts.IsCancellationRequested)
                        {
                            using (var cache = services.GetService<IAggregateTradesCache>())
                            {
                                try
                                {
                                    await cache.SubscribeAsync(symbol, e => Display(e.Trades), limit, cts.Token);
                                }
                                catch (OperationCanceledException) { }
                                catch (Exception e)
                                {
                                    Console.WriteLine(e.Message);
                                    await Task.Delay(5000, cts.Token); // ...wait a bit.
                                }
                            }
                        }
                    });

                    Console.ReadKey(true);

                    cts.Cancel();
                    await task;
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

        private static void Display(IEnumerable<AggregateTrade> trades)
        {
            Console.SetCursorPosition(0, 0);
            foreach (var trade in trades.Reverse())
            {
                Console.WriteLine($"  {trade.Time().ToLocalTime()} - {trade.Symbol.PadLeft(8)} - {(trade.IsBuyerMaker ? "Sell" : "Buy").PadLeft(4)} - {trade.Quantity:0.00000000} @ {trade.Price:0.00000000}{(trade.IsBestPriceMatch ? "*" : " ")} - [ID: {trade.Id}] - {trade.Timestamp}           ");
            }
            Console.WriteLine();
            Console.WriteLine("...press any key to exit.");
        }
    }
}
