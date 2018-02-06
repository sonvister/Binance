using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Binance;
using Binance.Application;
using Binance.Cache;
using Binance.Market;
using Binance.Utility;
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
            ExampleMain(); await Task.CompletedTask;

            //await CombinedStreamsExample.ExampleMain();
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
                    .AddBinance()
                    .AddLogging(builder => builder.SetMinimumLevel(LogLevel.Trace))
                    .BuildServiceProvider();

                // Configure logging.
                services.GetService<ILoggerFactory>()
                    .AddFile(configuration.GetSection("Logging").GetSection("File"));

                // Get configuration settings.
                var symbol = configuration.GetSection("PriceChart")?["Symbol"] ?? Symbol.BTC_USDT;

                var interval = CandlestickInterval.Minute;
                try { interval = configuration.GetSection("PriceChart")?["Interval"].ToCandlestickInterval() ?? CandlestickInterval.Minute; }
                catch { /* ignored */ }

                var limit = 25;
                try { limit = Convert.ToInt32(configuration.GetSection("PriceChart")?["Limit"] ?? "25"); }
                catch { /* ignored */ }

                var cache = services.GetService<ICandlestickCache>();

                Func<CancellationToken, Task> action;
                action = tkn => cache.SubscribeAndStreamAsync(symbol, interval, limit, evt => Display(evt.Candlesticks), tkn);
                //action = tkn => cache.StreamAsync(tkn);

                using (var controller = new RetryTaskController(action, err => Console.WriteLine(err.Message)))
                {
                    // Monitor latest aggregate trades and display updates in real-time.
                    controller.Begin();

                    // Alternative usage (if sharing IBinanceWebSocket for combined streams).
                    //cache.Subscribe(symbol, interval, limit, evt => Display(evt.Candlesticks));
                    //controller.Begin();

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

        private static void Display(IEnumerable<Candlestick> candlesticks)
        {
            Console.SetCursorPosition(0, 0);
            foreach (var candlestick in candlesticks.Reverse())
            {
                Console.WriteLine($"  {candlestick.Symbol} - O: {candlestick.Open:0.00000000} | H: {candlestick.High:0.00000000} | L: {candlestick.Low:0.00000000} | C: {candlestick.Close:0.00000000} | V: {candlestick.Volume:0.00} - [{candlestick.OpenTime.ToTimestamp()}]");
            }
            Console.WriteLine();
            Console.WriteLine("...press any key to exit.");
        }
    }
}
