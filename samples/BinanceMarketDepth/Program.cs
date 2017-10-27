using Binance;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceMarketDepth
{
    /// <summary>
    /// Demonstrate how to maintain a local order book cache for a symbol
    /// and respond to real-time order book update events.
    /// </summary>
    class Program
    {
        static async Task Main(string[] args)
        {
            // Load configuration.
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                .Build();

            // Configure services.
            var services = new ServiceCollection()
                .AddBinance().BuildServiceProvider();

            // Get configuration settings.
            var orderBookLimit = 12;
            var symbol = configuration.GetSection("OrderBook")?["Symbol"] ?? Symbol.BTC_USDT;
            try { orderBookLimit = Convert.ToInt32(configuration.GetSection("OrderBook")?["Limit"]); } catch { }
            // NOTE: Currently the Depth WebSocket Endpoint/Client only supports maximum limit of 100.

            using (var cache = services.GetService<IOrderBookCache>())
            using (var cts = new CancellationTokenSource())
            {
                var task = Task.Run(() => cache.SubscribeAsync(symbol, (e) =>
                {
                    // Display the updated order book.
                    Console.SetCursorPosition(0, 0);
                    e.OrderBook.Print(Console.Out, orderBookLimit);

                    Console.WriteLine();
                    Console.WriteLine("...press any key to exit.");
                }, cts.Token));

                Console.ReadKey(true); // ...press any key to exit.

                cts.Cancel();
                await task;
            }
        }
    }
}
