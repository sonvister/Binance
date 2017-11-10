using System;
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
                var limit = 10;
                var symbol = configuration.GetSection("OrderBook")?["Symbol"] ?? Symbol.BTC_USDT;
                try { limit = Convert.ToInt32(configuration.GetSection("OrderBook")?["Limit"]); }
                catch { /* ignored */ }
                // NOTE: Currently the Depth WebSocket Endpoint/Client only supports maximum limit of 100.

                var cache = services.GetService<IOrderBookCache>();

                using (var controller = new RetryTaskController())
                using (var api = services.GetService<IBinanceApi>())
                {
                    // Query and display the order book.
                    Display(await api.GetOrderBookAsync(symbol, limit));

                    // Monitor order book and display updates in real-time.
                    controller.Begin(
                        tkn => cache.SubscribeAsync(symbol, limit, e => Display(e.OrderBook), tkn),
                        err => Console.WriteLine(err.Message));

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

        private static void Display(OrderBook orderBook)
        {
            Console.SetCursorPosition(0, 0);
            orderBook.Print(Console.Out);

            Console.WriteLine();
            Console.WriteLine("...press any key to exit.");
        }
    }
}
