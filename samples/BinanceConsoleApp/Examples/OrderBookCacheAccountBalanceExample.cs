using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Binance;
using Binance.Account;
using Binance.Api;
using Binance.Application;
using Binance.Cache;
using Binance.Client.Events;
using Binance.Market;
using Binance.Utility;
using Binance.WebSocket;
using Binance.WebSocket.UserData;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// ReSharper disable AccessToDisposedClosure

// ReSharper disable once CheckNamespace
namespace BinanceConsoleApp
{
    /// <summary>
    /// Demonstrate how to monitor order book for a symbol
    /// and base asset account balance.
    /// </summary>
    internal class OrderBookCacheAccountBalanceExample
    {
        private static int _limit;

        public static async Task ExampleMain(string[] args)
        {
            try
            {
                // Load configuration.
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", true, false)
                    .AddUserSecrets<Program>() // for access to API key and secret.
                    .Build();

                // Get API key.
                var key = configuration["BinanceApiKey"] // user secrets configuration.
                    ?? configuration.GetSection("User")["ApiKey"]; // appsettings.json configuration.

                // Get API secret.
                var secret = configuration["BinanceApiSecret"] // user secrets configuration.
                    ?? configuration.GetSection("User")["ApiSecret"]; // appsettings.json configuration.

                // Configure services.
                var services = new ServiceCollection()
                    .AddBinance() // add default Binance services.
                    .AddLogging(builder => builder.SetMinimumLevel(LogLevel.Trace))
                    .BuildServiceProvider();

                // Configure logging.
                services.GetService<ILoggerFactory>()
                    .AddFile(configuration.GetSection("Logging:File"));
                    // NOTE: Using ":" requires Microsoft.Extensions.Configuration.Binder.

                Console.Clear(); // clear the display.

                _limit = 5;

                var symbol = Symbol.BTC_USDT;
                var asset = symbol.BaseAsset;

                var api = services.GetService<IBinanceApi>();
                var cache = services.GetService<IOrderBookCache>();
                var webSocket = services.GetService<IBinanceWebSocketStream>();
                var manager = services.GetService<_IUserDataWebSocketManager>();
                var userProvider = services.GetService<IBinanceApiUserProvider>();

                using (var user = userProvider.CreateUser(key, secret))
                {
                    // Query and display order book and current asset balance.
                    var balance = (await api.GetAccountInfoAsync(user)).GetBalance(asset);
                    var orderBook = await api.GetOrderBookAsync(symbol, _limit);
                    Display(orderBook, balance);

                    // Subscribe cache to symbol with callback.
                    cache.Subscribe(symbol, evt => Display(evt.OrderBook, balance));
                    // Subscribe stream to cache observed streams.
                    webSocket.Subscribe(cache);
                    // NOTE: This must be done after cache subscribe.

                    Func<CancellationToken, Task> action1 = tkn => webSocket.StreamAsync(tkn);

                    Func<CancellationToken, Task> action2 =
                        tkn => manager.SubscribeAndStreamAsync(user,
                            evt =>
                            {
                                if (evt is AccountUpdateEventArgs accountUpdateEvent)
                                {
                                    // Update asset balance.
                                    balance = accountUpdateEvent.AccountInfo.GetBalance(asset);
                                    // Display latest order book and asset balance.
                                    Display(cache.OrderBook, balance);
                                }
                            }, tkn);

                    Action<Exception> onError = err => Console.WriteLine(err.Message);

                    using (var controller1 = new RetryTaskController(action1, onError))
                    using (var controller2 = new RetryTaskController(action2, onError))
                    {
                        // Subscribe to symbol to display latest order book and asset balance.
                        controller1.Begin();

                        // Subscribe to user account information and begin streaming.
                        controller2.Begin();

                        // Verify we are NOT using a combined streams (DEMONSTRATION ONLY).
                        if (webSocket.IsCombined() || webSocket == manager.Client.Stream)
                            throw new Exception("You ARE using combined streams :(");

                        _message = "...press any key to continue.";
                        Console.ReadKey(true); // wait for user input.
                    }
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

        private static Task _displayTask = Task.CompletedTask;

        private static void Display(OrderBook orderBook, AccountBalance balance)
        {
            lock (_sync)
            {
                if (_displayTask.IsCompleted)
                {
                    // Delay to allow multiple data updates between display updates.
                    _displayTask = Task.Delay(250)
                        .ContinueWith(_ =>
                        {
                            Console.SetCursorPosition(0, 0);

                            orderBook.Print(Console.Out, _limit);
                            Console.WriteLine();

                            Console.WriteLine(balance == null
                                ? "  [None]"
                                : $"  {balance.Asset}:  {balance.Free} (free)   {balance.Locked} (locked)");
                            Console.WriteLine();

                            Console.WriteLine(_message);
                        });
                }
            }
        }
    }
}
