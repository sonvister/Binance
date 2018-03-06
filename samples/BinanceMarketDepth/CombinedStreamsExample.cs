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

namespace BinanceMarketDepth
{
    /// <summary>
    /// Demonstrate how to monitor order book for multiple symbols
    /// and how to unsubscribe/subscribe a symbol after streaming begins.
    /// </summary>
    internal class CombinedStreamsExample
    {
        public static async Task AdvancedExampleMain()
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

                Console.Clear(); // clear the display.

                const int limit = 5;

                // Create client.
                var client = services.GetService<IDepthClient>();

                // Initialize the stream.
                var webSocket = services.GetService<IBinanceWebSocketStream>();

                // Initialize controller.
                using (var controller = new RetryTaskController(webSocket.StreamAsync))
                {
                    controller.Error += (s, e) => HandleError(e.Exception);

                    if (symbols.Length == 1)
                    {
                        // Subscribe to symbol with callback.
                        client.Subscribe(symbols[0], Display);
                    }
                    else
                    {
                        // Alternative usage (combined streams).
                        client.DepthUpdate += (s, evt) => { Display(evt); };

                        // Subscribe to all symbols.
                        foreach (var symbol in symbols)
                        {
                            client.Subscribe(symbol, limit); // using event instead of callbacks.
                        }
                    }

                    // Set stream URI using cache subscribed streams.
                    webSocket.Uri = BinanceWebSocketStream.CreateUri(client);
                    // NOTE: This must be done after client subscribe.

                    // Route stream messages to client.
                    webSocket.Message += (s, e) => client.HandleMessage(e.Subject, e.Json);

                    // Begin streaming.
                    controller.Begin();

                    // Optionally, wait for web socket open event.
                    await webSocket.WaitUntilWebSocketOpenAsync();

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
                    client.Unsubscribe(symbols[0], limit);

                    // Subscribe to the real Bitcoin :D
                    client.Subscribe(Symbol.BCH_USDT, limit); // a.k.a. BCC.

                    // Set stream URI using cache subscribed streams.
                    webSocket.Uri = BinanceWebSocketStream.CreateUri(client);
                    // NOTE: This must be done after client subscribe.

                    lock (_sync)
                    {
                        // Remove unsubscribed symbol and clear display (application specific).
                        _orderBookTops.Remove(symbols[0]);
                        Console.Clear();
                    }

                    // Optionally, wait for web socket open event.
                    await webSocket.WaitUntilWebSocketOpenAsync();

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
        private static readonly IDictionary<string, OrderBookTop> _orderBookTops
            = new SortedDictionary<string, OrderBookTop>();

        private static Task _displayTask = Task.CompletedTask;

        private static void Display(DepthUpdateEventArgs args)
        {
            var orderBookTop = OrderBookTop.Create(args.Symbol, args.Bids.First(), args.Asks.First());

            lock (_sync)
            {
                _orderBookTops[orderBookTop.Symbol] = orderBookTop;

                if (_displayTask.IsCompleted)
                {
                    // Delay to allow multiple data updates between display updates.
                    _displayTask = Task.Delay(250)
                        .ContinueWith(_ =>
                        {
                            OrderBookTop[] latestTops;
                            lock (_sync)
                            {
                                latestTops = _orderBookTops.Values.ToArray();
                            }

                            Console.SetCursorPosition(0, 0);

                            foreach (var t in latestTops)
                            {
                                Console.WriteLine($" {t.Symbol.PadLeft(8)}  -  Bid: {t.Bid.Price.ToString("0.00000000").PadLeft(13)} (qty: {t.Bid.Quantity})   |   Ask: {t.Ask.Price.ToString("0.00000000").PadLeft(13)} (qty: {t.Ask.Quantity})".PadRight(119));
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
