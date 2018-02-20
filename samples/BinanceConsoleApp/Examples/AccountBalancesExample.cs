using System;
using System.IO;
using System.Threading.Tasks;
using Binance;
using Binance.Account;
using Binance.Api;
using Binance.Application;
using Binance.Client;
using Binance.Client.Events;
using Binance.Utility;
using Binance.WebSocket;
using Binance.WebSocket.Manager;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

// ReSharper disable AccessToDisposedClosure

// ReSharper disable once CheckNamespace
namespace BinanceConsoleApp
{
    /// <summary>
    /// Demonstrate how to get current account balances, maintain a local cache
    /// and respond to real-time changes in account balances.
    /// </summary>
    internal class AccountBalancesExample
    {
        private static string _asset = Asset.BTC;

        public static async Task ExampleMain(string[] args)
        {
            try
            {
                // Load configuration.
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", true, false)
                    .AddUserSecrets<AccountBalancesExample>()
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

                var api = services.GetService<IBinanceApi>();
                var userProvider = services.GetService<IBinanceApiUserProvider>();

                using (var user = userProvider.CreateUser(key, secret))
                using (var manager = services.GetService<IUserDataWebSocketClientManager>())
                {
                    // Query and display current account balance.
                    var account = await api.GetAccountInfoAsync(user);
                    Display(account.GetBalance(_asset));

                    // Subscribe manager to user (automatically begin streaming).
                    await manager.SubscribeAsync<AccountUpdateEventArgs>(user, Display);

                    Console.WriteLine("...press any key to continue.");
                    Console.ReadKey(true); // wait for user input.

                    // Unsubscribe user (automatically cancel streaming).
                    //await manager.UnsubscribeAsync(user);
                }
            }
            catch (Exception e) { Console.WriteLine(e.Message); }
        }

        public static async Task AdvancedExampleMain(string[] args)
        {
            try
            {
                // Load configuration.
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", true, false)
                    .AddUserSecrets<AccountBalancesExample>()
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

                var api = services.GetService<IBinanceApi>();
                var client = services.GetService<IUserDataWebSocketClient>();
                var userProvider = services.GetService<IBinanceApiUserProvider>();
                var streamControl = services.GetService<IUserDataWebSocketStreamControl>();

                using (var user = userProvider.CreateUser(key, secret))
                {
                    // Query and display current account balance.
                    var account = await api.GetAccountInfoAsync(user);
                    Display(account.GetBalance(_asset));

                    var listenKey = await streamControl.OpenStreamAsync(user); // add user and start timer.

                    streamControl.ListenKeyUpdate += (s, a) =>
                    {
                        try
                        {
                            // Unsubscribe old listen key.
                            Console.WriteLine($"Unsubscribe old listen key... {a.OldListenKey}");
                            client.Unsubscribe(a.OldListenKey);

                            if (a.NewListenKey == null)
                            {
                                Console.WriteLine($"! Failed to get new listen key...");
                                return;
                            }

                            Console.WriteLine($"Subscribe to new listen key... {a.NewListenKey}");
                            client.Subscribe<AccountUpdateEventArgs>(a.NewListenKey, user, Display);

                            listenKey = a.NewListenKey;
                        }
                        catch (Exception e) { Console.WriteLine(e.Message); }
                    };

                    Console.WriteLine($"Subscribe to listen key... {listenKey}");
                    client.Subscribe<AccountUpdateEventArgs>(listenKey, user, Display);

                    using (var controller = new RetryTaskController(
                        tkn => client.Stream.StreamAsync(tkn),
                        err => Console.WriteLine(err.Message)))
                    {
                        // Begin streaming.
                        controller.Begin();

                        Console.WriteLine("...press any key to continue.");
                        Console.ReadKey(true);
                    }

                    Console.WriteLine($"Unsubscribe listen key... {listenKey}");
                    client.Unsubscribe(listenKey);

                    await streamControl.CloseStreamAsync(user);
                    streamControl.Dispose();
                }
            }
            catch (Exception e) { Console.WriteLine(e.Message); }
        }

        private static void Display(AccountUpdateEventArgs args)
            => Display(args.AccountInfo.GetBalance(_asset));

        private static void Display(AccountBalance balance)
        {
            Console.WriteLine();
            Console.WriteLine(balance == null
                ? "  [None]"
                : $"  {balance.Asset}:  {balance.Free} (free)   {balance.Locked} (locked)");
            Console.WriteLine();
        }
    }
}
