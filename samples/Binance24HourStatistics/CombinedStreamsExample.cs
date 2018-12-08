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

namespace Binance24HourStatistics
{
    /// <summary>
    /// Demonstrate how to monitor candlesticks for multiple symbols
    /// and how to unsubscribe/subscribe a symbol after streaming begins.
    /// </summary>
    internal class CombinedStreamsExample
    {
        public static void AdvancedExampleMain()
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
                    .AddLogging(builder => builder // configure logging.
                        .SetMinimumLevel(LogLevel.Trace)
                        .AddFile(configuration.GetSection("Logging:File")))
                    .BuildServiceProvider();

                // Get configuration settings.
                var symbols = configuration.GetSection("CombinedStreamsExample:Symbols").Get<string[]>()
                    ?? new string[] { Symbol.BTC_USDT };

                // Initialize the client.
                var client = services.GetService<ISymbolStatisticsClient>();

                // Initialize the stream.
                var webSocket = services.GetService<IBinanceWebSocketStream>();

                // Initialize controller.
                using (var controller = new RetryTaskController(webSocket.StreamAsync))
                {
                    controller.Error += (s, e) => HandleError(e.Exception);

                    if (symbols.Length == 1)
                    {
                        // Subscribe to symbol with callback.
                        client.Subscribe(Display, symbols[0]);
                    }
                    else
                    {
                        // Alternative usage (combined streams).
                        client.StatisticsUpdate += (s, evt) => { Display(evt); };

                        // Subscribe to all symbols.
                        foreach (var symbol in symbols)
                        {
                            client.Subscribe(symbol); // using event instead of callbacks.
                        }
                    }

                    // Set stream URI using client subscribed streams.
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

                    // Subscribe to XRP.
                    client.Subscribe(Symbol.XRP_USDT); // drink the kool-aid.

                    // Set stream URI using client subscribed streams.
                    webSocket.Uri = BinanceWebSocketStream.CreateUri(client);
                    // NOTE: This must be done after client subscribe.

                    lock (_sync)
                    {
                        // Remove unsubscribed symbol and clear display (application specific).
                        _statistics.Remove(symbols[0]);
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
        private static readonly IDictionary<string, SymbolStatistics> _statistics
            = new SortedDictionary<string, SymbolStatistics>();

        private static Task _displayTask = Task.CompletedTask;

        private static void Display(SymbolStatisticsEventArgs args)
        {
            lock (_sync)
            {
                _statistics[args.Statistics[0].Symbol] = args.Statistics[0];

                if (_displayTask.IsCompleted)
                {
                    // Delay to allow multiple data updates between display updates.
                    _displayTask = Task.Delay(250)
                        .ContinueWith(_ =>
                        {
                            SymbolStatistics[] latestStatistics;
                            lock (_sync)
                            {
                                latestStatistics = _statistics.Values.ToArray();
                            }

                            Console.SetCursorPosition(0, 0);

                            foreach (var stats in latestStatistics)
                            {
                                Console.WriteLine($"  24-hour statistics for {stats.Symbol}:".PadRight(119));
                                Console.WriteLine($"    %: {stats.PriceChangePercent:0.00} | O: {stats.OpenPrice:0.00000000} | H: {stats.HighPrice:0.00000000} | L: {stats.LowPrice:0.00000000} | V: {stats.Volume:0.}".PadRight(119));
                                Console.WriteLine($"    Bid: {stats.BidPrice:0.00000000} | Last: {stats.LastPrice:0.00000000} | Ask: {stats.AskPrice:0.00000000} | Avg: {stats.WeightedAveragePrice:0.00000000}".PadRight(119));
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
