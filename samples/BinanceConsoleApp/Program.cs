using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Binance;
using Binance.Account;
using Binance.Account.Orders;
using Binance.Api;
using Binance.Api.WebSocket;
using Binance.Cache;
using Binance.Market;
using BinanceConsoleApp.Controllers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace BinanceConsoleApp
{
    /// <summary>
    /// .NET Core console application used for Binance integration testing.
    /// </summary>
    internal class Program
    {
        public static IConfigurationRoot Configuration;

        public static IServiceProvider ServiceProvider;

        public static IBinanceApi Api;
        public static IBinanceApiUser User;

        public static IOrderBookCache OrderBookCache;
        public static ICandlesticksCache KlineCache;
        public static IAggregateTradesCache TradesCache;
        public static IUserDataWebSocketClient UserDataClient;

        public static Task LiveTask;
        public static CancellationTokenSource LiveTokenSource;

        public static readonly object ConsoleSync = new object();

        public static bool IsOrdersTestOnly = true;

        private static readonly IList<IHandleCommand> CommandHandlers
            = new List<IHandleCommand>();

        public static async Task Main(string[] args)
        {
            // Un-comment to run example(s)...
            //await AccountBalancesExample.ExampleMain(args);
            //await MinimalWithDependencyInjection.ExampleMain(args);
            //await MinimalWithoutDependencyInjection.ExampleMain(args);

            var cts = new CancellationTokenSource();

            try
            {
                // Load configuration.
                Configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", true, false)
                    .AddUserSecrets<Program>()
                    .Build();

                // Configure services.
               ServiceProvider = new ServiceCollection()
                    .AddBinance().AddOptions()
                    .AddLogging(builder => builder.SetMinimumLevel(LogLevel.Trace))
                    .Configure<BinanceJsonApiOptions>(Configuration.GetSection("Api"))
                    .Configure<UserDataWebSocketClientOptions>(Configuration.GetSection("UserClient"))
                    .BuildServiceProvider();

                // Configure logging.
                ServiceProvider
                    .GetService<ILoggerFactory>()
                        .AddConsole(Configuration.GetSection("Logging").GetSection("Console"))
                        .AddFile(Configuration.GetSection("Logging").GetSection("File"));

                var key = Configuration["BinanceApiKey"] // user secrets configuration.
                    ?? Configuration.GetSection("User")["ApiKey"]; // appsettings.json configuration.

                var secret = Configuration["BinanceApiSecret"] // user secrets configuration.
                    ?? Configuration.GetSection("User")["ApiSecret"]; // appsettings.json configuration.

                if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(secret))
                {
                    PrintApiNotice();
                }

                if (!string.IsNullOrEmpty(key))
                {
                    User = new BinanceApiUser(key, secret);
                }

                Api = ServiceProvider.GetService<IBinanceApi>();

                // Instantiate all assembly command handlers.
                foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
                {
                    if ((typeof(IHandleCommand)).IsAssignableFrom(type) && !type.IsAbstract)
                    {
                        CommandHandlers.Add((IHandleCommand)Activator.CreateInstance(type));
                    }
                }

                await SuperLoopAsync(cts.Token);
            }
            catch (Exception e)
            {
                lock (ConsoleSync)
                {
                    Console.WriteLine($"! FAIL: \"{e.Message}\"");
                    if (e.InnerException != null)
                    {
                        Console.WriteLine($"  -> Exception: \"{e.InnerException.Message}\"");
                    }
                }
            }
            finally
            {
                await DisableLiveTask();

                cts.Cancel();
                cts.Dispose();

                Api?.Dispose();
                User?.Dispose();

                lock (ConsoleSync)
                {
                    Console.WriteLine();
                    Console.WriteLine("  ...press any key to close window.");
                    Console.ReadKey(true);
                }
            }
        }

        private static void PrintHelp()
        {
            lock (ConsoleSync)
            {
                Console.WriteLine();
                Console.WriteLine("Usage: <command> <args>");
                Console.WriteLine();
                Console.WriteLine("Commands:");
                Console.WriteLine();
                Console.WriteLine(" Connectivity:");
                Console.WriteLine("  ping                                                 test connection to server.");
                Console.WriteLine("  time                                                 display the current server time (UTC).");
                Console.WriteLine();
                Console.WriteLine(" Market Data:");
                Console.WriteLine("  stats <symbol>                                       display 24h stats for symbol.");
                Console.WriteLine("  depth|book <symbol> [limit]                          display symbol order book, where limit: [1-100].");
                Console.WriteLine("  trades <symbol> [limit]                              display latest trades, where limit: [1-500].");
                Console.WriteLine("  tradesIn <symbol> <start> <end>                      display trades within a time range (inclusive).");
                Console.WriteLine("  tradesFrom <symbol> <tradeId> [limit]                display trades beginning with trade ID.");
                Console.WriteLine("  candles|klines <symbol> <interval> [limit]           display candlestick bars for a symbol.");
                Console.WriteLine("  candlesIn|klinesIn <symbol> <interval> <start> <end> display candlestick bars for a symbol in time range.");
                Console.WriteLine("  symbols                                              display all symbols.");
                Console.WriteLine("  prices                                               display current price for all symbols.");
                Console.WriteLine("  tops                                                 display order book top price and quantity for all symbols.");
                Console.WriteLine("  live depth|book <symbol>                             enable order book live feed for a symbol.");
                Console.WriteLine("  live klines|candles <symbol> <interval>              enable kline live feed for a symbol and interval.");
                Console.WriteLine("  live trades <symbol>                                 enable trades live feed for a symbol.");
                Console.WriteLine("  live account|user                                    enable user data live feed (api key required).");
                Console.WriteLine("  live off                                             disable the websocket live feed (there can be only one).");
                Console.WriteLine();
                Console.WriteLine(" Account (authentication required):");
                Console.WriteLine("  market <side> <symbol> <qty> [stop]                  create a market order.");
                Console.WriteLine("  limit <side> <symbol> <qty> <price> [stop]           create a limit order.");
                Console.WriteLine("  orders <symbol> [limit]                              display orders for a symbol, where limit: [1-500].");
                Console.WriteLine("  orders <symbol> open                                 display all open orders for a symbol.");
                Console.WriteLine("  order <symbol> <ID>                                  display an order by ID.");
                Console.WriteLine("  order <symbol> <ID> cancel                           cancel an order by ID.");
                Console.WriteLine("  account|balances                                     display user account information (including balances).");
                Console.WriteLine("  myTrades <symbol> [limit]                            display user trades of a symbol.");
                Console.WriteLine("  deposits [asset]                                     display user deposits of an asset or all deposits.");
                Console.WriteLine("  withdrawals [asset]                                  display user withdrawals of an asset or all withdrawals.");
                Console.WriteLine("  withdraw <asset> <address> <amount>                  submit a withdraw request (NOTE: 'test only' does NOT apply).");
                Console.WriteLine("  test <on|off>                                        determines if orders are test only (default: on).");
                Console.WriteLine();
                Console.WriteLine("  quit | exit                                          terminate the application.");
                Console.WriteLine();
                Console.WriteLine(" * default symbol: BTCUSDT");
                Console.WriteLine(" * default limit: 10");
                Console.WriteLine();
            }
        }

        internal static void PrintApiNotice()
        {
            lock (ConsoleSync)
            {
                Console.WriteLine("* NOTICE: To access some Binance endpoint features, your API Key and Secret may be required.");
                Console.WriteLine();
                Console.WriteLine("  You can either modify the 'ApiKey' and 'ApiSecret' configuration values in appsettings.json.");
                Console.WriteLine();
                Console.WriteLine("  Or use the following commands to configure the .NET user secrets for the project:");
                Console.WriteLine();
                Console.WriteLine("    dotnet user-secrets set BinanceApiKey <your api key>");
                Console.WriteLine("    dotnet user-secrets set BinanceApiSecret <your api secret>");
                Console.WriteLine();
                Console.WriteLine("  For more information: https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets");
                Console.WriteLine();
            }
        }

        private static async Task SuperLoopAsync(CancellationToken token = default)
        {
            PrintHelp();

            do
            {
                try
                {
                    var stdin = Console.ReadLine()?.Trim();
                    if (string.IsNullOrWhiteSpace(stdin))
                    {
                        PrintHelp();
                        continue;
                    }

                    // Quit/Exit
                    if (stdin.Equals("quit", StringComparison.OrdinalIgnoreCase) ||
                        stdin.Equals("exit", StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }

                    // Test-Only Orders (enable/disable)
                    if (stdin.StartsWith("test ", StringComparison.OrdinalIgnoreCase))
                    {
                        var args = stdin.Split(' ');

                        var value = "on";
                        if (args.Length > 1)
                        {
                            value = args[1];
                        }

                        IsOrdersTestOnly = !value.Equals("off", StringComparison.OrdinalIgnoreCase);

                        lock (ConsoleSync)
                        {
                            Console.WriteLine();
                            Console.WriteLine($"  Test orders: {(IsOrdersTestOnly ? "ON" : "OFF")}");
                            if (!IsOrdersTestOnly)
                                Console.WriteLine("  !! Market and Limit orders WILL be placed !!");
                            Console.WriteLine();
                        }
                    }
                    else
                    {
                        var isHandled = false;

                        foreach (var handler in CommandHandlers)
                        {
                            if (!await handler.HandleAsync(stdin, token))
                                continue;

                            isHandled = true;
                            break;
                        }

                        if (isHandled) continue;

                        lock (ConsoleSync)
                        {
                            Console.WriteLine($"! Unrecognized Command: \"{stdin}\"");
                            PrintHelp();
                        }
                    }
                }
                catch (Exception e)
                {
                    lock (ConsoleSync)
                    {
                        Console.WriteLine();
                        Console.WriteLine($"! Exception: {e.Message}");
                        if (e.InnerException != null)
                        {
                            Console.WriteLine($"  -> {e.InnerException.Message}");
                        }
                    }
                }
            }
            while (true);
        }

        internal static async Task DisableLiveTask()
        {
            LiveTokenSource?.Cancel();

            // Wait for live task to complete.
            if (LiveTask != null && !LiveTask.IsCompleted)
                await LiveTask;

            OrderBookCache?.Dispose();
            TradesCache?.Dispose();
            KlineCache?.Dispose();
            UserDataClient?.Dispose();

            LiveTokenSource?.Dispose();

            if (OrderBookCache != null)
            {
                lock (ConsoleSync) 
                {
                    Console.WriteLine();
                    Console.WriteLine("  ...live order book feed disabled.");
                }
            }
            OrderBookCache = null;

            if (KlineCache != null)
            {
                lock (ConsoleSync)
                {
                    Console.WriteLine();
                    Console.WriteLine("  ...live kline feed disabled.");
                }
            }
            KlineCache = null;

            if (TradesCache != null)
            {
                lock (ConsoleSync)
                {
                    Console.WriteLine();
                    Console.WriteLine("  ...live trades feed disabled.");
                }
            }
            TradesCache = null;

            if (UserDataClient != null)
            {
                lock (ConsoleSync)
                {
                    Console.WriteLine();
                    Console.WriteLine("  ...live account feed disabled.");
                }
            }
            UserDataClient = null;

            LiveTokenSource = null;
            LiveTask = null;
        }

        internal static void Display(AggregateTrade trade)
        {
            lock (ConsoleSync)
            {
                Console.WriteLine($"  {trade.Time().ToLocalTime()} - {trade.Symbol.PadLeft(8)} - {(trade.IsBuyerMaker ? "Sell" : "Buy").PadLeft(4)} - {trade.Quantity:0.00000000} @ {trade.Price:0.00000000}{(trade.IsBestPriceMatch ? "*" : " ")} - [ID: {trade.Id}] - {trade.Timestamp}");
            }
        }

        internal static void Display(Candlestick candlestick)
        {
            lock (ConsoleSync)
            {
                Console.WriteLine($"  {candlestick.Symbol} - O: {candlestick.Open:0.00000000} | H: {candlestick.High:0.00000000} | L: {candlestick.Low:0.00000000} | C: {candlestick.Close:0.00000000} | V: {candlestick.Volume:0.00} - [{candlestick.OpenTime}]");
            }
        }

        internal static void Display(Order order)
        {
            lock (ConsoleSync)
            {
                Console.WriteLine($"  {order.Symbol.PadLeft(8)} - {order.Type.ToString().PadLeft(6)} - {order.Side.ToString().PadLeft(4)} - {order.OriginalQuantity:0.00000000} @ {order.Price:0.00000000} - {order.Status.ToString()}  [ID: {order.Id}]");
            }
        }

        internal static void Display(AccountTrade trade)
        {
            lock (ConsoleSync)
            {
                Console.WriteLine($"  {trade.Time().ToLocalTime().ToString(CultureInfo.CurrentCulture).PadLeft(22)} - {trade.Symbol.PadLeft(8)} - {(trade.IsBuyer ? "Buy" : "Sell").PadLeft(4)} - {(trade.IsMaker ? "Maker" : "Taker")} - {trade.Quantity:0.00000000} @ {trade.Price:0.00000000}{(trade.IsBestPriceMatch ? "*" : " ")} - Fee: {trade.Commission:0.00000000} {trade.CommissionAsset.PadLeft(5)} [ID: {trade.Id}]");
            }
        }

        internal static void Display(AccountInfo account)
        {
            lock (ConsoleSync)
            {
                Console.WriteLine($"    Maker Commission:  {account.Commissions.Maker.ToString().PadLeft(3)} %");
                Console.WriteLine($"    Taker Commission:  {account.Commissions.Taker.ToString().PadLeft(3)} %");
                Console.WriteLine($"    Buyer Commission:  {account.Commissions.Buyer.ToString().PadLeft(3)} %");
                Console.WriteLine($"    Seller Commission: {account.Commissions.Seller.ToString().PadLeft(3)} %");
                Console.WriteLine($"    Can Trade:    {(account.Status.CanTrade ? "Yes" : "No").PadLeft(3)}");
                Console.WriteLine($"    Can Withdraw: {(account.Status.CanWithdraw ? "Yes" : "No").PadLeft(3)}");
                Console.WriteLine($"    Can Deposit:  {(account.Status.CanDeposit ? "Yes" : "No").PadLeft(3)}");
                Console.WriteLine();
                Console.WriteLine("    Balances (only amounts > 0):");

                Console.WriteLine();
                foreach (var balance in account.Balances)
                {
                    if (balance.Free > 0 || balance.Locked > 0)
                    {
                        Console.WriteLine($"      Asset: {balance.Asset} - Free: {balance.Free} - Locked: {balance.Locked}");
                    }
                }
                Console.WriteLine();
            }
        }
    }
}
