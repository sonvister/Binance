using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Binance;
using Binance.Application;
using Binance.Client;
using Binance.Client.Events;
using Binance.Market;
using Binance.Utility;
using Binance.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// ReSharper disable AccessToDisposedClosure

namespace BinanceTradeHistory
{
    /// <summary>
    /// Demonstrate how to monitor aggregate trades for multiple symbols
    /// and how to unsubscribe/subscribe a symbol after streaming begins.
    /// </summary>
    internal class CombinedStreamsExample
    {
        public static void ExampleMain()
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
                    .AddBinance() // add default Binance services.
                    .AddLogging(builder => builder.SetMinimumLevel(LogLevel.Trace))
                    .BuildServiceProvider();

                // Configure logging.
                services.GetService<ILoggerFactory>()
                    .AddFile(configuration.GetSection("Logging:File"));
                    // NOTE: Using ":" requires Microsoft.Extensions.Configuration.Binder.

                // Get configuration settings.
                var symbols = configuration.GetSection("CombinedStreamsExample:Symbols").Get<string[]>()
                    ?? new string[] { Symbol.BTC_USDT };

                // Initialize client.
                var client = services.GetService<IAggregateTradeWebSocketClient>();

                // Initialize controller.
                using (var controller = new RetryTaskController(
                    tkn => client.StreamAsync(tkn),
                    err => Console.WriteLine(err.Message)))
                {
                    if (symbols.Length == 1)
                    {
                        // Subscribe client to symbol with callback.
                        client.Subscribe(symbols[0], Display);
                    }
                    else
                    {
                        // Alternative usage (combined streams).
                        client.AggregateTrade += (s, evt) => { Display(evt); };

                        // Subscribe to all symbols.
                        foreach (var symbol in symbols)
                        {
                            client.Subscribe(symbol); // use event handler instead of callbacks.
                        }
                    }

                    // Begin streaming.
                    controller.Begin();

                    _message = "...press any key to continue.";
                    Console.ReadKey(true); // wait for user input.

                    //*//////////////////////////////////////////////////
                    // Example: Unsubscribe/Subscribe after streaming...
                    /////////////////////////////////////////////////////

                    // NOTE: When stream names are subscribed/unsubscribed, the
                    //       websocket is aborted and a new connection is made.
                    //       There is a small delay before streaming retarts to
                    //       allow for multiple subscribe/unsubscribe changes.

                    // Unsubscribe a symbol.
                    client.Unsubscribe(symbols[0]);

                    // Subscribe to the real Bitcoin :D
                    client.Subscribe(Symbol.BCH_USDT); // a.k.a. BCC.

                    // Remove unsubscribed symbol and clear display (application specific).
                    lock (_sync)
                    {
                        _trades.Remove(symbols[0]);
                        Console.Clear();
                    }

                    _message = "...press any key to exit.";
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

        private static string _message;

        private static readonly object _sync = new object();

        private static readonly IDictionary<string, AggregateTrade> _trades
            = new SortedDictionary<string, AggregateTrade>();

        private static Task _displayTask = Task.CompletedTask;

        private static void Display(AggregateTradeEventArgs args)
        {
            var trade = args.Trade;

            lock (_sync)
            {
                _trades[trade.Symbol] = trade;

                if (_displayTask.IsCompleted)
                {
                    // Delay to allow multiple data updates between display updates.
                    _displayTask = Task.Delay(250)
                        .ContinueWith(_ =>
                        {
                            AggregateTrade[] latestsTrades;
                            lock (_sync)
                            {
                                latestsTrades = _trades.Values.ToArray();
                            }

                            Console.SetCursorPosition(0, 0);

                            foreach (var t in latestsTrades)
                            {
                                Console.WriteLine($" {t.Time.ToLocalTime()} - {t.Symbol.PadLeft(8)} - {(t.IsBuyerMaker ? "Sell" : "Buy").PadLeft(4)} - {t.Quantity:0.00000000} @ {t.Price:0.00000000}{(t.IsBestPriceMatch ? "*" : " ")} - [ID: {t.Id}] - {t.Time.ToTimestamp()}".PadRight(119));
                                Console.WriteLine();
                            }

                            Console.WriteLine(_message.PadRight(119));
                        });
                }
            }
        }
    }
}
