using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Binance;
using Binance.Application;
using Binance.Cache;
using Binance.Market;
using Binance.Utility;
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
        private static void Main()
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
                    .AddFile(configuration.GetSection("Logging:File"));

                // Get configuration settings.
                var limit = 25;
                var symbol = configuration.GetSection("TradeHistory")?["Symbol"] ?? Symbol.BTC_USDT;
                try { limit = Convert.ToInt32(configuration.GetSection("TradeHistory")?["Limit"]); }
                catch { /* ignored */ }

                var cache = services.GetService<IAggregateTradeCache>();

                using (var controller = new RetryTaskController())
                {
                    // Monitor latest aggregate trades and display updates in real-time.
                    controller.Begin(
                        tkn => cache.StreamAsync(symbol, limit, evt => Display(evt.Trades), tkn),
                        err => Console.WriteLine(err.Message));

                    // Alternative usage (if sharing IBinanceWebSocket for combined streams).
                    //cache.Subscribe(symbol, limit, evt => Display(evt.Trades));
                    //controller.Begin(
                    //    tkn => cache.Client.WebSocket.StreamAsync(tkn),
                    //    err => Console.WriteLine(err.Message));

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

        private static void Display(IEnumerable<AggregateTrade> trades)
        {
            Console.SetCursorPosition(0, 0);
            foreach (var trade in trades.Reverse())
            {
                Console.WriteLine($"  {trade.Time.ToLocalTime()} - {trade.Symbol.PadLeft(8)} - {(trade.IsBuyerMaker ? "Sell" : "Buy").PadLeft(4)} - {trade.Quantity:0.00000000} @ {trade.Price:0.00000000}{(trade.IsBestPriceMatch ? "*" : " ")} - [ID: {trade.Id}] - {trade.Time.ToTimestamp()}         ");
            }
            Console.WriteLine();
            Console.WriteLine("...press any key to exit.");
        }
    }
}
