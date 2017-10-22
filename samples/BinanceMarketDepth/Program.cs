using Binance;
using Binance.Orders.Book.Cache;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceMarketDepth
{
    class Program
    {
        private static IConfigurationRoot _configuration;
        private static int _orderBookLimit = 12;

        static async Task Main(string[] args)
        {
            var cts = new CancellationTokenSource();
            IOrderBookCache orderBook = null;

            try
            {
                _configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                    .Build();

                var services = new ServiceCollection()
                    .AddBinance()
                    .BuildServiceProvider();

                var symbol = _configuration.GetSection("OrderBook")?["Symbol"] ?? Symbol.BTC_USDT;
                try { _orderBookLimit = Convert.ToInt32(_configuration.GetSection("OrderBook")?["Limit"]); } catch { }

                orderBook = services.GetService<IOrderBookCache>();
                orderBook.Update += OnOrderBookUpdated;

                var task = Task.Run(() => orderBook.SubscribeAsync(symbol, cts.Token));

                Console.ReadKey(true); // ...press any key to exit.
                cts?.Cancel();
                await task;
            }
            catch (Exception e) { Console.WriteLine(e.Message); }
            finally
            {
                orderBook?.Dispose();
                cts?.Dispose();
            }
        }

        private static void OnOrderBookUpdated(object sender, OrderBookUpdateEventArgs e)
        {
            Console.SetCursorPosition(0, 0);
            e.OrderBook.Print(Console.Out, _orderBookLimit);

            Console.WriteLine();
            Console.WriteLine("...press any key to exit.");
        }
    }
}
