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

namespace BinancePriceChart
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
                    .AddLogging(builder => builder.SetMinimumLevel(LogLevel.Trace))
                    .BuildServiceProvider();

                // Configure logging.
                services.GetService<ILoggerFactory>()
                    .AddFile(configuration.GetSection("Logging:File"));
                    // NOTE: Using ":" requires Microsoft.Extensions.Configuration.Binder.

                // Get configuration settings.
                var symbols = configuration.GetSection("CombinedStreamsExample:Symbols").Get<string[]>()
                    ?? new string[] { Symbol.BTC_USDT };

                var interval = CandlestickInterval.Minute;
                try { interval = configuration.GetSection("PriceChart")?["Interval"].ToCandlestickInterval() ?? CandlestickInterval.Minute; }
                catch { /* ignore */ }

                // Initialize client.
                var client = services.GetService<ICandlestickClient>();

                // Initialize the stream.
                var webSocket = services.GetService<IBinanceWebSocketStream>();

                // Initialize controller.
                using (var controller = new RetryTaskController(webSocket.StreamAsync))
                {
                    controller.Error += (s, e) => HandleError(e.Exception);

                    if (symbols.Length == 1)
                    {
                        // Subscribe to symbol with callback.
                        client.Subscribe(symbols[0], interval, Display);
                    }
                    else
                    {
                        // Alternative usage (combined streams).
                        client.Candlestick += (s, evt) => { Display(evt); };

                        // Subscribe to each of the symbols.
                        foreach (var symbol in symbols)
                        {
                            client.Subscribe(symbol, interval); // using event instead of callbacks.
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
                    client.Unsubscribe(symbols[0], interval);

                    // Subscribe to the real Bitcoin :D
                    client.Subscribe(Symbol.BCH_USDT, interval); // a.k.a. BCC.

                    // Set stream URI using cache subscribed streams.
                    webSocket.Uri = BinanceWebSocketStream.CreateUri(client);
                    // NOTE: This must be done after client subscribe.

                    lock (_sync)
                    {
                        // Remove unsubscribed symbol and clear display (application specific).
                        _candlesticks.Remove(symbols[0]);
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
        private static readonly IDictionary<string, Candlestick> _candlesticks
            = new SortedDictionary<string, Candlestick>();

        private static Task _displayTask = Task.CompletedTask;

        private static void Display(CandlestickEventArgs args)
        {
            lock (_sync)
            {
                _candlesticks[args.Candlestick.Symbol] = args.Candlestick;

                if (_displayTask.IsCompleted)
                {
                    // Delay to allow multiple data updates between display updates.
                    _displayTask = Task.Delay(250)
                        .ContinueWith(_ =>
                        {
                            Candlestick[] latestCandlesticks;
                            lock (_sync)
                            {
                                latestCandlesticks = _candlesticks.Values.ToArray();
                            }

                            Console.SetCursorPosition(0, 0);

                            foreach (var c in latestCandlesticks)
                            {
                                Console.WriteLine($" {c.Symbol} - O: {c.Open:0.00000000} | H: {c.High:0.00000000} | L: {c.Low:0.00000000} | C: {c.Close:0.00000000} | V: {c.Volume:0.00} - [{c.OpenTime.ToTimestamp()}]".PadRight(119));
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
