using Binance;
using Binance.Trades;
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
    /// Demonstrate how to maintain a local aggregate trades cache for a symbol
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
                var symbol = configuration.GetSection("TradeHistory")?["Symbol"] ?? Symbol.BTC_USDT;
                try { limit = Convert.ToInt32(configuration.GetSection("TradeHistory")?["Limit"]); } catch { }

                using (var api = services.GetService<IBinanceApi>())
                using (var cache = services.GetService<IAggregateTradesCache>())
                using (var cts = new CancellationTokenSource())
                {
                    // Query and display the latest aggregate trades for the symbol.
                    Display(await api.GetAggregateTradesAsync(symbol, limit: limit, token: cts.Token));

                    // Monitor latest aggregate trades and display updates in real-time.
                    var task = Task.Run(() =>
                        cache.SubscribeAsync(symbol, (e) => Display(e.Trades),limit, cts.Token));

                    Console.ReadKey(true); // ...press any key to exit.

                    cts.Cancel();
                    await task;
                }
            }
            catch (Exception e) { Console.WriteLine(e.Message); }
        }

        /// <summary>
        /// Display aggregate trades.
        /// </summary>
        /// <param name="trades"></param>
        private static void Display(IEnumerable<AggregateTrade> trades)
        {
            Console.SetCursorPosition(0, 0);
            foreach (var trade in trades.Reverse())
            {
                Console.WriteLine($"  {trade.Time().ToLocalTime()} - {trade.Symbol.PadLeft(8)} - {(trade.IsBuyerMaker ? "Sell" : "Buy").PadLeft(4)} - {trade.Quantity.ToString("0.00000000")} @ {trade.Price.ToString("0.00000000")}{(trade.IsBestPriceMatch ? "*" : " ")} - [ID: {trade.Id}] - {trade.Timestamp}           ");
            }
            Console.WriteLine();
            Console.WriteLine("...press any key to exit.");
        }
    }
}
