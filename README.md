# Binance ![](https://github.com/sonvister/Binance/blob/master/images/logo.png?raw=true)
A full-featured .NET Standard 2.0 **[Binance API](https://www.binance.com/restapipub.html)** facade designed for ease of use.

[![](https://img.shields.io/github/last-commit/sonvister/Binance.svg)](https://github.com/sonvister/Binance)

## Features
* **Complete** implementation of the [Binance API](https://www.binance.com/restapipub.html) including deposit/withdrawal features and WebSocket endpoints.
* A **simple** API abstraction using domain/value objects that do not expose underlying (*HTTP/REST*) behavior.
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
### Binance Sign-up
To use the (*non-public*) account related features of the API you must have a Binance account and create an API Key. \
Please use my Referral ID: **10899093** when you [Register](https://www.binance.com/register.html?ref=10899093).

&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;[![](https://github.com/sonvister/Binance/blob/master/images/register.png?raw=true)](https://www.binance.com/register.html?ref=10899093)

*NOTE*: An account is not required to access the public market data.

### Installation
Using [Nuget](https://www.nuget.org/packages/Binance/) Package Manager:
```
PM> Install-Package Binance
```
[![](https://img.shields.io/nuget/v/Binance.svg)](https://www.nuget.org/packages/Binance)\
[![](https://img.shields.io/nuget/dt/Binance.svg)](https://www.nuget.org/packages/Binance)

### How To
  - [Verify connection to the Binance server](#connectivity) (*minimal examples*).
    ##### Market Data (*public*)
- [Get the market depth (order book) for a symbol](#order-book).
- [Maintain a real-time order book cache for a symbol](#order-book-cache).
- [Get the aggregate trades for a symbol](#trades).
- [Maintain a real-time trade history cache for a symbol](#aggregate-trades-cache).
- [Get the candlesticks for a symbol](#candlesticks).
- [Maintain a real-time price chart cache for a symbol](#candlesticks-cache).
- [Get the 24-hour statistics for a symbol](#24-hour-statistics).
- [Get current prices for all symbols for a price ticker](#prices).
- [Get best price and quantity on the order book for all symbols](#order-book-tops).
- [Get a list of all *current* symbols](#symbols).
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
This library provides a complete implementation of the [Binance REST API](https://www.binance.com/restapipub.html) in the [`IBinanceApi`](src/Binance/Api/IBinanceApi.cs) interface. This high-level facade provides a simple abstraction to interact with the remote Binance REST API returning immutable domain objects and HTTP response status through the use of exceptions.

*NOTE*: The [`IBinanceApi`](src/Binance/Api/IBinanceApi.cs) interface is `IDisposable` and does not require an API Key or Secret for instantiation. [Authentication](#authentication) information is only required where necessary.
```c#
var api = serviceProvider.GetService<IBinanceApi>();
// or...
var api = new BinanceApi();
```
Alternatively, the low-level [`IBinanceJsonApi`](src/Binance/Api/Json/IBinanceJsonApi.cs) interface can be used with all the same capabilities, but returns JSON instead.
```c#
var jsonApi = serviceProvider.GetService<IBinanceJsonApi>();
// or...
var jsonApi = api.JsonApi; // ...property of IBinanceApi.
```
All API methods are asynchronous with an optional `CancellationToken` parameter.

### General Information
For consistency with the Binance REST API:
- All timestamp related fields are in milliseconds (Unix time).
- All `IEnumerable<>` data is returned in **ascending** chronological order (oldest first, newest last).

### Rate Limiting
Built into the [`IBinanceJsonApi`](src/Binance/Api/Json/IBinanceJsonApi.cs) implementation is an [`IRateLimiter`](src/Binance/Api/Json/IRateLimiter.cs) that ensures the API call rate doesn't exceed a configurable threshold. By default this threshold is set to 3 calls per second. This helps provide *fair* use of the Binance API and to prevent *potentially* being disconnected or blocked.

*NOTE*: Currently, the`PlaceOrderAsync` and `CancelOrderAsync` calls are not affected by the rate limiter.

### Configuration Options
The following classes are used to provide configurable options: [`BinanceJsonApiOptions`](src/Binance/Options/BinanceJsonApiOptions.cs) and [`UserDataWebSocketClientOptions`](src/Binance/Options/UserDataWebSocketClientOptions.cs). The `BinanceConsoleApp` demonstrates how these options can be configured using a JSON file.

### Connectivity
##### *Minimal* Examples
- [Minimal](samples/BinanceConsoleApp/Examples/MinimalWithDependencyInjection.cs) with dependency injection (*recommended*).
- [Minimal](samples/BinanceConsoleApp/Examples/MinimalWithoutDependencyInjection.cs) without dependency injection.

*NOTE*: C# 7.1 is required for `async Main()` (*set language version in project advanced build properties*).

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
There are pre-defined constant symbols in the static [`Symbol`](src/Binance/Symbol.cs) class.
#### Order Book
Get the [order book](src/Binance/Market/OrderBook.cs) (depth of market) for a symbol with optional limit [5, 10, 20, 50, 100, 200, 500].
```c#
var book = await api.GetOrderBookAsync(Symbol.BTC_USDT);
```
Sample console application [example](samples/BinanceConsoleApp/Controllers/GetOrderBook.cs).

#### Trades
Get compressed/[aggregate trades](src/Binance/Market/AggregateTrade.cs) for a symbol with optional limit [1-500]. \
Trades that fill at the same time, from the same order, with the same price will have an aggregate quantity.
```c#
var trades = await api.GetTradesAsync(Symbol.BTC_USDT);
```
Sample console application [example with limit](samples/BinanceConsoleApp/Controllers/GetAggregateTrades.cs), [example from trade ID](samples/BinanceConsoleApp/Controllers/GetAggregateTradesFrom.cs), [example with time range](samples/BinanceConsoleApp/Controllers/GetAggregateTradesIn.cs).

#### Candlesticks
Get [candlesticks](src/Binance/Market/Candlestick.cs) for a symbol with optional limit [1-500].
```c#
var candles = await api.GetCandlesticksAsync(Symbol.BTC_USDT, CandlestickInterval.Hour);
```
Sample console application [example with limit](samples/BinanceConsoleApp/Controllers/GetCandlesticks.cs), [example with time range](samples/BinanceConsoleApp/Controllers/GetCandlesticksIn.cs).

#### 24-hour Statistics
Get the [24-hour statistics](src/Binance/Market/Symbol24HourStatistics.cs) for a symbol.
```c#
var stats = await api.Get24HourStatisticsAsync(Symbol.BTC_USDT);
```
Sample console application [example](samples/BinanceConsoleApp/Controllers/Get24HourStatistics.cs).

#### Prices
Get current [prices](src/Binance/Market/SymbolPrice.cs) for all symbols.
```c#
var prices = await api.GetPricesAsync();
```
Sample console application [example](samples/BinanceConsoleApp/Controllers/GetPrices.cs).

#### Order Book Tops
Get best, [top price and quantity](src/Binance/Market/OrderBookTop.cs) on the order book for all symbols.
```c#
var tops = await api.GetOrderBookTopsAsync();
```
Sample console application [example](samples/BinanceConsoleApp/Controllers/GetOrderBookTops.cs).

#### Symbols
Get a list of all *current* symbols.
```c#
var symbols = await api.SymbolsAsync();
```
Sample console application [example](samples/BinanceConsoleApp/Controllers/Symbols.cs).


### Account
To use the following features of the API you must have a Binance account and create an API Key. If you haven't already created an account, please use my Referral ID: **10899093** when you [Sign Up](https://www.binance.com/register.html?ref=10899093).

#### Authentication
Create a user authentication instance ([`IBinanceApiUser`](src/Binance/Api/IBinanceApiUser.cs)) with your Binance account **API Key** and **Secret** (optional). The interface and implementation is `IDisposable` due to an internal `HMAC` used for signing requests (subsequently, the API Secret is *not stored* as a property in the [`BinanceApiUser`](src/Binance/Api/BinanceApiUser.cs) class privately or otherwise).
```c#
var user = new BinanceApiUser("<Your API Key>", "<your API Secret>");
```
*NOTE*: User authentication is method injected -- only where required -- so a single Binance API instance (with a single `HttpClient`) is capable of supporting multiple Binance users.

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
The following timing diagram illustrates the meaning and application of the `recvWindow` argument as well as the relationship of the high-level and low-level APIs.

*NOTE*: This diagram displays an ideal situation where your local time is synchronized with the Binance server time. This is not always so and is why there is a synchronization process which calculates an offset used during timestamp creation. As you can see, it is important for reliable use of the recvWindow argument -- and the API in general -- that timestamps are synchronized. The default recvWindow value, based on the latest Binance API documentation, is 5000 (*5 seconds*). The synchronization process cannot be disabled, but the rate at which the timestamp offset is refreshed can be controlled (*default period is arbitrarily set at 1 hour*).

![](https://github.com/sonvister/Binance/blob/master/images/order-placement.png?raw=true)

#### Exception Handling
This code demonstrates how to handle exceptions from the [`IBinanceApi`](src/Binance/Api/IBinanceApi.cs) methods.

NOTE: Handling exceptions with this level of precision is only applicable to processes that an application may retry should the first attempt fail (*e.g. order placement or withdraw requests*).

```c#
try
{
    using (var api = serviceProvider.GetService<IBinanceApi>())
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
Create and place a new [*LIMIT* order](src/Binance/Account/Orders/LimitOrder.cs).

NOTE: [Client orders](src/Binance/Account/Orders/ClientOrder.cs) are created to serve as a mutable order placeholder, only after the client order is placed successfully does an immutable [`Order`](src/Binance/Account/Orders/Order.cs) exist.
```c#
using (var user = new BinanceApiUser("api-key", "api-secret"))
{
    var order = await api.PlaceAsync(new LimitOrder(user)
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
Create and place a new [*MARKET* order](src/Binance/Account/Orders/MarketOrder.cs). You do not set a price for Market orders.

NOTE: [Client orders](src/Binance/Account/Orders/ClientOrder.cs) are created to serve as a mutable order placeholder, only after that client order is placed does an immutable [`Order`](src/Binance/Account/Orders/Order.cs) exist.
```c#
using (var user = new BinanceApiUser("api-key", "api-secret"))
{
    var order = await api.PlaceAsync(new MarketOrder(user)
    {
        Symbol = Symbol.BTC_USDT,
        Side = OrderSide.Sell,
        Quantity = 1
    });
}
```
Sample console application [example](samples/BinanceConsoleApp/Controllers/PlaceMarketOrder.cs).

#### Test Order
Create and *TEST* place a new order. An exception will be thrown if the order placement test fails.
```c#
using (var user = new BinanceApiUser("api-key", "api-secret"))
{
    try
    {
        await api.TestPlaceAsync(new MarketOrder(user)
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
Get an order to determine current status. [`Order`](src/Binance/Account/Orders/Order.cs) lookup requires an order instance or the combination of a symbol and the order ID or original client order ID. If an order instance is provided, it will be updated in place in addition to being returned.
```c#
var order = await api.GetAsync(order); // use to update status in place.
// or...
var order = await api.GetOrderAsync(user, Symbol.BTC_USDT, orderId);
// or...
var order = await api.GetOrderAsync(user, Symbol.BTC_USDT, clientOrderId);
```
Sample console application [example](samples/BinanceConsoleApp/Controllers/GetOrCancelOrder.cs).

#### Cancel an Order
Cancel an order. [`Order`](src/Binance/Account/Orders/Order.cs) lookup requires an order instance or the combination of a symbol and the order ID or original client order ID.
```c#
await api.CancelAsync(order);
// or...
await api.CancelOrderAsync(user, Symbol.BTC_USDT, orderId);
// or...
await api.CancelOrderAsync(user, Symbol.BTC_USDT, clientOrderId);
```
Sample console application [example](samples/BinanceConsoleApp/Controllers/GetOrCancelOrder.cs).

#### Open Orders
Get all open [`orders`](src/Binance/Account/Orders/Order.cs) for a symbol with optional limit [1-500].
```c#
var orders = await api.GetOpenOrdersAsync(user, Symbol.BTC_USDT);
```
Sample console application [example](samples/BinanceConsoleApp/Controllers/GetOrders.cs).

#### Orders
Get all [`orders`](src/Binance/Account/Orders/Order.cs); active, canceled, or filled with optional limit [1-500].
```c#
var orders = await api.GetOrdersAsync(user, Symbol.BTC_USDT);
```
Sample console application [example](samples/BinanceConsoleApp/Controllers/GetOrders.cs).

#### Account Information
Get current [account information](src/Binance/Account/AccountInfo.cs).
```c#
var account = await api.GetAccountInfoAsync(user);
```
Sample console application [example](samples/BinanceConsoleApp/Controllers/GetAccountInfo.cs).

#### Account Trades
Get [trades](src/Binance/Account/AccountTrade.cs) for a specific account and symbol with optional limit [1-500].
```c#
var account = await api.GetTradesAsync(user, Symbol.BTC_USDT);
```
Sample console application [example](samples/BinanceConsoleApp/Controllers/GetTrades.cs).

#### Withdraw
Submit a [withdraw request](src/Binance/Account/WithdrawRequest.cs) ...*optionally donate to me* :)
```c#
await api.WithdrawAsync(new WithdrawRequest(user)
{
    Asset = Asset.BTC,
    Address = "3JjG3tRR1dx98UJyNdpzpkrxRjXmPfQHk9",
    Amount = 0.01m
});
```
Sample console application [example](samples/BinanceConsoleApp/Controllers/Withdraw.cs).

#### Deposit History
Get [deposit](src/Binance/Account/Deposit.cs) history.
```c#
var deposits = await api.GetDepositsAsync(user);
```
Sample console application [example](samples/BinanceConsoleApp/Controllers/GetDeposits.cs).

#### Withdrawal History
Get [withdrawal](src/Binance/Account/Withdrawal.cs) history.
```c#
var withdrawals = await api.GetWithdrawalsAsync(user);
```
Sample console application [example](samples/BinanceConsoleApp/Controllers/GetWithdrawals.cs).

### WebSocket Clients
This library provides a complete implementation of the [Binance WebSocket API](https://www.binance.com/restapipub.html) in the following interfaces:

#### Depth Endpoint
Get real-time depth update events using [`IDepthWebSocketClient`](src/Binance/Api/WebSocket/IDepthWebSocketClient.cs).
```c#
var client = serviceProvider.GetService<IDepthWebSocketClient>();

client.DepthUpdate += OnDepthUpdateEvent; // optional event subscribing.
    
var task = Task.Run(() => client.SubscribeAsync(Symbol.BTC_USDT, evt =>
{
    // optional inline event handler.
}, cts.Token));

// ...

cts.Cancel();
await task;
```
```c#
void OnDepthUpdateEvent(object sender, DepthUpdateEventArgs evt)
{
    // ...
}
```
Optionally, you can use the `IDisposable` [`TaskController`](src/Binance/Utility/TaskController.cs) to encapsulate the `Task` and `CancellationTokenSource`.
```c#
var client = serviceProvider.GetService<IDepthWebSocketClient>();

using (var controller = new TaskController())
{
    controller.Begin(tkn => client.SubscribeAsync(Symbol.BTC_USDT,
        evt => { /* optional inline event handler. */ }, tkn),
        err => { /* optional inline exception handler. */ });

    // ...

    // NOTE: The encapsulated Task is canceled and awaited when the controller is disposed.
}
```

#### Candlestick Endpoint
Get real-time candlestick events using [`ICandlestickWebSocketClient`](src/Binance/Api/WebSocket/ICandlestickWebSocketClient.cs).
```c#
var client = serviceProvider.GetService<ICandlestickWebSocketClient>();

client.Candlestick += OnCandlestickEvent; // optional event subscribing.

var task = Task.Run(() => client.SubscribeAsync(Symbol.BTC_USDT, CandlestickInterval.Hour, evt =>
{
    // optional inline event handler.
}, cts.Token));

// ...

cts.Cancel();
await task;
```
```c#
void OnCandlestickEvent(object sender, CandlestickEventArgs evt)
{
    // ...
}
```
Optionally, you can use the `IDisposable` [`TaskController`](src/Binance/Utility/TaskController.cs) to encapsulate the `Task` and `CancellationTokenSource`.
```c#
var client = serviceProvider.GetService<ICandlestickWebSocketClient>();

using (var controller = new TaskController())
{
    controller.Begin(tkn => client.SubscribeAsync(Symbol.BTC_USDT, CandlestickInterval.Hour,
        evt => { /* optional inline event handler. */ }, tkn),
        err => { /* optional inline exception handler. */ });

    // ...

    // NOTE: The encapsulated Task is canceled and awaited when the controller is disposed.
}
```

#### Trades Endpoint
Get real-time aggregate trade events using [`ITradesWebSocketClient`](src/Binance/Api/WebSocket/ITradesWebSocketClient.cs).
```c#
var client = serviceProvider.GetService<ITradesWebSocketClient>();

client.AggregateTrade += OnAggregateTradeEvent; // optional event subscribing.

var task = Task.Run(() => client.SubscribeAsync(Symbol.BTC_USDT, evt => 
{
    // optional inline event handler.
}, cts.Token));

// ...

cts.Cancel();
await task;
}
```
```c#
void OnAggregateTradeEvent(object sender, AggregateTradeEventArgs evt)
{
    // ...
}
```
Optionally, you can use the `IDisposable` [`TaskController`](src/Binance/Utility/TaskController.cs) to encapsulate the `Task` and `CancellationTokenSource`.
```c#
var client = serviceProvider.GetService<ITradesWebSocketClient>();

using (var controller = new TaskController())
{
    controller.Begin(tkn => client.SubscribeAsync(Symbol.BTC_USDT,
        evt => { /* optional inline event handler. */ }, tkn),
        err => { /* optional inline exception handler. */ });

    // ...

    // NOTE: The encapsulated Task is canceled and awaited when the controller is disposed.
}
```

#### User Data Endpoint
Get real-time account update events using [`IUserDataWebSocketClient`](src/Binance/Api/WebSocket/IUserDataWebSocketClient.cs).
```c#
var client = serviceProvider.GetService<IUserDataWebSocketClient>();

// optional (preferred) event subscribing.
client.AccountUpdate += OnAccountUpdateEvent;
client.OrderUpdate += OnOrderUpdateEvent;
client.TradeUpdate += OnTradeUpdateEvent;

var task = Task.Run(() => client.SubscribeAsync(user, evt => 
{
    // optional inline event handler (where 'evt' is the base UserDataEventArgs type).
}, cts.Token));

// ...

cts.Cancel();
await task;
```
```c#
void OnAccountUpdateEvent(object sender, AccountUpdateEventArgs evt)
{
    // ...
}
void OnOrderUpdateEvent(object sender, OrderUpdateEventArgs evt)
{
    // ...
}
void OnTradeUpdateEvent(object sender, TradeUpdateEventArgs evt)
{
    // ...
}
```
Optionally, you can use the `IDisposable` [`TaskController`](src/Binance/Utility/TaskController.cs) to encapsulate the `Task` and `CancellationTokenSource`.
```c#
var client = serviceProvider.GetService<IUserDataWebSocketClient>();

using (var controller = new TaskController())
{
    controller.Begin(tkn => client.SubscribeAsync(user,
        evt => { /* optional inline event handler. */ }, tkn),
        err => { /* optional inline exception handler. */ });

    // ...

    // NOTE: The encapsulated Task is canceled and awaited when the controller is disposed.
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

#### Real-time Caching
The caching classes are high-level implementations that utilize the corresponding WebSocket client to provide a local copy of the order book, trade history, price chart, etc. for a symbol that is also updated in real-time. Applications are notified of updates via an event handler, callback, or both.

*NOTE*: Multiple event listener classes can be linked to the `...Cache` implementations though the `Update` event.

##### Order Book Cache
Use an [`IOrderBookCache`](src/Binance/Cache/IOrderBookCache.cs) (with an [`IDepthWebSocketClient`](src/Binance/Api/WebSocket/IDepthWebSocketClient.cs)) to create a real-time, synchronized order book for a symbol. Refer to the BinanceMarketDepth sample for an [additional example](samples/BinanceMarketDepth/Program.cs).
```c#
var cache = serviceProvider.GetService<IOrderBookCache>();

cache.Update += OnUpdateEvent; // optionally, subscribe to update events.

var task = Task.Run(() => cache.SubscribeAsync(Symbol.BTC_USDT, evt =>
{
    // optionally, use an inline event handler.
}, cts.Token)); // starts synchronization.

// ...

var price = cache.OrderBook.Top.Bid.Price; // access latest order book (thread-safe).
var book = cache.OrderBook; // keep a static (snapshot) reference of order book.

// ...

cts.Cancel(); // end the task.
await task; // wait for task to complete.
```
```c#
void OnUpdateEvent(object sender, OrderBookCacheEventArgs evt)
{
    // Event has an immutable copy of the order book.
    var price = evt.OrderBook.Top.Bid.Price;
}
```
Optionally, you can use the `IDisposable` [`TaskController`](src/Binance/Utility/TaskController.cs) to encapsulate the `Task` and `CancellationTokenSource`.
```c#
var cache = serviceProvider.GetService<IOrderBookCache>();

using (var controller = new TaskController())
{
    controller.Begin(tkn => cache.SubscribeAsync(Symbol.BTC_USDT,
        evt => { /* optional inline event handler. */ }, tkn),
        err => { /* optional inline exception handler. */ });

    // ...

    // NOTE: The encapsulated Task is canceled and awaited when the controller is disposed.
}
```

##### Aggregate Trades Cache
Use an [`IAggregateTradesCache`](src/Binance/Cache/IAggregateTradesCache.cs) (with an [`ITradesWebSocketClient`](src/Binance/Api/WebSocket/ITradesWebSocketClient.cs)) to create a real-time, synchronized trade history for a symbol. Refer to the BinanceTradeHistory sample for an [additional example](samples/BinanceTradeHistory/Program.cs).
```c#
var cache = serviceProvider.GetService<IAggregateTradeCache>();

cache.Update += OnUpdateEvent; // optionally, subscribe to update events.

var task = Task.Run(() => cache.SubscribeAsync(Symbol.BTC_USDT, evt =>
{
    // optionally, use an inline event handler.
}, cts.Token)); // starts synchronization.

// ...

var trades = cache.Trades; // access latest aggregate trades (thread-safe).

// ...

cts.Cancel(); // end the task.
await task; // wait for task to complete.
```
```c#
void OnUpdateEvent(object sender, AggregateTradesCacheEventArgs evt)
{
    // Event has an immutable copy of aggregate trades.
    var trades = evt.Trades.
}
```
Optionally, you can use the `IDisposable` [`TaskController`](src/Binance/Utility/TaskController.cs) to encapsulate the `Task` and `CancellationTokenSource`.
```c#
var cache = serviceProvider.GetService<IAggregateTradeCache>();

using (var controller = new TaskController())
{
    controller.Begin(tkn => cache.SubscribeAsync(Symbol.BTC_USDT,
        evt => { /* optional inline event handler. */ }, tkn),
        err => { /* optional inline exception handler. */ });

    // ...

    // NOTE: The encapsulated Task is canceled and awaited when the controller is disposed.
}
```

##### Candlesticks Cache
Use an [`ICandlesticksCache`](src/Binance/Cache/ICandlesticksCache.cs) (with an [`ICandlestickWebSocketClient`](src/Binance/Api/WebSocket/ICandlestickWebSocketClient.cs)) to create a real-time, synchronized price chart for a symbol. Refer to the BinancePriceChart sample for an [additional example](samples/BinancePriceChart/Program.cs).

```c#
var cache = serviceProvider.GetService<ICandlesticksCache>();

cache.Update += OnUpdateEvent; // optionally, subscribe to update events.

var task = Task.Run(() => cache.SubscribeAsync(Symbol.BTC_USDT, CandlestickInterval.Hour, evt =>
{
    // optionally, use an inline event handler.
}, cts.Token)); // starts synchronization.

// ...

var candlesticks = cache.Candlestics; // access latest candlesticks (thread-safe).

// ...

cts.Cancel(); // end the task.
await task; // wait for task to complete.
```
```c#
void OnUpdateEvent(object sender, CandlesticksCacheEventArgs evt)
{
    // Event has an immutable copy of candlesticks.
    var candlesticks = evt.Candlesticks.
}
```
Optionally, you can use the `IDisposable` [`TaskController`](src/Binance/Utility/TaskController.cs) to encapsulate the `Task` and `CancellationTokenSource`.
```c#
var cache = serviceProvider.GetService<ICandlesticksCache>();

using (var controller = new TaskController())
{
    controller.Begin(tkn => cache.SubscribeAsync(Symbol.BTC_USDT, CandlestickInterval.Hour,
        evt => { /* optional inline event handler. */ }, tkn),
        err => { /* optional inline exception handler. */ });

    // ...

    // NOTE: The encapsulated Task is canceled and awaited when the controller is disposed.
}
```

##### Account Info Cache
Use an [`IAccountInfoCache`](src/Binance/Cache/IAccountInfoCache.cs) (with an [`IUserDataWebSocketClient`](src/Binance/Api/WebSocket/IUserDataWebSocketClient.cs)) to create a real-time, synchronized account profile for a user. Refer to the following for an [additional example](samples/BinanceConsoleApp/Examples/AccountBalancesExample.cs).

```c#
var cache = serviceProvider.GetService<IAccountInfoCache>();

cache.Update += OnUpdateEvent; // optionally, subscribe to update events.

var task = Task.Run(() => cache.SubscribeAsync(user, evt =>
{
    // optionally, use an inline event handler.
}, cts.Token)); // starts synchronization.

// ...

var accountInfo = cache.AccountInfo; // access latest candlesticks (thread-safe).

// ...

cts.Cancel(); // end the task.
await task; // wait for task to complete.
```
```c#
void OnUpdateEvent(object sender, AccountInfoCacheEventArgs evt)
{
    // Event has an immutable copy of account info.
    var accountInfo = evt.AccountInfo.
}
```
Optionally, you can use the `IDisposable` [`TaskController`](src/Binance/Utility/TaskController.cs) to encapsulate the `Task` and `CancellationTokenSource`.
```c#
var cache = serviceProvider.GetService<IAccountInfoCache>();

using (var controller = new TaskController())
{
    controller.Begin(tkn => cache.SubscribeAsync(user,
        evt => { /* optional inline event handler. */ }, tkn),
        err => { /* optional inline exception handler. */ });

    // ...

    // NOTE: The encapsulated Task is canceled and awaited when the controller is disposed.
}
```
