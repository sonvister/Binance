# Binance ![](https://github.com/sonvister/Binance/blob/master/logo.png?raw=true)
A full-featured .NET Standard 2.0 **[Binance API](https://www.binance.com/restapipub.html)** facade designed for ease of use.

[![](https://img.shields.io/github/last-commit/sonvister/Binance.svg)](https://github.com/sonvister/Binance)

## Installation
Using [Nuget](https://www.nuget.org/packages/Binance/) Package Manager:
```
PM> Install-Package Binance
```
[![](https://img.shields.io/nuget/v/Binance.svg)](https://www.nuget.org/packages/Binance)\
[![](https://img.shields.io/nuget/dt/Binance.svg)](https://www.nuget.org/packages/Binance)

## Features
* **Complete** implementation of [Binance API](https://www.binance.com/restapipub.html) including latest deposit/withdrawal features and WebSocket clients. 
* **Simple** API abstraction using domain/value objects that do not expose underlying (*HTTP/REST*) behavior.
* Consistent use of **domain models** whether you're querying the API or using real-time WebSocket client events.
* Customizable **dual-layer API** with access to JSON responses (*low-level*) or deserialized domain/value objects.
* API exceptions provide the Binance server response **ERROR code and message** for easier troubleshooting.
* Unique implementation supports **multiple users** and requires user authentication only where necessary.
* Web API interface includes automatic **rate limiting** and system-to-server **time synchronization** for reliability.
* Easy-to-use **WebSocket endpoint clients** and various ready-to-use **caching** implementations (*w/ events*).
* Low-level API utilizes a single, cached HttpClient for performance (*when used as singleton as in DI framework*).
* **Limited dependencies** and use of Microsoft extensions for **dependency injection**, **logging**, and **options**.
* .NET Core **sample applications** including live displays of market depth, trades, and candlesticks for a symbol.

## Getting Started
### General Information
- All timestamp related fields are in milliseconds (Unix time).
- All `IEnumerable<>` data is returned in **ascending** chronological order (oldest first, newest last).

### How To
  - [Verify connection to the Binance server](#connectivity). (*minimal examples*)
    ##### Market Data (*public*)
- [Get the market depth (order book) for a symbol](#order-book).
- [Maintain a real-time order book cache for a symbol](#order-book-cache).
- [Get the aggregate trades for a symbol](#trades).
- [Maintain a real-time trade history cache for a symbol](#aggregate-trades-cache).
- [Get the candlesticks for a symbol](#candlesticks).
- [Maintain a real-time price chart cache for a symbol](#candlesticks-cache).
- [Get the 24-hour statistics for a symbol](#24-hour-statistics).
- [Get current prices for all symobls for price ticker](#prices).
- [Get best price and quantity on the order book for all symobls](#order-book-tops).
    ##### Account (*private - API Key and Secret required*)
- [Place a LIMIT order](#limit-order).
- [Place a MARKET order](#market-order).
- [Place a TEST order to verify client order properties](#test-order).
- [Look-up an existing order to check status](#query-an-order).
- [Cancel an open order](#cancel-an-order).
- [Get all open orders for a symbol](#open-orders).
- [Get all orders for a symbol](#orders).
- [Get current account information](#account-information).
- [Get account trades for a symbol](#account-trades).
- [Submit a withdraw request](#withdraw).
- [Get deposit history](#deposit-history).
- [Get withdraw history](#withdraw-history).
- [Donate BTC to the creator of this library](#withdraw).

## API Method Reference
### Connectivity
##### *Minimal* Examples
**NOTE**: C# 7.1 is required for async Main (*set language version in project advanced build properties*).

- [Minimal](samples/BinanceConsoleApp/Examples/MinimalWithDependencyInjection.cs) with dependency injection (*recommended*).
- [Minimal](samples/BinanceConsoleApp/Examples/MinimalWithoutDependencyInjection.cs) without dependency injection.

#### Ping
```c#
    var successful = await api.PingAsync();
```
Sample console application [example](samples/BinanceConsoleApp/Controllers/Ping.cs).

#### Server Time
```c#
    var time = await api.GetTimeAsync(); // UTC date/time.
    var timestamp = await api.GetTimestampAsync(); // Unix time milliseconds.
```
Sample console application [example](samples/BinanceConsoleApp/Controllers/GetTime.cs).

### Market Data
There are pre-defined constant symbols in the static `Symbol` class.
#### Order Book
Get the order book (depth of market) for a symbol with optional limit [5, 10, 20, 50, 100, 200, 500].
```c#
    var book = await api.GetOrderBookAsync(Symbol.BTC_USDT);
```
Sample console application [example](samples/BinanceConsoleApp/Controllers/GetOrderBook.cs).

#### Trades
Get compressed/aggregate trades for a symbol with optional limit [1-500].
```c#
    var trades = await api.GetTradesAsync(Symbol.BTC_USDT);
```
Sample console application [example with limit](samples/BinanceConsoleApp/Controllers/GetAggregateTrades.cs), [example from trade ID](samples/BinanceConsoleApp/Controllers/GetAggregateTradesFrom.cs), [example with time range](samples/BinanceConsoleApp/Controllers/GetAggregateTradesIn.cs).

#### Candlesticks
Get candlesticks for a symbol with optional limit [1-500].
```c#
    var candles = await api.GetCandlesticksAsync(Symbol.BTC_USDT, KlineInterval.Hour);
```
Sample console application [example with limit](samples/BinanceConsoleApp/Controllers/GetCandlesticks.cs), [example with time range](samples/BinanceConsoleApp/Controllers/GetCandlesticksIn.cs).

#### 24-hour Statistics
Get the 24-hour statistics for a symbol.
```c#
    var stats = await api.Get24hrStatsAsync(Symbol.BTC_USDT);
```
Sample console application [example](samples/BinanceConsoleApp/Controllers/Get24HourStatistics.cs).

#### Prices
Get current prices for all symbols.
```c#
    var prices = await api.GetPricesAsync();
```
Sample console application [example](samples/BinanceConsoleApp/Controllers/GetPrices.cs).

#### Order Book Tops
Get best (top) price and quantity on the order book for all symbols.
```c#
    var tops = await api.GetOrderBookTopsAsync();
```
Sample console application [example](samples/BinanceConsoleApp/Controllers/GetOrderBookTops.cs).

#### Real-time Caching
##### Order Book Cache
Utilize an `IDepthWebSocketClient` (internal to `IOrderBookCache`) to create a real-time, synchronized order book for a symbol. \
Refer to the BinanceMarketDepth sample for an [additional example](samples/BinanceMarketDepth/Program.cs).
```c#
    using (var cache = serviceProvider.GetService<IOrderBookCache>())
    {
        cache.Update += OnUpdateEvent; // optionally, subscribe to update events.
        
        var task = Task.Run(() => cache.SubscribeAsync(Symbol.BTC_USDT, (e) =>
        {
            // optionally, use an inline event handler.
        }, cts.Token)); // starts synchronization.
        
        // ...
        
        var price = cache.OrderBook.Top.Bid.Price; // access latest order book (thread-safe).
        var book = cache.OrderBook; // keep a static (snapshot) reference of order book.
        
        // ...
        
        cts.Cancel(); // end the task.
        await task; // wait for task to complete.
    }
```
```
void OnUpdateEvent(object sender, OrderBookCacheEventArgs e)
{
    // Event has an immutable copy of the order book.
    var price = e.OrderBook.Top.Bid.Price;
}
```
##### Aggregate Trades Cache
Utilize an `ITradesWebSocketClient` (internal to `IAggregateTradesCache`) to create a real-time, synchronized trade history for a symbol. \
Refer to the BinanceTradeHistory sample for an [additional example](samples/BinanceTradeHistory/Program.cs).
```c#
    using (var cache = serviceProvider.GetService<IAggregateTradeCache>())
    {
        cache.Update += OnUpdateEvent; // optionally, subscribe to update events.
        
        var task = Task.Run(() => cache.SubscribeAsync(Symbol.BTC_USDT, (e) =>
        {
            // optionally, use an inline event handler.
        }, cts.Token)); // starts synchronization.
        
        // ...
        
        var trades = cache.Trades; // access latest aggregate trades (thread-safe).

        // ...
        
        cts.Cancel(); // end the task.
        await task; // wait for task to complete.
    }
```
```
void OnUpdateEvent(object sender, AggregateTradesCacheEventArgs e)
{
    // Event has an immutable copy of aggregate trades.
    var trades = e.Trades.
}
```
##### Candlesticks Cache
Utilize an `IKlineWebSocketClient` (internal to `ICandlesticksCache`) to create a real-time, synchronized price chart for a symbol. \
Refer to the BinancePriceChart sample for an [additional example](samples/BinancePriceChart/Program.cs).

```c#
    using (var cache = serviceProvider.GetService<ICandlesticksCache>())
    {
        cache.Update += OnUpdateEvent; // optionally, subscribe to update events.
        
        var task = Task.Run(() => cache.SubscribeAsync(Symbol.BTC_USDT, KlineInterval.Hour, (e) =>
        {
            // optionally, use an inline event handler.
        }, cts.Token)); // starts synchronization.
        
        // ...
        
        var candlesticks = cache.Candlestics; // access latest candlesticks (thread-safe).

        // ...
        
        cts.Cancel(); // end the task.
        await task; // wait for task to complete.
    }
```
```
void OnUpdateEvent(object sender, AggregateTradesCacheEventArgs e)
{
    // Event has an immutable copy of candlesticks.
    var candlesticks = e.Candlesticks.
}
```
##### Account Info Cache
Utilize an `IUserDataWebSocketClient` (internal to `IAccountInfoCache`) to create a real-time, synchronized account profile for a user. \
Refer to the following for an [additional example](samples/BinanceConsoleApp/Examples/AccountBalancesExample.cs).

```c#
    using (var cache = serviceProvider.GetService<IAccountInfoCache>())
    {
        cache.Update += OnUpdateEvent; // optionally, subscribe to update events.
        
        var task = Task.Run(() => cache.SubscribeAsync(user, (e) =>
        {
            // optionally, use an inline event handler.
        }, cts.Token)); // starts synchronization.
        
        // ...
        
        var accountInfo = cache.AccountInfo; // access latest candlesticks (thread-safe).

        // ...
        
        cts.Cancel(); // end the task.
        await task; // wait for task to complete.
    }
```
```
void OnUpdateEvent(object sender, AccountInfoCacheEventArgs e)
{
    // Event has an immutable copy of account info.
    var accountInfo = e.AccountInfo.
}
```

### Account
#### Authentication
Create a user authentication instance `IBinanceApiUser` with your Binance account API **Key** and **Secret** (optional).
```c#
    var user = new BinanceApiUser("<Your API Key>", <your API Secret>);
```
NOTE: User authentication is method injected -- only where required -- so a single Binance API instance (with a single `HttpClient`) can support multiple Binance users.

##### Console Application Prerequisites
When using the `BinanceConsoleApp` sample you may see this message when accessing non-public API methods:

> To access some Binance endpoint features, your **API Key and Secret** may be required.
> You can either modify the '**ApiKey**' and '**ApiSecret**' configuration values in **appsettings.json**.
> Or use the following commands to configure the .NET user secrets for the project:
> \
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;`dotnet user-secrets set BinanceApiKey <your api key>` \
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;`dotnet user-secrets set BinanceApiSecret <your api secret>`
> \
> For more information: <https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets>

#### Order Placement Process and recvWindow
![](https://github.com/sonvister/Binance/blob/master/images/order-placement.png?raw=true)

#### Exception Handling
This code demonstrates how to handle exceptions from Binance API methods.

NOTE: Handling exceptions with this level of precision is only applicable to processes that an application may retry should the first attempt fail (*e.g. new order placement*).

```c#
try
{
    using (var api = new BinanceApi())
    using (var user = new BinanceApiUser("api-key", "api-secret"))
    {
        var order = await api.PlaceAsync(user, new MarketOrder()
        {
            Symbol = Symbol.BTC_USDT,
            Side = OrderSide.Sell,
            Quantity = 1
        });
    }
}
catch (BinanceUnknownStatusException) // ...is BinanceHttpException (w/ status code 504)
{
    // HTTP 504 return code is used when the API successfully sent the message but not get a response within the timeout period.
    // It is important to NOT treat this as a failure; the execution status is UNKNOWN and could have been a success.
}
catch (BinanceHttpException e) // ...is BinanceApiException
{
    // The request failed; handle exception based on HTTP status code.
    if (e.IsClientError())
    {
        // HTTP 4XX return codes are used for for malformed requests; the issue is on the sender's side.
    }
    else if (e.IsServerError())
    {
        // HTTP 5XX return codes are used for internal errors; the issue is on Binance's side.
    }
    else
    {
        // Other HTTP return codes... 
    }
}
catch (BinanceApiException)
{
    // Respond to other Binance API exceptions (typically JSON deserialization failures).
}
catch (Exception)
{
    // Other exceptions...
}
```

#### Limit Order
Create and place a new *LIMIT* order. \
NOTE: Client orders are created to serve as a mutable order placeholder, only after that client order is placed does an immutable Order exist.
```c#
    using (var user = new BinanceApiUser("api-key", "api-secret"))
    {
        var order = await api.PlaceAsync(user, new LimitOrder()
        {
            Symbol = Symbol.BTC_USDT,
            Side = OrderSide.Buy,
            Quantity = 1,
            Price = 1000
        });
    }
```
Sample console application [example](samples/BinanceConsoleApp/Controllers/PlaceLimitOrder.cs).

#### Market Order
Create and place a new *MARKET* order. You do not set a price for Market orders. \
NOTE: Client orders are created to serve as a mutable order placeholder, only after that client order is placed does an immutable Order exist.
```c#
    using (var user = new BinanceApiUser("api-key", "api-secret"))
    {
        var order = await api.PlaceAsync(user, new MarketOrder()
        {
            Symbol = Symbol.BTC_USDT,
            Side = OrderSide.Sell,
            Quantity = 1
        });
    }
```
Sample console application [example](samples/BinanceConsoleApp/Controllers/PlaceMarketOrder.cs).

#### Test Order
Create and place a new *TEST* order. An exception will be thrown if the order placement test fails.
```c#
    using (var user = new BinanceApiUser("api-key", "api-secret"))
    {
        try
        {
            await api.TestPlaceAsync(user, new MarketOrder()
            {
                Symbol = Symbol.BTC_USDT,
                Side = OrderSide.Sell,
                Quantity = 1
            });
        }
        catch (BinanceApiException) { }
    }
```
Sample console application [example LIMIT order](samples/BinanceConsoleApp/Controllers/PlaceLimitOrder.cs), [example MARKET order](samples/BinanceConsoleApp/Controllers/PlaceMarketOrder.cs).

#### Query an Order
Get an order to determine current status. Order lookup requires an order instance or the combination of a symbol and the order ID or original client order ID. If an order instance is provided, it will be updated in place in addition to being returned.
```c#
    var order = await api.GetAsync(order); // use to update status in place.
    // or...
    var order = await api.GetOrderAsync(user, Symbol.BTC_USDT, orderId);
    // or...
    var order = await api.GetOrderAsync(user, Symbol.BTC_USDT, clientOrderId);
```
Sample console application [example](samples/BinanceConsoleApp/Controllers/GetOrCancelOrder.cs).

#### Cancel an Order
Cancel an order. Order lookup requires an order instance or the combination of a symbol and the order ID or original client order ID.
```c#
    await api.CancelAsync(order);
    // or...
    await api.CancelOrderAsync(user, Symbol.BTC_USDT, orderId);
    // or...
    await api.CancelOrderAsync(user, Symbol.BTC_USDT, clientOrderId);
```
Sample console application [example](samples/BinanceConsoleApp/Controllers/GetOrCancelOrder.cs).

#### Open Orders
Get all open orders for a symbol with optional limit [1-500].
```c#
    var orders = await api.GetOpenOrdersAsync(user, Symbol.BTC_USDT);
```
Sample console application [example](samples/BinanceConsoleApp/Controllers/GetOrders.cs).

#### Orders
Get all orders; active, canceled, or filled with optional limit [1-500].
```c#
    var orders = await api.GetOrdersAsync(user, Symbol.BTC_USDT);
```
Sample console application [example](samples/BinanceConsoleApp/Controllers/GetOrders.cs).

#### Account Information
Get current account information.
```c#
    var account = await api.GetAccountInfoAsync(user);
```
Sample console application [example](samples/BinanceConsoleApp/Controllers/GetAccountInfo.cs).

#### Account Trades
Get trades for a specific account and symbol with optional limit [1-500].
```c#
    var account = await api.GetTradesAsync(user, Symbol.BTC_USDT);
```
Sample console application [example](samples/BinanceConsoleApp/Controllers/GetTrades.cs).

#### Withdraw
Submit a withdraw request... *optionally donate to me* :)
```c#
    await api.WithdrawAsync(user, Asset.BTC, "3JjG3tRR1dx98UJyNdpzpkrxRjXmPfQHk9", 0.01m);
```
Sample console application [example](samples/BinanceConsoleApp/Controllers/Withdraw.cs).

#### Deposit History
Get deposit history.
```c#
    var deposits = await api.GetDepositsAsync(user);
```
Sample console application [example](samples/BinanceConsoleApp/Controllers/GetDeposits.cs).

#### Withdraw History
Get withdraw history.
```c#
    var withdrawals = await api.GetWithdrawalsAsync(user);
```
Sample console application [example](samples/BinanceConsoleApp/Controllers/GetWithdrawals.cs).

### WebSocket Clients
#### Depth Endpoint
Get real-time depth update events.
```c#
    using (var client = serviceProvider.GetService<IDepthWebSocketClient>())
    {
        client.DepthUpdate += OnDepthUpdateEvent; // optional event subscribing.
        
        var task = Task.Run(() => client.SubscribeAsync(Symbol.BTC_USDT, (e) =>
        {
            // optional inline event handler.
        }, cts.Token));
        
        // ...

        cts.Cancel();
        await task;
    }
```
```
void OnDepthUpdateEvent(object sender, DepthUpdateEventArgs e)
{
    // ...
}
```

#### Kline Endpoint
Get real-time kline/candlestick events.
```c#
    using (var client = serviceProvider.GetService<IKlineWebSocketClient>())
    {
        client.Kline += OnKlineEvent; // optional event subscribing.
        
        var task = Task.Run(() => client.SubscribeAsync(Symbol.BTC_USDT, KlineInterval.Hour, (e) =>
        {
            // optional inline event handler.
        }, cts.Token));
        
        // ...
        
        cts.Cancel();
        await task;
    }
```
```
void OnKlineEvent(object sender, KlineEventArgs e)
{
    // ...
}
```

#### Trades Endpoint
Get real-time aggregate trade events.
```c#
    using (var client = serviceProvider.GetService<ITradesWebSocketClient>())
    {
        client.AggregateTrade += OnAggregateTradeEvent; // optional event subscribing.
        
        var task = Task.Run(() => client.SubscribeAsync(Symbol.BTC_USDT, (e) => 
        {
            // optional inline event handler.
        }, cts.Token));
        
        // ...
        
        cts.Cancel();
        await task;
    }
```
```
void OnAggregateTradeEvent(object sender, AggregateTradeEventArgs e)
{
    // ...
}
```

#### User Data Endpoint
Get real-time account update events.
```c#
    using (var client = serviceProvider.GetService<IUserDataWebSocketClient>())
    {
        // optional (preferred) event subscribing.
        client.AccountUpdate += OnAccountUpdateEvent;
        client.OrderUpdate += OnOrderUpdateEvent;
        client.TradeUpdate += OnTradeUpdateEvent;
        
        var task = Task.Run(() => client.SubscribeAsync(user, (e) => 
        {
            // optional inline event handler (where 'e' is the base UserDataEventArgs type).
        }, cts.Token));
        
        // ...
        
        cts.Cancel();
        await task;
    }
```
```
void OnAccountUpdateEvent(object sender, AccountUpdateEventArgs e)
{
    // ...
}
void OnOrderUpdateEvent(object sender, OrderUpdateEventArgs e)
{
    // ...
}
void OnTradeUpdateEvent(object sender, TradeUpdateEventArgs e)
{
    // ...
}
```
#### User Stream Control
##### Start User Stream
Start a new user data stream.
```c#
    var listenKey = await api.UserStreamStartAsync(user);
```

##### Keepalive User Stream
Ping a user data stream to prevent a timeout.
```c#
    await api.UserStreamKeepAliveAsync(user, listenKey);
```

##### Close User Stream
Close a user data stream.
```c#
    await api.UserStreamCloseAsync(user, listenKey);
```
