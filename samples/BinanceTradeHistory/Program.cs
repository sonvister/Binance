using System;
using System.IO;
using System.Linq;
using Binance;
using Binance.Api;
using Binance.Application;
using Binance.Cache;
using Binance.Cache.Events;
using Binance.Client;
using Binance.Client.Events;
using Binance.Utility;
using Binance.WebSocket;
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
            ExampleMain();
            //ExampleMainWithoutDI();
            //AdvancedExampleMain();
            //CombinedStreamsExample.ExampleMain();
        }

        /// <summary>
        /// Example using cache and manager.
        /// </summary>
        private static void ExampleMain()
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

                    // Use alternative, low-level, web socket client implementation.
                    //.AddTransient<IWebSocketClient, WebSocket4NetClient>()
                    //.AddTransient<IWebSocketClient, WebSocketSharpClient>()

                    .AddLogging(builder => builder.SetMinimumLevel(LogLevel.Trace))
                    .BuildServiceProvider();

                // Configure logging.
                services.GetService<ILoggerFactory>()
                    .AddFile(configuration.GetSection("Logging:File"));
                    // NOTE: Using ":" requires Microsoft.Extensions.Configuration.Binder.

                // Get configuration settings.
                var symbols = configuration.GetSection("TradeHistory:Symbols").Get<string[]>()
                    ?? new string[] { Symbol.BTC_USDT };

                var limit = 25;
                try { limit = Convert.ToInt32(configuration.GetSection("TradeHistory")?["Limit"]); }
                catch { /* ignore */ }

                // Initialize manager.
                var cache = services.GetService<IAggregateTradeWebSocketCache>();

                // Add error event handler.
                cache.Error += (s, e) => Console.WriteLine(e.Exception.Message);

                foreach (var symbol in symbols)
                {
                    // Subscribe to symbol with callback.
                    cache.Subscribe(symbol, limit, Display);

                    lock (_sync)
                    {
                        _message = symbol == symbols.Last()
                            ? $"Symbol: \"{symbol}\" ...press any key to exit."
                            : $"Symbol: \"{symbol}\" ...press any key to continue.";
                    }
                    Console.ReadKey(true);

                    // Unsubscribe from symbol.
                    cache.Unsubscribe();
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

        /// <summary>
        /// Example using manager without DI framework (not recommended).
        /// </summary>
        private static void ExampleMainWithoutDI()
        {
            try
            {
                // Load configuration.
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", true, false)
                    .Build();

                // Get configuration settings.
                var symbols = configuration.GetSection("TradeHistory:Symbols").Get<string[]>()
                    ?? new string[] { Symbol.BTC_USDT };

                var limit = 25;
                try { limit = Convert.ToInt32(configuration.GetSection("TradeHistory")?["Limit"]); }
                catch { /* ignore */ }

                var loggerFactory = new LoggerFactory();
                loggerFactory.AddFile(configuration.GetSection("Logging:File"));

                // All the things a DI framework can instantiate for you...
                var api = new BinanceApi(BinanceHttpClient.Instance, logger: loggerFactory.CreateLogger<BinanceApi>());
                var client = new AggregateTradeClient(loggerFactory.CreateLogger<AggregateTradeClient>());
                var webSocket = new DefaultWebSocketClient(logger: loggerFactory.CreateLogger<DefaultWebSocketClient>());
                var stream = new BinanceWebSocketStream(webSocket, loggerFactory.CreateLogger<BinanceWebSocketStream>());
                var controller = new BinanceWebSocketStreamController(api, stream, loggerFactory.CreateLogger<BinanceWebSocketStreamController>());
                var publisher = new BinanceWebSocketStreamPublisher(controller, loggerFactory.CreateLogger<BinanceWebSocketStreamPublisher>());
                var webSocketClient = new AggregateTradeWebSocketClient(client, publisher, loggerFactory.CreateLogger<AggregateTradeWebSocketClient>());
                var cache = new AggregateTradeWebSocketCache(api, webSocketClient, loggerFactory.CreateLogger<AggregateTradeWebSocketCache>());

                // Add error event handler.
                controller.Error += (s, e) => HandleError(e.Exception);

                foreach (var symbol in symbols)
                {
                    // Subscribe to symbol with callback.
                    cache.Subscribe(symbol, limit, Display);

                    lock (_sync)
                    {
                        _message = symbol == symbols.Last()
                            ? $"Symbol: \"{symbol}\" ...press any key to exit."
                            : $"Symbol: \"{symbol}\" ...press any key to continue.";
                    }
                    Console.ReadKey(true);

                    // Unsubscribe from symbol.
                    cache.Unsubscribe();
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

        /// <summary>
        /// Example using cache, web socket stream (or client), and controller.
        /// </summary>
        // ReSharper disable once UnusedMember.Local
        private static void AdvancedExampleMain()
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
                var limit = 25;
                var symbol = configuration.GetSection("TradeHistory")?["Symbol"] ?? Symbol.BTC_USDT;
                try { limit = Convert.ToInt32(configuration.GetSection("TradeHistory")?["Limit"]); }
                catch { /* ignore */ }

                // Initialize cache.
                var cache = services.GetService<IAggregateTradeCache>();
                
                // Initialize stream.
                var webSocket = services.GetService<IBinanceWebSocketStream>();

                // Initialize controller.
                using (var controller = new RetryTaskController(webSocket.StreamAsync))
                {
                    controller.Error += (s, e) => HandleError(e.Exception);

                    // Subscribe cache to symbol with limit and callback.
                    cache.Subscribe(symbol, limit, Display);

                    // Set web socket URI using cache subscribed streams.
                    webSocket.Uri = BinanceWebSocketStream.CreateUri(cache);
                    // NOTE: This must be done after cache subscribe.

                    // Route stream messages to cache.
                    webSocket.Message += (s, e) => cache.HandleMessage(e.Subject, e.Json);

                    // Begin streaming.
                    controller.Begin();

                    lock (_sync) _message = "...press any key to continue.";
                    Console.ReadKey(true);
                }

                //*//////////////////////////////////////////////////////////
                // Alternative usage (with an existing IJsonPublisherClient).
                /////////////////////////////////////////////////////////////

                // Initialize stream/client.
                var client = services.GetService<IAggregateTradeWebSocketClient>();

                // Disable automatic streaming (for this contrived example).
                client.Publisher.IsAutoStreamingEnabled = false;

                cache.Client = client; // link [new] client to cache.

                // Initialize controller.
                using (var controller = new RetryTaskController(webSocket.StreamAsync))
                {
                    controller.Error += (s, e) => HandleError(e.Exception);

                    // Subscribe cache to symbol with limit and callback.
                    //cache.Subscribe(symbol, limit, Display);
                    // NOTE: Cache is already subscribed to symbol (above).

                    // NOTE: With IJsonPublisherClient, publisher is automagically subscribed.

                    // Begin streaming.
                    controller.Begin();

                    lock (_sync) _message = "(alternative usage) ...press any key to exit.";
                    Console.ReadKey(true);
                }
                ///////////////////////////////////////////////////////////*/
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

        private static void Display(AggregateTradeCacheEventArgs args)
        {
            lock (_sync)
            {
                Console.SetCursorPosition(0, 0);
                foreach (var trade in args.Trades.Reverse())
                {
                    Console.WriteLine($"  {trade.Time.ToLocalTime()} - {trade.Symbol.PadLeft(8)} - {(trade.IsBuyerMaker ? "Sell" : "Buy").PadLeft(4)} - {trade.Quantity:0.00000000} @ {trade.Price:0.00000000}{(trade.IsBestPriceMatch ? "*" : " ")} - [ID: {trade.Id}] - {trade.Time.ToTimestamp()}         ".PadRight(119));
                }
                Console.WriteLine();
                Console.WriteLine(_message.PadRight(119));
            }
        }

        private static void HandleError(Exception e)
        {
            lock (_sync)
            {
                Console.WriteLine(e.Message);
            }
        }

        /// <summary>
        /// TEST
        /// </summary>
        /// <param name="args"></param>
        // ReSharper disable once UnusedMember.Local
        private static void Display(AggregateTradeEventArgs args)
        {
            lock (_sync)
            {
                Console.SetCursorPosition(0, 0);

                var trade = args.Trade;
                Console.WriteLine($"  {trade.Time.ToLocalTime()} - {trade.Symbol.PadLeft(8)} - {(trade.IsBuyerMaker ? "Sell" : "Buy").PadLeft(4)} - {trade.Quantity:0.00000000} @ {trade.Price:0.00000000}{(trade.IsBestPriceMatch ? "*" : " ")} - [ID: {trade.Id}] - {trade.Time.ToTimestamp()}         ".PadRight(119));

                Console.WriteLine();
                Console.WriteLine(_message.PadRight(119));
            }
        }

        /// <summary>
        /// TEST
        /// </summary>
        /// <param name="args"></param>
        // ReSharper disable once UnusedMember.Local
        private static void Display(TradeCacheEventArgs args)
        {
            lock (_sync)
            {
                Console.SetCursorPosition(0, 0);
                foreach (var trade in args.Trades.Reverse())
                {
                    Console.WriteLine($"  {trade.Time.ToLocalTime()} - {trade.Symbol.PadLeft(8)} - {(trade.IsBuyerMaker ? "Sell" : "Buy").PadLeft(4)} - {trade.Quantity:0.00000000} @ {trade.Price:0.00000000}{(trade.IsBestPriceMatch ? "*" : " ")} - [ID: {trade.Id}] - {trade.Time.ToTimestamp()}         ".PadRight(119));
                }
                Console.WriteLine();
                Console.WriteLine(_message.PadRight(119));
            }
        }
    }
}
