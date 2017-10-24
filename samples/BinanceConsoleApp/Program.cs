using Binance;
using Binance.Accounts;
using Binance.Api;
using Binance.Api.WebSocket.Events;
using Binance.Orders;
using Binance.Orders.Book.Cache;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BinanceConsoleApp
{
    /// <summary>
    /// .NET Core console application used for Binance integration testing.
    /// </summary>
    class Program
    {
        private static IConfigurationRoot _configuration;

        private static IServiceProvider _serviceProvider;

        private static IBinanceApi _api;

        private static IBinanceUser _user;

        private static IOrderBookCache _liveOrderBook;
        private static IKlineWebSocketClient _klineClient;
        private static ITradesWebSocketClient _tradesClient;
        private static IUserDataWebSocketClient _userDataClient;

        private static Task _liveTask;
        private static CancellationTokenSource _liveTokenSource;

        private static readonly object _consoleSync = new object();

        private const long _recvWindow = 15000;

        private static bool _isOrdersTestOnly = true;

        public static async Task Main(string[] args)
        {
            var cts = new CancellationTokenSource();

            try
            {
                // Load configuration.
                _configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: false)
                    .AddUserSecrets<Program>()
                    .Build();

                // Configure services.
               _serviceProvider = new ServiceCollection()
                    //.Configure<BinanceOptions>(
                    //    _configuration.GetSection(nameof(BinanceOptions)))
                    .AddLogging()
                    .AddApplicationServices()
                    .BuildServiceProvider();

                // Configure logging.
                _serviceProvider
                    .GetService<ILoggerFactory>()
                        .AddConsole(_configuration.GetSection("Logging.Console"));


                var key = _configuration["BinanceWebApiKey"] // user secrets configuration.
                    ?? _configuration.GetSection("Binance")["WebApiKey"]; // appsettings.json configuration.

                var secret = _configuration["BinanceWebApiSecret"] // user secrets configuration.
                    ?? _configuration.GetSection("Binance")["WebApiSecret"]; // appsettings.json configuration.

                if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(secret))
                {
                    PrintApiNotice();
                }

                if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(secret))
                {
                    _user = new BinanceUser(
                        _configuration["BinanceWebApiKey"],
                        _configuration["BinanceWebApiSecret"]);
                }

                _api = _serviceProvider.GetService<IBinanceApi>();

                await SuperLoopAsync(cts.Token);
            }
            catch (Exception e)
            {
                lock (_consoleSync)
                {
                    Console.WriteLine($"! FAIL: \"{e.Message}\"");
                    if (e.InnerException != null)
                        Console.WriteLine($"  -> Exception: \"{e.InnerException.Message}\"");
                }
            }
            finally
            {
                await DisableLiveTask();

                cts?.Cancel();
                cts?.Dispose();

                _user?.Dispose();
                _user = null;

                lock (_consoleSync)
                {
                    Console.WriteLine();
                    Console.WriteLine("  ...press any key to close window.");
                    Console.ReadKey(true);
                }
            }
        }

        private static void PrintHelp()
        {
            lock (_consoleSync)
            {
                Console.WriteLine();
                Console.WriteLine("Usage: <command> <args>");
                Console.WriteLine();
                Console.WriteLine("Commands:");
                Console.WriteLine();
                Console.WriteLine(" Connectivity:");
                Console.WriteLine("  ping                                       test connection to server.");
                Console.WriteLine("  time                                       get current server time (UTC).");
                Console.WriteLine();
                Console.WriteLine(" Market Data:");
                Console.WriteLine("  stats <symbol>                             display 24h stats for symbol.");
                Console.WriteLine("  depth|book <symbol> [limit]                display symbol order book, where limit: [1-100].");
                Console.WriteLine("  trades <symbol> [limit]                    display latest N trades, where limit: [1-500].");
                Console.WriteLine("  candles|klines <symbol> <interval>         display candlestick bars for a symbol.");
                Console.WriteLine("  symbols                                    display all symbols.");
                Console.WriteLine("  prices                                     display current price for all symbols.");
                Console.WriteLine("  tops                                       display order book top price and quantity for all symbols.");
                Console.WriteLine("  live depth <symbol>                        enable order book live feed for a symbol.");
                Console.WriteLine("  live kline <symbol> <interval>             enable kline live feed for a symbol and interval.");
                Console.WriteLine("  live trades <symbol>                       enable trades live feed for a symbol.");
                Console.WriteLine("  live account                               enable user data live feed.");
                Console.WriteLine("  live off                                   disable the websocket live feed (there can be only one).");
                Console.WriteLine();
                Console.WriteLine(" Account (authentication required):");
                Console.WriteLine("  limit <side> <symbol> <qty> <price>        create a limit order.");
                Console.WriteLine("  market <side> <symbol> <qty>               create a market order.");
                Console.WriteLine("  orders <symbol> [limit]                    display orders for a symbol, where limit: [1-500].");
                Console.WriteLine("  orders <symbol> open                       display all open orders for a symbol.");
                Console.WriteLine("  order <symbol> <ID>                        display an order by ID.");
                Console.WriteLine("  order <symbol> <ID> cancel                 cancel an order by ID.");
                Console.WriteLine("  account|balances|positions                 display user account information (including balances).");
                Console.WriteLine("  mytrades <symbol> [limit]                  display user trades of a symbol.");
                Console.WriteLine("  deposits [asset]                           display user deposits of an asset or all deposits.");
                Console.WriteLine("  withdrawals [asset]                        display user withdrawals of an asset or all withdrawals.");
                Console.WriteLine("  withdraw <asset> <address> <amount>        submit a withdraw request (NOTE: 'test only' does NOT apply).");
                Console.WriteLine("  test <on|off>                              determines if orders are test only (default: on).");
                Console.WriteLine();
                Console.WriteLine("  quit | exit                                terminate the application.");
                Console.WriteLine();
                Console.WriteLine(" * default symbol: BTCUSDT");
                Console.WriteLine(" * default limit: 10");
                Console.WriteLine();
            }
        }

        private static void PrintApiNotice()
        {
            lock (_consoleSync)
            {
                Console.WriteLine("* NOTICE: To access some Binance endpoint features, your web API Key and Secret may be required.");
                Console.WriteLine();
                Console.WriteLine("  You can either modify the 'WebApiKey' and 'WebApiSecret' configuration values in appsettings.json.");
                Console.WriteLine();
                Console.WriteLine("  Or use the following commands to configure the .NET user secrets for the project:");
                Console.WriteLine();
                Console.WriteLine("    dotnet user-secrets set BinanceWebApiKey <your api key>");
                Console.WriteLine("    dotnet user-secrets set BinanceWebApiSecret <your api secret>");
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
                    // Ping
                    else if (stdin.Equals("ping", StringComparison.OrdinalIgnoreCase))
                    {
                        var isSuccessful = await _api.PingAsync(token);
                        lock (_consoleSync)
                        {
                            Console.WriteLine($"  Ping: {(isSuccessful ? "SUCCESSFUL" : "FAILED")}");
                            Console.WriteLine();
                        }
                    }
                    // Time
                    else if (stdin.Equals("time", StringComparison.OrdinalIgnoreCase))
                    {
                        var time = await _api.GetTimeAsync(token);
                        lock (_consoleSync)
                        {
                            Console.WriteLine($"  {time.Kind.ToString().ToUpper()} Time: {time}  [Local: {time.ToLocalTime()}]");
                            Console.WriteLine();
                        }
                    }
                    // Stats (24-hour)
                    else if (stdin.StartsWith("stats", StringComparison.OrdinalIgnoreCase))
                    {
                        var args = stdin.Split(' ');

                        string symbol = Symbol.BTC_USDT;
                        if (args.Length > 1)
                        {
                            symbol = args[1];
                        }

                        var stats = await _api.Get24hrStatsAsync(symbol, token);

                        lock (_consoleSync)
                        {
                            Console.WriteLine();
                            Console.WriteLine($"  24-hour statistics for {stats.Symbol}:");
                            Console.WriteLine($"    %: {stats.PriceChangePercent.ToString("0.00")} | O: {stats.OpenPrice.ToString("0.00000000")} | H: {stats.HighPrice.ToString("0.00000000")} | L: {stats.LowPrice.ToString("0.00000000")} | V: {stats.Volume.ToString("0.")}");
                            Console.WriteLine($"    Bid: {stats.BidPrice.ToString("0.00000000")} | Last: {stats.LastPrice.ToString("0.00000000")} | Ask: {stats.AskPrice.ToString("0.00000000")} | Avg: {stats.WeightedAveragePrice.ToString("0.00000000")}");
                            Console.WriteLine();
                        }
                    }
                    // Order Book
                    else if (stdin.StartsWith("depth", StringComparison.OrdinalIgnoreCase)
                          || stdin.StartsWith("book", StringComparison.OrdinalIgnoreCase))
                    {
                        var args = stdin.Split(' ');

                        string symbol = Symbol.BTC_USDT;
                        int limit = 10;

                        if (args.Length > 1)
                        {
                            if (!int.TryParse(args[1], out limit))
                            {
                                symbol = args[1];
                                limit = 10;
                            }
                        }

                        if (args.Length > 2)
                        {
                            int.TryParse(args[2], out limit);
                        }

                        IOrderBook orderBook = null;

                        // If live order book is active (for symbol), get cached data.
                        if (_liveOrderBook != null && _liveOrderBook.Symbol == symbol)
                            orderBook = _liveOrderBook.Clone(limit); // get snapshot.

                        // Query order book from API, if needed.
                        if (orderBook == null)
                            orderBook = await _api.GetOrderBookAsync(symbol, limit, token);

                        lock (_consoleSync)
                        {
                            Console.WriteLine();
                            orderBook.Print(Console.Out, limit);
                            Console.WriteLine();
                        }
                    }
                    // Trades
                    else if (stdin.StartsWith("trades", StringComparison.OrdinalIgnoreCase))
                    {
                        var args = stdin.Split(' ');

                        string symbol = Symbol.BTC_USDT;
                        if (args.Length > 1)
                        {
                            symbol = args[1];
                        }

                        int limit = 10;
                        if (args.Length > 2)
                        {
                            int.TryParse(args[2], out limit);
                        }

                        var trades = await _api.GetAggregateTradesAsync(symbol, limit: limit, token: token);

                        lock (_consoleSync)
                        {
                            Console.WriteLine();
                            foreach (var trade in trades)
                            {
                                Console.WriteLine($"  {(trade.IsBuyerMaker ? "SELL" : "BUY").PadLeft(4)}  -  ID: {trade.Id} [{trade.FirstTradeId} - {trade.LastTradeId}]  -  {trade.Quantity.ToString("0.00000000")} @ {trade.Price.ToString("0.00000000")}  -  At Best: {(trade.IsBestPriceMatch ? "Yes" : "No")}");
                            }
                            Console.WriteLine();
                        }
                    }
                    // Candlesticks
                    else if (stdin.StartsWith("candles", StringComparison.OrdinalIgnoreCase)
                          || stdin.StartsWith("klines", StringComparison.OrdinalIgnoreCase))
                    {
                        var args = stdin.Split(' ');

                        string symbol = Symbol.BTC_USDT;
                        if (args.Length > 1)
                        {
                            symbol = args[1];
                        }

                        var interval = KlineInterval.Hour;
                        if (args.Length > 2)
                        {
                            interval = args[2].ToKlineInterval();
                        }

                        int limit = 10;
                        if (args.Length > 3)
                        {
                            int.TryParse(args[3], out limit);
                        }

                        var candlesticks = await _api.GetCandlesticksAsync(symbol, interval, limit, token: token);

                        lock (_consoleSync)
                        {
                            Console.WriteLine();
                            foreach (var candlestick in candlesticks)
                            {
                                Console.WriteLine($"   O: {candlestick.Open.ToString("0.00000000")} | H: {candlestick.High.ToString("0.00000000")} | L: {candlestick.Low.ToString("0.00000000")} | C: {candlestick.Close.ToString("0.00000000")} | V: {candlestick.Volume.ToString("0.")}");
                            }
                            Console.WriteLine();
                        }
                    }
                    // Symbols
                    else if (stdin.Equals("symbols", StringComparison.OrdinalIgnoreCase))
                    {
                        var symbols = await _api.SymbolsAsync(token);
                        lock (_consoleSync)
                        {
                            Console.WriteLine();
                            Console.WriteLine(string.Join(", ", symbols));
                            Console.WriteLine();
                        }
                    }
                    // Prices
                    else if (stdin.Equals("prices", StringComparison.OrdinalIgnoreCase))
                    {
                        var prices = await _api.GetPricesAsync(token);
                        lock (_consoleSync)
                        {
                            Console.WriteLine();
                            foreach (var price in prices)
                            {
                                Console.WriteLine($"  {price.Symbol.PadLeft(8)}: {price.Value}");
                            }
                            Console.WriteLine();
                        }
                    }
                    // Tops
                    else if (stdin.Equals("tops", StringComparison.OrdinalIgnoreCase))
                    {
                        var tops = await _api.GetOrderBookTopsAsync(token);
                        lock (_consoleSync)
                        {
                            Console.WriteLine();
                            foreach (var top in tops)
                            {
                                Console.WriteLine($"  {top.Symbol.PadLeft(8)}  -  {top.Bid.Price.ToString().PadLeft(12)}: {top.Bid.Quantity}  |  {top.Ask.Price}: {top.Ask.Quantity}");
                            }
                            Console.WriteLine();
                        }
                    }
                    // Live Order Book
                    else if (stdin.StartsWith("live", StringComparison.OrdinalIgnoreCase))
                    {
                        var args = stdin.Split(' ');

                        string endpoint = "depth";
                        if (args.Length > 1)
                        {
                            endpoint = args[1];
                        }

                        string symbol = Symbol.BTC_USDT;
                        if (args.Length > 2)
                        {
                            symbol = args[2];
                        }

                        if (endpoint.Equals("depth", StringComparison.OrdinalIgnoreCase))
                        {
                            if (_liveTask != null)
                            {
                                lock (_consoleSync)
                                {
                                    Console.WriteLine($"! A live task is currently active ...use 'live off' to disable.");
                                }
                                continue;
                            }

                            _liveTokenSource = new CancellationTokenSource();

                            _liveOrderBook = _serviceProvider.GetService<IOrderBookCache>();
                            _liveOrderBook.Update += OnOrderBookUpdated;

                            _liveTask = Task.Run(() => _liveOrderBook.SubscribeAsync(symbol, _liveTokenSource.Token));

                            lock (_consoleSync)
                            {
                                Console.WriteLine();
                                Console.WriteLine($"  ...live order book enabled for symbol: {symbol} ...use 'live off' to disable.");
                            }
                        }
                        else if (endpoint.Equals("kline", StringComparison.OrdinalIgnoreCase))
                        {
                            if (_liveTask != null)
                            {
                                lock (_consoleSync)
                                {
                                    Console.WriteLine($"! A live task is currently active ...use 'live off' to disable.");
                                }
                                continue;
                            }

                            var interval = KlineInterval.Hour;
                            if (args.Length > 3)
                            {
                                interval = args[3].ToKlineInterval();
                            }

                            _liveTokenSource = new CancellationTokenSource();

                            _klineClient = _serviceProvider.GetService<IKlineWebSocketClient>();
                            _klineClient.Kline += OnKlineEvent;

                            _liveTask = Task.Run(() => _klineClient.SubscribeAsync(symbol, interval, _liveTokenSource.Token));

                            lock (_consoleSync)
                            {
                                Console.WriteLine();
                                Console.WriteLine($"  ...live kline feed enabled for symbol: {symbol}, interval: {interval} ...use 'live off' to disable.");
                            }
                        }
                        else if (endpoint.Equals("trades", StringComparison.OrdinalIgnoreCase))
                        {
                            if (_liveTask != null)
                            {
                                lock (_consoleSync)
                                {
                                    Console.WriteLine($"! A live task is currently active ...use 'live off' to disable.");
                                }
                                continue;
                            }

                            _liveTokenSource = new CancellationTokenSource();

                            _tradesClient = _serviceProvider.GetService<ITradesWebSocketClient>();
                            _tradesClient.AggregateTrade += OnAggregateTradeEvent;

                            _liveTask = Task.Run(() => _tradesClient.SubscribeAsync(symbol, _liveTokenSource.Token));

                            lock (_consoleSync)
                            {
                                Console.WriteLine();
                                Console.WriteLine($"  ...live trades feed enabled for symbol: {symbol} ...use 'live off' to disable.");
                            }
                        }
                        else if (endpoint.Equals("account", StringComparison.OrdinalIgnoreCase))
                        {
                            if (_liveTask != null)
                            {
                                lock (_consoleSync)
                                {
                                    Console.WriteLine($"! A live task is currently active ...use 'live off' to disable.");
                                }
                                continue;
                            }

                            _liveTokenSource = new CancellationTokenSource();

                            _userDataClient = _serviceProvider.GetService<IUserDataWebSocketClient>();
                            _userDataClient.AccountUpdate += OnAccountUpdateEvent;
                            _userDataClient.OrderUpdate += OnOrderUpdateEvent;
                            _userDataClient.TradeUpdate += OnTradeUpdateEvent;

                            _liveTask = Task.Run(() => _userDataClient.SubscribeAsync(_user, _liveTokenSource.Token));

                            lock (_consoleSync)
                            {
                                Console.WriteLine();
                                Console.WriteLine($"  ...live account feed enabled ...use 'live off' to disable.");
                            }
                        }
                        else if (endpoint.Equals("off", StringComparison.OrdinalIgnoreCase))
                        {
                            await DisableLiveTask();
                        }
                        else
                        {
                            lock (_consoleSync)
                            {
                                Console.WriteLine($"! Unrecognized Command: \"{stdin}\"");
                                PrintHelp();
                            }
                            continue;
                        }
                    }
                    // Limit
                    else if (stdin.StartsWith("limit", StringComparison.OrdinalIgnoreCase))
                    {
                        var args = stdin.Split(' ');

                        if (args.Length < 5)
                        {
                            lock (_consoleSync)
                                Console.WriteLine("A side, symbol, quantity and price are required.");
                            continue;
                        }

                        if (!Enum.TryParse(typeof(OrderSide), args[1], true, out var side))
                        {
                            lock (_consoleSync)
                                Console.WriteLine("A valid order side is required ('buy' or 'sell').");
                            continue;
                        }

                        var symbol = args[2];

                        if (!decimal.TryParse(args[3], out var quantity) || quantity <= 0)
                        {
                            lock (_consoleSync)
                                Console.WriteLine("A quantity greater than 0 is required.");
                            continue;
                        }

                        if (!decimal.TryParse(args[4], out var price) || price <= 0)
                        {
                            lock (_consoleSync)
                                Console.WriteLine("A price greater than 0 is required.");
                            continue;
                        }

                        var clientOrder = new LimitOrder()
                        {
                            Symbol = symbol,
                            Side = (OrderSide)side,
                            Quantity = quantity,
                            Price = price,
                            IsTestOnly = _isOrdersTestOnly // *** NOTICE *** 
                        };

                        var order = await _api.PlaceAsync(_user, clientOrder, _recvWindow, token);

                        if (order != null)
                        {
                            lock (_consoleSync)
                            {
                                Console.WriteLine($"{(clientOrder.IsTestOnly ? "TEST " : "")}>> Limit order (ID: {order.Id}) placed for {order.OriginalQuantity.ToString("0.00000000")} {order.Symbol} @ {order.Price.ToString("0.00000000")}.");
                            }
                        }
                    }
                    // Market
                    else if (stdin.StartsWith("market", StringComparison.OrdinalIgnoreCase))
                    {
                        var args = stdin.Split(' ');

                        if (args.Length < 4)
                        {
                            lock (_consoleSync)
                                Console.WriteLine("A side, symbol, and quantity are required.");
                            continue;
                        }

                        if (!Enum.TryParse(typeof(OrderSide), args[1], true, out var side))
                        {
                            lock (_consoleSync)
                                Console.WriteLine("A valid order side is required ('buy' or 'sell').");
                            continue;
                        }

                        var symbol = args[2];

                        if (!decimal.TryParse(args[3], out var quantity) || quantity <= 0)
                        {
                            lock (_consoleSync)
                                Console.WriteLine("A quantity greater than 0 is required.");
                            continue;
                        }

                        var clientOrder = new MarketOrder()
                        {
                            Symbol = symbol,
                            Side = (OrderSide)side,
                            Quantity = quantity,
                            IsTestOnly = _isOrdersTestOnly // *** NOTICE *** 
                        };

                        var order = await _api.PlaceAsync(_user, clientOrder, _recvWindow, token);

                        if (order != null)
                        {
                            lock (_consoleSync)
                            {
                                Console.WriteLine($"{(clientOrder.IsTestOnly ? "~ TEST ~ " : "")}>> Market order (ID: {order.Id}) placed for {order.OriginalQuantity.ToString("0.00000000")} {order.Symbol} @ {order.Price.ToString("0.00000000")}.");
                            }
                        }
                    }
                    // Orders
                    else if (stdin.StartsWith("orders", StringComparison.OrdinalIgnoreCase))
                    {
                        if (_user == null)
                        {
                            PrintApiNotice();
                            continue;
                        }

                        var args = stdin.Split(' ');

                        string symbol = Symbol.BTC_USDT;
                        bool openOrders = false;
                        int limit = 10;

                        if (args.Length > 1)
                        {
                            if (!int.TryParse(args[1], out limit))
                            {
                                symbol = args[1];
                                limit = 10;
                            }
                        }

                        if (args.Length > 2)
                        {
                            if (!int.TryParse(args[2], out limit))
                            {
                                if (args[2].Equals("open", StringComparison.OrdinalIgnoreCase))
                                    openOrders = true;

                                limit = 10;
                            }
                        }

                        var orders = openOrders
                            ? await _api.GetOpenOrdersAsync(_user, symbol, _recvWindow, token)
                            : await _api.GetOrdersAsync(_user, symbol, limit: limit, recvWindow: _recvWindow, token: token);

                        lock (_consoleSync)
                        {
                            Console.WriteLine();
                            if (!orders.Any())
                            {
                                Console.WriteLine("[None]");
                            }
                            else
                            {
                                foreach (var order in orders)
                                {
                                    Console.WriteLine($"  {order.Symbol.PadLeft(8)} - {order.Type.ToString().PadLeft(6)} - {order.Side.ToString().PadLeft(4)} - {order.OriginalQuantity.ToString("0.00000000")} @ {order.Price.ToString("0.00000000")} - {order.Status.ToString()}  [ID: {order.Id}]");
                                }
                            }
                            Console.WriteLine();
                        }
                    }
                    // Order
                    else if (stdin.StartsWith("order", StringComparison.OrdinalIgnoreCase))
                    {
                        if (_user == null)
                        {
                            PrintApiNotice();
                            continue;
                        }

                        var args = stdin.Split(' ');

                        if (args.Length < 3)
                        {
                            Console.WriteLine("A symbol and order ID are required.");
                            continue;
                        }

                        var symbol = args[1];

                        string clientOrderId = null;

                        if (!long.TryParse(args[2], out var id))
                        {
                            clientOrderId = args[2];
                        }
                        else if (id < 0)
                        {
                            Console.WriteLine("An order ID not less than 0 is required.");
                            continue;
                        }

                        if (args.Length > 3 && args[3].Equals("cancel", StringComparison.OrdinalIgnoreCase))
                        {
                            var cancelOrderId = clientOrderId != null
                               ? await _api.CancelOrderAsync(_user, symbol, clientOrderId, recvWindow: _recvWindow, token: token)
                               : await _api.CancelOrderAsync(_user, symbol, id, recvWindow: _recvWindow, token: token);

                            lock (_consoleSync)
                            {
                                Console.WriteLine();
                                Console.WriteLine($"Cancel Order ID: {cancelOrderId}");
                                Console.WriteLine();
                            }
                        }
                        else
                        {
                            var order = clientOrderId != null
                                ? await _api.GetOrderAsync(_user, symbol, clientOrderId, _recvWindow, token)
                                : await _api.GetOrderAsync(_user, symbol, id, _recvWindow, token);

                            lock (_consoleSync)
                            {
                                Console.WriteLine();
                                if (order == null)
                                {
                                    Console.WriteLine("[Not Found]");
                                }
                                else
                                {
                                    Console.WriteLine($"  {order.Symbol.PadLeft(8)} - {order.Type.ToString().PadLeft(6)} - {order.Side.ToString().PadLeft(4)} - {order.OriginalQuantity.ToString("0.00000000")} @ {order.Price.ToString("0.00000000")} - {order.Status.ToString()}  [ID: {order.Id}]");
                                }
                                Console.WriteLine();
                            }
                        }
                    }
                    // Account
                    else if (stdin.Equals("account", StringComparison.OrdinalIgnoreCase)
                          || stdin.Equals("balances", StringComparison.OrdinalIgnoreCase)
                          || stdin.Equals("positions", StringComparison.OrdinalIgnoreCase))
                    {
                        if (_user == null)
                        {
                            PrintApiNotice();
                            continue;
                        }

                        var account = await _api.GetAccountAsync(_user, _recvWindow, token);

                        Display(account);
                    }
                    // My Trades
                    else if (stdin.StartsWith("mytrades", StringComparison.OrdinalIgnoreCase))
                    {
                        if (_user == null)
                        {
                            PrintApiNotice();
                            continue;
                        }

                        var args = stdin.Split(' ');

                        string symbol = Symbol.BTC_USDT;
                        int limit = 10;

                        if (args.Length > 1)
                        {
                            if (!int.TryParse(args[1], out limit))
                            {
                                symbol = args[1];
                                limit = 10;
                            }
                        }

                        if (args.Length > 2)
                        {
                            if (!int.TryParse(args[2], out limit))
                            {
                                limit = 10;
                            }
                        }

                        var trades = await _api.GetTradesAsync(_user, symbol, limit, recvWindow: _recvWindow, token: token);

                        lock (_consoleSync)
                        {
                            Console.WriteLine();
                            if (!trades.Any())
                            {
                                Console.WriteLine("[None]");
                            }
                            else
                            {
                                foreach (var trade in trades)
                                {
                                    Console.WriteLine($"  {trade.Time().ToLocalTime()} - {trade.Symbol.PadLeft(8)} - {(trade.IsBuyer ? "Buy" : "Sell").PadLeft(4)} - {(trade.IsMaker ? "Maker" : "Taker")} - {trade.Quantity.ToString("0.00000000")} @ {trade.Price.ToString("0.00000000")}{(trade.IsBestPriceMatch ? "*" : " ")} - Fee: {trade.Commission.ToString("0.00000000")} {trade.CommissionAsset} [ID: {trade.Id}]");
                                }
                            }
                            Console.WriteLine();
                        }
                    }
                    // Deposits
                    else if (stdin.StartsWith("deposits", StringComparison.OrdinalIgnoreCase))
                    {
                        if (_user == null)
                        {
                            PrintApiNotice();
                            continue;
                        }

                        var args = stdin.Split(' ');

                        string asset = null;
                        if (args.Length > 1)
                        {
                            asset = args[1];
                        }

                        var deposits = await _api.GetDepositsAsync(_user, asset, recvWindow: _recvWindow, token: token);

                        lock (_consoleSync)
                        {
                            Console.WriteLine();
                            if (!deposits.Any())
                            {
                                Console.WriteLine("[None]");
                            }
                            else
                            {
                                foreach (var deposit in deposits)
                                {
                                    Console.WriteLine($"  {deposit.Time().ToLocalTime()} - {deposit.Asset.PadLeft(4)} - {deposit.Amount.ToString("0.00000000")} - Status: {deposit.Status}");
                                }
                            }
                            Console.WriteLine();
                        }
                    }
                    // Withdrawals
                    else if (stdin.StartsWith("withdrawals", StringComparison.OrdinalIgnoreCase))
                    {
                        if (_user == null)
                        {
                            PrintApiNotice();
                            continue;
                        }

                        var args = stdin.Split(' ');

                        string asset = null;
                        if (args.Length > 1)
                        {
                            asset = args[1];
                        }

                        var withdrawals = await _api.GetWithdrawalsAsync(_user, asset, null, 0, 0, _recvWindow, token);

                        lock (_consoleSync)
                        {
                            Console.WriteLine();
                            if (!withdrawals.Any())
                            {
                                Console.WriteLine("[None]");
                            }
                            else
                            {
                                foreach (var withdrawal in withdrawals)
                                {
                                    Console.WriteLine($"  {withdrawal.Time().ToLocalTime()} - {withdrawal.Asset.PadLeft(4)} - {withdrawal.Amount.ToString("0.00000000")} => {withdrawal.Address} - Status: {withdrawal.Status}");
                                }
                            }
                            Console.WriteLine();
                        }
                    }
                    // Withdraw
                    else if (stdin.StartsWith("withdraw", StringComparison.OrdinalIgnoreCase))
                    {
                        var args = stdin.Split(' ');

                        if (args.Length < 4)
                        {
                            lock (_consoleSync)
                                Console.WriteLine("An asset, address, and amount are required.");
                            continue;
                        }

                        var asset = args[1];

                        var address = args[2];

                        if (!decimal.TryParse(args[3], out var amount) || amount <= 0)
                        {
                            lock (_consoleSync)
                                Console.WriteLine("An amount greater than 0 is required.");
                            continue;
                        }

                        await _api.WithdrawAsync(_user, asset, address, amount, null, _recvWindow, token);

                        lock (_consoleSync)
                        {
                            Console.WriteLine();
                            Console.WriteLine($"  Withdraw request successful: {amount} {asset} => {address}");
                        }
                    }
                    // Test Orders
                    else if (stdin.StartsWith("test", StringComparison.OrdinalIgnoreCase))
                    {
                        var args = stdin.Split(' ');

                        string value = "on";

                        if (args.Length > 1)
                        {
                            value = args[1];
                        }

                        _isOrdersTestOnly = !value.Equals("off", StringComparison.OrdinalIgnoreCase);

                        lock (_consoleSync)
                        {
                            Console.WriteLine();
                            Console.WriteLine($"  Test orders: {(_isOrdersTestOnly ? "ON" : "OFF")}");
                            if (!_isOrdersTestOnly)
                                Console.WriteLine($"  !! Market and Limit orders WILL be placed !!");
                            Console.WriteLine();
                        }
                    }
                    // Debug
                    else if (stdin.StartsWith("debug", StringComparison.OrdinalIgnoreCase))
                    {
                        var args = stdin.Split(' ');

                        // ...for development testing only...

                        lock (_consoleSync)
                        {
                            Console.WriteLine();
                            Console.WriteLine($"  Done.");
                            Console.WriteLine();
                        }
                    }
                    else
                    {
                        lock (_consoleSync)
                        {
                            Console.WriteLine($"! Unrecognized Command: \"{stdin}\"");
                            PrintHelp();
                        }
                        continue;
                    }
                }
                catch (Exception e)
                {
                    lock (_consoleSync)
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

        private static async Task DisableLiveTask()
        {
            _liveTokenSource?.Cancel();

            // Wait for live task to complete.
            if (_liveTask != null)
                await _liveTask;

            _liveOrderBook?.Dispose();
            _klineClient?.Dispose();
            _tradesClient?.Dispose();
            _userDataClient?.Dispose();

            _liveTokenSource?.Dispose();

            if (_liveOrderBook != null)
            {
                lock (_consoleSync) 
                {
                    Console.WriteLine();
                    Console.WriteLine($"  ...live order book feed disabled.");
                }
            }
            _liveOrderBook = null;

            if (_klineClient != null)
            {
                lock (_consoleSync)
                {
                    Console.WriteLine();
                    Console.WriteLine($"  ...live kline feed disabled.");
                }
            }
            _klineClient = null;

            if (_tradesClient != null)
            {
                lock (_consoleSync)
                {
                    Console.WriteLine();
                    Console.WriteLine($"  ...live trades feed disabled.");
                }
            }
            _tradesClient = null;

            if (_userDataClient != null)
            {
                lock (_consoleSync)
                {
                    Console.WriteLine();
                    Console.WriteLine($"  ...live account feed disabled.");
                }
            }
            _userDataClient = null;

            _liveTokenSource = null;
            _liveTask = null;
        }

        private static void OnOrderBookUpdated(object sender, OrderBookUpdateEventArgs e)
        {
            // NOTE: object 'sender' is IDepthOfMarket (live order book)...
            //       e.OrderBook is a clone (snapshot) of the live order book.
            var top = e.OrderBook.Top;

            lock (_consoleSync)
            {
                Console.WriteLine($"    {top.Symbol}  -  Bid: {top.Bid.Price.ToString(".00")}  |  {top.MidMarketPrice().ToString(".0000")}  |  Ask: {top.Ask.Price.ToString(".00")}  -  Spread: {top.Spread().ToString(".00")}");
            }
        }

        private static void OnKlineEvent(object sender, KlineEventArgs e)
        {
            lock (_consoleSync)
            {
                Console.WriteLine($"    {e.Candlestick.Symbol}  -  O: {e.Candlestick.Open.ToString("0.00000000")}  |  H: {e.Candlestick.High.ToString("0.00000000")}  |  L: {e.Candlestick.Low.ToString("0.00000000")}  |  C: {e.Candlestick.Close.ToString("0.00000000")}{(e.IsFinal ? "*" : "")}");
            }
        }

        private static void OnAggregateTradeEvent(object sender, AggregateTradeEventArgs e)
        {
            lock (_consoleSync)
            {
                Console.WriteLine($"  {e.Trade.Time().ToLocalTime()} - {e.Trade.Symbol.PadLeft(8)} - {(e.Trade.IsBuyerMaker ? "Sell" : "Buy").PadLeft(4)} - {e.Trade.Quantity.ToString("0.00000000")} @ {e.Trade.Price.ToString("0.00000000")}{(e.Trade.IsBestPriceMatch ? "*" : " ")} - [ID: {e.Trade.Id}]");
            }
        }

        private static void OnAccountUpdateEvent(object sender, AccountUpdateEventArgs e)
        {
            Display(e.Account);
        }

        private static void OnOrderUpdateEvent(object sender, OrderUpdateEventArgs e)
        {
            lock (_consoleSync)
            {
                Console.WriteLine();
                Console.WriteLine($"Order [{e.Order.Id}] updated: {e.ExecutionType}"); // TODO
                Console.WriteLine();
            }
        }

        private static void OnTradeUpdateEvent(object sender, TradeUpdateEventArgs e)
        {
            lock (_consoleSync)
            {
                Console.WriteLine();
                Console.WriteLine($"Trade [{e.Trade.Id}] updated."); // TODO
                Console.WriteLine();
            }
        }

        private static void Display(Account account)
        {
            lock (_consoleSync)
            {
                Console.WriteLine($"    Maker Commission:  {account.Commissions.Maker.ToString().PadLeft(3)} %");
                Console.WriteLine($"    Taker Commission:  {account.Commissions.Taker.ToString().PadLeft(3)} %");
                Console.WriteLine($"    Buyer Commission:  {account.Commissions.Buyer.ToString().PadLeft(3)} %");
                Console.WriteLine($"    Seller Commission: {account.Commissions.Seller.ToString().PadLeft(3)} %");
                Console.WriteLine($"    Can Trade:    {(account.Status.CanTrade ? "Yes" : "No").PadLeft(3)}");
                Console.WriteLine($"    Can Withdraw: {(account.Status.CanWithdraw ? "Yes" : "No").PadLeft(3)}");
                Console.WriteLine($"    Can Deposit:  {(account.Status.CanDeposit ? "Yes" : "No").PadLeft(3)}");
                Console.WriteLine();
                Console.WriteLine($"    Balances (only amounts > 0):");

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
