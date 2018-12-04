using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Binance;
using Binance.Application;
using Binance.WebSocket;
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

        public static IBinanceWebSocketClientManager ClientManager;
        public static IUserDataWebSocketManager UserDataManager;

        public static readonly object ConsoleSync = new object();

        public static bool IsOrdersTestOnly = true;

        private static readonly IList<IHandleCommand> CommandHandlers
            = new List<IHandleCommand>();

        public static async Task Main(string[] args)
        {
            // Un-comment to run example applications...
            //await AccountBalancesExample.ExampleMain(args);
            //await AccountBalancesExample.AdvancedExampleMain(args);
            //await MinimalWithDependencyInjection.ExampleMain(args);
            //await MinimalWithoutDependencyInjection.ExampleMain(args);
            //await OrderBookCacheAccountBalanceExample.AdvancedExampleMain(args);
            //await ReadMeExample.ExampleMain(args);


            var cts = new CancellationTokenSource();

            try
            {
                // Load configuration.
                Configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", true, false)
                    .AddUserSecrets<Program>() // for access to API key and secret.
                    .Build();

                // Configure services.
                ServiceProvider = new ServiceCollection()
                    // ReSharper disable once ArgumentsStyleLiteral
                    .AddBinance(useSingleCombinedStream: true) // add default Binance services.

                    // Use alternative, low-level, web socket client implementation.
                    //.AddTransient<IWebSocketClient, WebSocket4NetClient>()
                    //.AddTransient<IWebSocketClient, WebSocketSharpClient>()

                    .AddOptions()
                    .Configure<BinanceApiOptions>(Configuration.GetSection("ApiOptions"))

                    .AddLogging(builder => builder.SetMinimumLevel(LogLevel.Trace))
                    .BuildServiceProvider();

                // Configure logging.
                ServiceProvider
                    .GetService<ILoggerFactory>()
                        .AddConsole(Configuration.GetSection("Logging:Console"))
                        .AddFile(Configuration.GetSection("Logging:File"));
                        // NOTE: Using ":" requires Microsoft.Extensions.Configuration.Binder.

                var apiKey = Configuration["BinanceApiKey"] // user secrets configuration.
                    ?? Configuration.GetSection("User")["ApiKey"]; // appsettings.json configuration.

                var apiSecret = Configuration["BinanceApiSecret"] // user secrets configuration.
                    ?? Configuration.GetSection("User")["ApiSecret"]; // appsettings.json configuration.

                if (string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(apiSecret))
                {
                    PrintApiNotice();
                }

                if (!string.IsNullOrEmpty(apiKey))
                {
                    User = ServiceProvider
                        .GetService<IBinanceApiUserProvider>()
                        .CreateUser(apiKey, apiSecret);
                }

                // Instantiate the Binance API service (singleton).
                Api = ServiceProvider.GetService<IBinanceApi>();

                // Instantiate the web socket client manager service.
                ClientManager = ServiceProvider.GetService<IBinanceWebSocketClientManager>();

                // Add client manager error handler.
                ClientManager.Error += (s, e) =>
                {
                    lock (ConsoleSync)
                    {
                        Console.WriteLine();
                        Console.WriteLine($"! Client Manager Error: \"{e.Exception.Message}\"");
                        Console.WriteLine();
                    }
                };

                // Instantiate the user data web socket manager.
                UserDataManager = ServiceProvider.GetService<IUserDataWebSocketManager>();

                // Instantiate all command handlers within this assembly.
                foreach (var type in Assembly.GetExecutingAssembly().GetTypes())
                {
                    if (typeof(IHandleCommand).IsAssignableFrom(type) && !type.IsAbstract)
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
                await DisableLiveTaskAsync();

                cts.Cancel();
                cts.Dispose();

                User?.Dispose();

                ClientManager?.Dispose();
                UserDataManager?.Dispose();

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
                Console.WriteLine("  ping                                                  test connection to server.");
                Console.WriteLine("  time                                                  display the current server time (UTC).");
                Console.WriteLine();
                Console.WriteLine(" Market Data:");
                Console.WriteLine("  stats <symbol>                                        display 24h stats for a symbol or all symbols.");
                Console.WriteLine("  depth|book <symbol> [limit]                           display symbol order book, where limit: [1-100].");
                Console.WriteLine("  aggTrades <symbol> [limit]                            display latest aggregate trades, where limit: [1-500].");
                Console.WriteLine("  aggTradesIn <symbol> <start> <end>                    display aggregate trades within a time interval.");
                Console.WriteLine("  aggTradesFrom <symbol> <tradeId> [limit]              display aggregate trades beginning with aggregate trade ID.");
                Console.WriteLine("  trades <symbol> [limit]                               display latest trades, where limit: [1-500].");
                Console.WriteLine("  tradesFrom <symbol> <tradeId> [limit]                 display trades beginning with trade ID.");
                Console.WriteLine("  candles|kLines <symbol> <interval> [limit]            display candlesticks for a symbol.");
                Console.WriteLine("  candlesIn|kLinesIn <symbol> <interval> <start> <end>  display candlesticks for a symbol in time interval.");
                Console.WriteLine("  symbols|pairs [refresh]                               display all symbols (currency pairs).");
                Console.WriteLine("  assets [refresh]                                      display all assets (currencies).");
                Console.WriteLine("  price <symbol>                                        display current market price for a symbol or all symbols.");
                Console.WriteLine("  avgPrice <symbol>                                     display current average price for a symbol.");
                Console.WriteLine("  rate <baseAsset> <quoteAsset>                         display exchange rate for any pair of assets.");
                Console.WriteLine("  top <symbol>                                          display order book top price/qty for a symbol or all symbols.");
                Console.WriteLine("  live depth|book <symbol> [off]                        enable/disable order book live feed for a symbol.");
                Console.WriteLine("  live candles|kLines <symbol> <interval> [off]         enable/disable candlestick live feed for a symbol and interval.");
                Console.WriteLine("  live stats <symbol> [off]                             enable/disable 24-hour statistics live feed for a symbol.");
                Console.WriteLine("  live aggTrades <symbol> [off]                         enable/disable aggregate trades live feed for a symbol.");
                Console.WriteLine("  live trades <symbol> [off]                            enable/disable trades live feed for a symbol.");
                Console.WriteLine("  live account|user [off]                               enable/disable user data live feed (api key required).");
                Console.WriteLine("  live off                                              disable all web socket live feeds.");
                Console.WriteLine();
                Console.WriteLine(" Account (authentication required):");
                Console.WriteLine("  market <side> <symbol> <qty>                          create a market order.");
                Console.WriteLine("  stopLoss <side> <symbol> <qty> <stop>                 create a stop loss market order.");
                Console.WriteLine("  takeProfit <side> <symbol> <qty> <stop>               create a take profit market order.");
                Console.WriteLine("  limit <side> <symbol> <qty> <price> [postonly]        create a limit order (postonly default: 'true').");
                Console.WriteLine("  stopLossLimit <side> <symbol> <qty> <price> <stop>    create a stop loss limit order.");
                Console.WriteLine("  takeProfitLimit <side> <symbol> <qty> <price> <stop>  create a take profit limit order.");
                Console.WriteLine("  orders <symbol> [limit]                               display orders for a symbol, where limit: [1-500].");
                Console.WriteLine("  orders [symbol] open                                  display all open orders for a symbol or all symbols.");
                Console.WriteLine("  ordersIn <symbol> <start> <end>                       display orders for a symbol within a time interval.");
                Console.WriteLine("  cancel [symbol]                                       cancel all open orders for a symbol (or all symbols...).");
                Console.WriteLine("  order <symbol> <ID>                                   display an order by symbol and ID.");
                Console.WriteLine("  order <symbol> <ID> cancel                            cancel an order by symbol and ID.");
                Console.WriteLine("  account|balances                                      display user account information (including balances).");
                Console.WriteLine("  myTrades <symbol> [limit]                             display user trades of a symbol.");
                Console.WriteLine("  myTradesFrom <symbol> <tradeId> [limit]               display user trades of a symbol beginning with trade ID.");
                Console.WriteLine("  myTradesIn <symbol> <start> <end>                     display user trades of a symbol within a time interval.");
                Console.WriteLine("  myTrades order <symbol> <orderId>                     display user trades of a symbol by order ID.");
                Console.WriteLine("  address <asset>                                       display user deposit address for an asset.");
                Console.WriteLine("  deposits [asset]                                      display user deposits of an asset or all deposits.");
                Console.WriteLine("  withdrawals [asset]                                   display user withdrawals of an asset or all withdrawals.");
                Console.WriteLine("  withdraw <asset> <address> <amount> [description]     submit a withdraw request (NOTE: 'test only' does NOT apply).");
                Console.WriteLine("  withdrawFee <asset>                                   display withdraw fee for an asset.");
                Console.WriteLine("  status [account|system]                               display account or system status.");
                Console.WriteLine("  test <on|off>                                         determines if orders are test only (default: 'on').");
                Console.WriteLine();
                Console.WriteLine("  quit | exit                                           terminate the application.");
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

        internal static async Task DisableLiveTaskAsync()
        {
            lock (ConsoleSync)
            {
                if (ClientManager.Clients().Any(c => c.Publisher.PublishedStreams.Any()))
                {
                    Console.WriteLine();
                    Console.WriteLine("  ...live feeds disabled.");
                }
            }

            ClientManager.UnsubscribeAll();

            await UserDataManager.UnsubscribeAllAsync();
        }

        internal static void Display(DepositAddress address)
        {
            lock (ConsoleSync)
            {
                Console.WriteLine($"  {address.Asset} => {address.Address} - {(!string.IsNullOrWhiteSpace(address.AddressTag) ? address.AddressTag : "[No Address Tag]")}");
            }
        }

        internal static void Display(SymbolPrice price)
        {
            lock (ConsoleSync)
            {
                Console.WriteLine($"  {price.Symbol.PadLeft(8)}: {price.Value}");
            }
        }

        internal static void Display(SymbolAveragePrice price)
        {
            lock (ConsoleSync)
            {
                Console.WriteLine($"  {price.Symbol.PadLeft(8)}: {price.Value}  ({price.Minutes} minutes)");
            }
        }

        internal static void Display(SymbolStatistics stats)
        {
            lock (ConsoleSync)
            {
                Console.WriteLine($"  24-hour statistics for {stats.Symbol}:");
                Console.WriteLine($"    %: {stats.PriceChangePercent:0.00} | O: {stats.OpenPrice:0.00000000} | H: {stats.HighPrice:0.00000000} | L: {stats.LowPrice:0.00000000} | V: {stats.Volume:0.}");
                Console.WriteLine($"    Bid: {stats.BidPrice:0.00000000} | Last: {stats.LastPrice:0.00000000} | Ask: {stats.AskPrice:0.00000000} | Avg: {stats.WeightedAveragePrice:0.00000000}");
                Console.WriteLine();
            }
        }

        internal static void Display(SymbolStatistics[] statistics)
        {
            lock (ConsoleSync)
            {
                foreach (var stats in statistics)
                {
                    Console.WriteLine($"  24-hour statistics for {stats.Symbol}:");
                    Console.WriteLine($"    %: {stats.PriceChangePercent:0.00} | O: {stats.OpenPrice:0.00000000} | H: {stats.HighPrice:0.00000000} | L: {stats.LowPrice:0.00000000} | V: {stats.Volume:0.}");
                    Console.WriteLine($"    Bid: {stats.BidPrice:0.00000000} | Last: {stats.LastPrice:0.00000000} | Ask: {stats.AskPrice:0.00000000} | Avg: {stats.WeightedAveragePrice:0.00000000}");
                    Console.WriteLine();
                }
            }
        }

        internal static void Display(OrderBookTop top)
        {
            lock (ConsoleSync)
            {
                Console.WriteLine($"  {top.Symbol.PadLeft(8)}  -  Bid: {top.Bid.Price.ToString("0.00000000").PadLeft(13)} (qty: {top.Bid.Quantity})   |   Ask: {top.Ask.Price.ToString("0.00000000").PadLeft(13)} (qty: {top.Ask.Quantity})");
            }
        }

        internal static void Display(AggregateTrade trade)
        {
            lock (ConsoleSync)
            {
                Console.WriteLine($"  {trade.Time.ToLocalTime()} - {trade.Symbol.PadLeft(8)} - {(trade.IsBuyerMaker ? "Sell" : "Buy").PadLeft(4)} - {trade.Quantity:0.00000000} @ {trade.Price:0.00000000}{(trade.IsBestPriceMatch ? "*" : " ")} - [ID: {trade.Id}] - {trade.Time.ToTimestamp()}");
            }
        }

        internal static void Display(Trade trade)
        {
            lock (ConsoleSync)
            {
                Console.WriteLine($"  {trade.Time.ToLocalTime()} - {trade.Symbol.PadLeft(8)} - {(trade.IsBuyerMaker ? "Sell" : "Buy").PadLeft(4)} - {trade.Quantity:0.00000000} @ {trade.Price:0.00000000}{(trade.IsBestPriceMatch ? "*" : " ")} - [ID: {trade.Id}] - {trade.Time.ToTimestamp()}");
            }
        }

        internal static void Display(Candlestick candlestick)
        {
            lock (ConsoleSync)
            {
                Console.WriteLine($"  {candlestick.Symbol} - O: {candlestick.Open:0.00000000}  H: {candlestick.High:0.00000000}  L: {candlestick.Low:0.00000000}  C: {candlestick.Close:0.00000000}  V: {candlestick.Volume:0.00}  [{candlestick.OpenTime.ToTimestamp()}]");
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
                Console.WriteLine($"  {trade.Time.ToLocalTime()} - {trade.Symbol.PadLeft(8)} - {(trade.IsBuyer ? "Buy" : "Sell").PadLeft(4)} - {(trade.IsMaker ? "Maker" : "Taker")} - {trade.Quantity:0.00000000} @ {trade.Price:0.00000000}{(trade.IsBestPriceMatch ? "*" : " ")} - Fee: {trade.Commission:0.00000000} {trade.CommissionAsset.PadRight(5)} ID: {trade.Id}");
            }
        }

        internal static void Display(AccountInfo account)
        {
            lock (ConsoleSync)
            {
                Console.WriteLine($"    Maker Commission:  {account.Commissions.Maker.ToString().PadLeft(3)} bips  ({account.Commissions.Maker / 100.0m}%)");
                Console.WriteLine($"    Taker Commission:  {account.Commissions.Taker.ToString().PadLeft(3)} bips  ({account.Commissions.Taker / 100.0m}%)");
                Console.WriteLine($"    Buyer Commission:  {account.Commissions.Buyer.ToString().PadLeft(3)} bips  ({account.Commissions.Buyer / 100.0m}%)");
                Console.WriteLine($"    Seller Commission: {account.Commissions.Seller.ToString().PadLeft(3)} bips  ({account.Commissions.Seller / 100.0m}%)");
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
