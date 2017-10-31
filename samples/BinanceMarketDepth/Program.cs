using Binance;
using Binance.Api;
using Binance.Cache;
using Binance.Market;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
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
                    .AddBinance().BuildServiceProvider();

                // Get configuration settings.
                var limit = 10;
                var symbol = configuration.GetSection("OrderBook")?["Symbol"] ?? Symbol.BTC_USDT;
                try { limit = Convert.ToInt32(configuration.GetSection("OrderBook")?["Limit"]); }
                catch { /* ignored */ }
                // NOTE: Currently the Depth WebSocket Endpoint/Client only supports maximum limit of 100.

                using (var api = services.GetService<IBinanceApi>())
                using (var cache = services.GetService<IOrderBookCache>())
                using (var cts = new CancellationTokenSource())
                {
                    // Query and display the order book.
                    Display(await api.GetOrderBookAsync(symbol, limit, cts.Token));

                    // Monitor order book and display updates in real-time.
                    // ReSharper disable once MethodSupportsCancellation
                    var task = Task.Run(() =>
                        cache.SubscribeAsync(symbol, (e) => Display(e.OrderBook), limit, cts.Token));

                    Console.ReadKey(true); // ...press any key to exit.

                    cts.Cancel();
                    await task;
                }
            }
            catch (Exception e) { Console.WriteLine(e.Message); }
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
