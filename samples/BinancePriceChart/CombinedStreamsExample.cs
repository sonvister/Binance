using System;
using System.Collections.Generic;
using System.IO;
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

namespace BinanceTradeHistory
{
    /// <summary>
    /// Demonstrate how to monitor aggregate trades for multiple symbols
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

                var interval = CandlestickInterval.Minute;
                try { interval = configuration.GetSection("PriceChart")?["Interval"].ToCandlestickInterval() ?? CandlestickInterval.Minute; }
                catch { /* ignored */ }

                var client = services.GetService<ICandlestickWebSocketClient>();

                using (var controller = new RetryTaskController())
                {
                    if (symbols.Length == 1)
                    {
                        // Monitor latest candlestick for a symbol and display.
                        controller.Begin(
                            tkn => client.StreamAsync(symbols[0], interval, evt => Display(evt.Candlestick), tkn),
                            err => Console.WriteLine(err.Message));
                    }
                    else
                    {
                        // Alternative usage (combined streams).
                        client.Candlestick += (s, evt) => { Display(evt.Candlestick); };

                        // Subscribe to all symbols.
                        foreach (var symbol in symbols)
                        {
                            client.Subscribe(symbol, interval); // using event instead of callbacks.
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
                    client.Unsubscribe(symbols[0], interval);

                    // Remove unsubscribed symbol and clear display (application specific).
                    _candlesticks.Remove(symbols[0]);
                    Console.Clear();

                    // Subscribe to the real Bitcoin :D
                    client.Subscribe(Symbol.BCH_USDT, interval); // a.k.a. BCC.

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

        private static IDictionary<string, Candlestick> _candlesticks
            = new SortedDictionary<string, Candlestick>();

        private static void Display(Candlestick candlestick)
        {
            lock (_sync)
            {
                Console.SetCursorPosition(0, 0);

                _candlesticks[candlestick.Symbol] = candlestick;

                foreach (var keyPair in _candlesticks)
                {
                    candlestick = keyPair.Value;
                    Console.WriteLine($" {candlestick.Symbol} - O: {candlestick.Open:0.00000000} | H: {candlestick.High:0.00000000} | L: {candlestick.Low:0.00000000} | C: {candlestick.Close:0.00000000} | V: {candlestick.Volume:0.00} - [{candlestick.OpenTime.ToTimestamp()}]".PadRight(119));
                    Console.WriteLine();
                }

                Console.WriteLine(message);
            }
        }
    }
}
