using Binance;
using Binance.Accounts;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceConsoleApp.Examples
{
    /// <summary>
    /// Demonstrate how to get current account balances, maintain a local cache
    /// and respond to real-time changes in account balances.
    /// </summary>
    class AccountBalancesExample
    {
        public static async Task Main(string[] args)
        {
            // Load configuration.
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
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
                .AddBinance().BuildServiceProvider();

            using (var user = new BinanceUser(key, secret))
            using (var api = services.GetService<IBinanceApi>())
            using (var cache = services.GetService<IAccountCache>())
            using (var cts = new CancellationTokenSource())
            {
                // Query and display current account balance.
                var account = await api.GetAccountAsync(user);
                Display(account.GetBalance(Asset.BTC));

                // Display updated account balance.
                var task = Task.Run(() => cache.SubscribeAsync(user, (e) =>
                {
                    Display(e.Account.GetBalance(Asset.BTC));
                }, cts.Token));

                Console.WriteLine("...press any key to exit.");
                Console.ReadKey(true);

                cts.Cancel();
                await task;
            }
        }

        private static void Display(AccountBalance balance)
        {
            Console.WriteLine();
            Console.WriteLine($"  {balance.Asset}:  {balance.Free} (free)   {balance.Locked} (locked)");
            Console.WriteLine();
        }
    }
}
