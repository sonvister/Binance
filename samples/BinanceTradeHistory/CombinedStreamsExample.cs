using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Binance;
using Binance.Application;
using Binance.Client;
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
                var client = services.GetService<IAggregateTradeClient>();

                // Initialize the stream.
                var webSocket = services.GetService<IBinanceWebSocketStream>();

                // Initialize controller.
                using (var controller = new RetryTaskController(webSocket.StreamAsync))
                {
                    controller.Error += (s, e) => HandleError(e.Exception);

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

                    // Set stream URI using cache subscribed streams.
                    webSocket.Uri = BinanceWebSocketStream.CreateUri(client);
                    // NOTE: This must be done after client subscribe.

                    // Route stream messages to client.
                    webSocket.Message += (s, e) => client.HandleMessage(e.Subject, e.Json);

                    // Begin streaming.
                    controller.Begin();

                    _message = "...press any key to continue.";
                    Console.ReadKey(true); // wait for user input.

                    //*//////////////////////////////////////////////////
                    // Example: Unsubscribe/Subscribe after streaming...
                    /////////////////////////////////////////////////////

                    // NOTE: When the URI is changed, the web socket is aborted
                    //       and a new connection is made. There's a delay
                    //       before streaming begins to allow for multiple
                    //       changes.

                    // Unsubscribe a symbol.
                    client.Unsubscribe(symbols[0]);

                    // Subscribe to the real Bitcoin :D
                    client.Subscribe(Symbol.BCH_USDT); // a.k.a. BCC.

                    // Set stream URI using cache subscribed streams.
                    webSocket.Uri = BinanceWebSocketStream.CreateUri(client);
                    // NOTE: This must be done after client subscribe.

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

        // ReSharper disable once InconsistentNaming
        private static readonly object _sync = new object();

        // ReSharper disable once InconsistentNaming
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

        private static void HandleError(Exception e)
        {
            lock (_sync)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}
