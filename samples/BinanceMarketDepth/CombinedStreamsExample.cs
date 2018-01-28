using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Binance;
using Binance.Application;
using Binance.Cache;
using Binance.Market;
using Binance.Utility;
using Binance.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// ReSharper disable AccessToDisposedClosure

namespace BinanceMarketDepth
{
    /// <summary>
    /// Demonstrate how to monitor order book for multiple symbols
    /// and how to unsubscribe/subscribe a symbol after streaming begins.
    /// </summary>
    internal class CombinedStreamsExample
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
                    .AddFile(configuration.GetSection("Logging:File"));

                // Get configuration settings.
                var symbols = configuration.GetSection("CombinedStreamsExample:Symbols").Get<string[]>()
                    ?? new string[] { Symbol.BTC_USDT };

                var limit = 5;

                var client = services.GetService<IDepthWebSocketClient>();

                using (var controller = new RetryTaskController())
                {
                    if (symbols.Length == 1)
                    {
                        // Monitor latest candlestick for a symbol and display.
                        controller.Begin(
                            tkn => client.StreamAsync(symbols[0], evt => Display(OrderBookTop.Create(evt.Symbol, evt.Bids.First(), evt.Asks.First())), tkn),
                            err => Console.WriteLine(err.Message));
                    }
                    else
                    {
                        // Alternative usage (combined streams).
                        client.DepthUpdate += (s, evt) => { Display(OrderBookTop.Create(evt.Symbol, evt.Bids.First(), evt.Asks.First())); };

                        // Subscribe to all symbols.
                        foreach (var symbol in symbols)
                        {
                            client.Subscribe(symbol, limit); // using event instead of callbacks.
                        }

                        // Begin streaming.
                        controller.Begin(
                            tkn => client.WebSocket.StreamAsync(tkn),
                            err => Console.WriteLine(err.Message));
                    }

                    message = "...press any key to continue.";
                    Console.ReadKey(true); // wait for user input.

                    //*//////////////////////////////////////////////////
                    // Example: Unsubscribe/Subscribe after streaming...

                    // Cancel streaming.
                    await controller.CancelAsync();

                    // Unsubscribe a symbol.
                    client.Unsubscribe(symbols[0], limit);

                    // Remove unsubscribed symbol and clear display (application specific).
                    _orderBookTops.Remove(symbols[0]);
                    Console.Clear();

                    // Subscribe to the real Bitcoin :D
                    client.Subscribe(Symbol.BCH_USDT, limit); // a.k.a. BCC.

                    // Begin streaming again.
                    controller.Begin(
                        tkn => client.WebSocket.StreamAsync(tkn),
                        err => Console.WriteLine(err.Message));

                    message = "...press any key to exit.";
                    Console.ReadKey(true); // wait for user input.
                    ///////////////////////////////////////////////////*/
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

        private static string message;

        private static readonly object _sync = new object();

        private static IDictionary<string, OrderBookTop> _orderBookTops
            = new SortedDictionary<string, OrderBookTop>();

        private static void Display(OrderBookTop orderBookTop)
        {
            lock (_sync)
            {
                Console.SetCursorPosition(0, 0);

                _orderBookTops[orderBookTop.Symbol] = orderBookTop;

                foreach (var keyPair in _orderBookTops)
                {
                    var top = keyPair.Value;
                    Console.WriteLine($" {top.Symbol.PadLeft(8)}  -  Bid: {top.Bid.Price.ToString("0.00000000").PadLeft(13)} (qty: {top.Bid.Quantity})   |   Ask: {top.Ask.Price.ToString("0.00000000").PadLeft(13)} (qty: {top.Ask.Quantity})");
                    Console.WriteLine();
                }

                Console.WriteLine(message);
            }
        }
    }
}
