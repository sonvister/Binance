using System;
using System.IO;
using System.Threading.Tasks;
using Binance;
using Binance.Account;
using Binance.Api;
using Binance.Application;
using Binance.Cache;
using Binance.Utility;
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
    /// 
    /// To use, call AccountBalancesExample.ExampleMain() from Program.Main().
    /// </summary>
    internal class AccountBalancesExample
    {
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
                    .AddBinance()
                    .AddOptions()
                    .AddLogging(builder => builder.SetMinimumLevel(LogLevel.Trace))
                    .Configure<UserDataWebSocketClientOptions>(configuration.GetSection("UserClientOptions"))
                    .BuildServiceProvider();

                // Configure logging.
                services.GetService<ILoggerFactory>()
                    .AddFile(configuration.GetSection("Logging:File"));

                var asset = Asset.BTC;

                var api = services.GetService<IBinanceApi>();
                var cache = services.GetService<IAccountInfoCache>();
                var userProvider = services.GetService<IBinanceApiUserProvider>();

                using (var controller = new RetryTaskController())
                using (var user = userProvider.CreateUser(key, secret))
                {
                    // Query and display current account balance.
                    var account = await api.GetAccountInfoAsync(user);

                    Display(account.GetBalance(asset));

                    // Display updated account balance.
                    controller.Begin(
                        tkn => cache.SubscribeAndStreamAsync(user, 
                            evt => Display(evt.AccountInfo.GetBalance(asset)), tkn),
                        err => Console.WriteLine(err.Message));

                    Console.WriteLine("...press any key to continue.");
                    Console.ReadKey(true);
                }
            }
            catch (Exception e) { Console.WriteLine(e.Message); }
        }

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
