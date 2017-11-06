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

namespace BinancePriceChart
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
                var symbol = configuration.GetSection("PriceChart")?["Symbol"] ?? Symbol.BTC_USDT;
                try { limit = Convert.ToInt32(configuration.GetSection("PriceChart")?["Limit"]); }
                catch { /* ignored */ }

                using (var api = services.GetService<IBinanceApi>())
                using (var cts = new CancellationTokenSource())
                {
                    const CandlestickInterval interval = CandlestickInterval.Minute;

                    // Query and display the latest aggregate trades for the symbol.
                    Display(await api.GetCandlesticksAsync(symbol, interval, limit, token: cts.Token));

                    // Monitor latest aggregate trades and display updates in real-time.
                    // ReSharper disable once MethodSupportsCancellation
                    var task = Task.Run(async () =>
                    {
                        while (!cts.IsCancellationRequested)
                        {
                            using (var cache = services.GetService<ICandlesticksCache>())
                            {
                                try
                                {
                                    await cache.SubscribeAsync(symbol, interval, e => Display(e.Candlesticks), limit, cts.Token);
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

                    Console.ReadKey(true); // ...press any key to exit.

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

        private static void Display(IEnumerable<Candlestick> candlesticks)
        {
            Console.SetCursorPosition(0, 0);
            foreach (var candlestick in candlesticks.Reverse())
            {
                Console.WriteLine($"  {candlestick.Symbol} - O: {candlestick.Open:0.00000000} | H: {candlestick.High:0.00000000} | L: {candlestick.Low:0.00000000} | C: {candlestick.Close:0.00000000} | V: {candlestick.Volume:0.00} - [{candlestick.OpenTime}]");
            }
            Console.WriteLine();
            Console.WriteLine("...press any key to exit.");
        }
    }
}
