using Binance;
using Binance.Api;
using Binance.Cache;
using Binance.Market;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceTradeHistory
{
    /// <summary>
    /// Demonstrate how to maintain an aggregate trades cache for a symbol
    /// and respond to real-time aggregate trade events.
    /// </summary>
    class Program
    {
        static async Task Main(string[] args)
        {
            try
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
                var limit = 25;
                var symbol = configuration.GetSection("PriceChart")?["Symbol"] ?? Symbol.BTC_USDT;
                try { limit = Convert.ToInt32(configuration.GetSection("PriceChart")?["Limit"]); } catch { }

                using (var api = services.GetService<IBinanceApi>())
                using (var cache = services.GetService<ICandlesticksCache>())
                using (var cts = new CancellationTokenSource())
                {
                    var interval = KlineInterval.Minute;

                    // Query and display the latest aggregate trades for the symbol.
                    Display(await api.GetCandlesticksAsync(symbol, interval, limit: limit, token: cts.Token));

                    // Monitor latest aggregate trades and display updates in real-time.
                    var task = Task.Run(() =>
                        cache.SubscribeAsync(symbol, interval, (e) => Display(e.Candlesticks), limit, cts.Token));

                    Console.ReadKey(true); // ...press any key to exit.

                    cts.Cancel();
                    await task;
                }
            }
            catch (Exception e) { Console.WriteLine(e.Message); }
        }

        private static void Display(IEnumerable<Candlestick> candlesticks)
        {
            Console.SetCursorPosition(0, 0);
            foreach (var candlestick in candlesticks.Reverse())
            {
                Console.WriteLine($"  {candlestick.Symbol} - O: {candlestick.Open.ToString("0.00000000")} | H: {candlestick.High.ToString("0.00000000")} | L: {candlestick.Low.ToString("0.00000000")} | C: {candlestick.Close.ToString("0.00000000")} | V: {candlestick.Volume.ToString("0.00")} - [{candlestick.OpenTime}]");
            }
            Console.WriteLine();
            Console.WriteLine("...press any key to exit.");
        }
    }
}
