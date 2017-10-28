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
* Web API interface includes automatic **rate limiting** and system-to-server **time synchronization** for stability.
* Easy-to-use **WebSocket endpoint clients** and various ready-to-use **caching** implementations (*w/ events*).
* Low-level API methods share an HttpClient for performance (*implemented as singleton w/in DI framework*).
* **Limited dependencies** and use of Microsoft extensions for: **dependency injection**, **logging**, and **options**
* .NET Core **sample applications** including live displays of market depth, trades, and candlesticks for a symbol.

## Getting Started
### General Information
- All timestamp related fields are in milliseconds (Unix time).
- All `IEnumerable<>` data is returned in **ascending** chronological order (oldest first, newest last).


### Example/Sample Applications
#### *Minimal* Examples
**NOTE**: C# 7.1 is required for async Main (*set language version in project advanced build properties*).

- [Minimal](samples/BinanceConsoleApp/Examples/MinimalWithDependencyInjection.cs) with dependency injection (*recommended*).
- [Minimal](samples/BinanceConsoleApp/Examples/MinimalWithoutDependencyInjection.cs) without dependency injection.

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
#### Sample Application Configuration
If using the `BinanceConsoleApp` sample you may see this message when accessing non-public API methods:

> To access some Binance endpoint features, your **API Key and Secret** may be required.
> You can either modify the '**ApiKey**' and '**ApiSecret**' configuration values in **appsettings.json**.
> Or use the following commands to configure the .NET user secrets for the project:
> \
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;`dotnet user-secrets set BinanceApiKey <your api key>`
> &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;`dotnet user-secrets set BinanceApiSecret <your api secret>`
> \
> For more information: <https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets>

## API Method Reference
### Connectivity
#### Ping
```c#
    var successful = await api.PingAsync();
```

#### Server Time
```c#
    var time = await api.GetTimeAsync();
```

### Market Data
#### Order Book
Get the order book (depth of market) for a symbol with optional limit [5, 10, 20, 50, 100, 200, 500].
```c#
    var book = await api.GetOrderBookAsync(Symbol.BTC_USDT);
```

#### Trades
Get compressed/aggregate trades for a symbol with optional limit [1-500].
```c#
    var trades = await api.GetTradesAsync(Symbol.BTC_USDT);
```

#### Candlesticks
Get candlesticks for a symbol with optional limit [1-500].
```c#
    var candles = await api.GetCandlesticksAsync(Symbol.BTC_USDT, KlineInterval.Hour);
```

#### 24-hour Statistics
Get the 24-hour statistics for a symbol.
```c#
    var stats = await api.Get24hrStatsAsync(Symbol.BTC_USDT);
```

#### Prices
Get current prices for all symbols.
```c#
    var prices = await api.GetPricesAsync();
```

#### Order Book Ticker
Get best (top) price and quantity on the order book for all symbols.
```c#
    var tops = await api.GetOrderBookTopsAsync();
```

#### Order Book Cache
Utilize an `IDepthWebSocketClient` (internal to `IOrderBookCache`) to create a real-time, synchronized order book for a symbol.
```c#
    using (var cache = serviceProvider.GetService<IOrderBookCache>())
    {
        cache.Update += OnOrderBookUpdate; // optionally, subscribe to update events.
        
        var task = Task.Run(() => book.SubscribeAsync(Symbol.BTC_USDT, (e) =>
        {
            // optionally, use an inline event handler.
        }, cts.Token)); // starts synchronization.
        
        // ...
        
        var price = cache.OrderBook.Top.Bid.Price; // access latest order book (thread-safe).
        var book = cache.OrderBook; // keep a static (snapshot) reference of order book.
        
        // ...
        
        cts.Cancel(); // end the order book task.
        await task; // wait for task to complete.
    }
```
```
void OnOrderBookUpdate(object sender, OrderBookCacheEventArgs e)
{
    // Event has an immutable copy of the order book.
    var price = e.OrderBook.Top.Bid.Price;
}
```

### Account
#### Authentication
Create a user authentication instance `IBinanceApiUser` with your Binance account API **Key** and **Secret** (optional).
```c#
    var user = new BinanceApiUser("<Your API Key>", <your API Secret>);
```
NOTE: User authentication is method injected -- only where required -- so a single Binance API instance (with a single `HttpClient`) can support multiple Binance users.

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

#### Test Order
Create and place a new *TEST* order. \
An exception will be thrown if the order placement test fails.
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

#### Query an Order
Get an order to determine current status. \
Order lookup requires an order instance or the combination of a symbol and the order ID or original client order ID. If an order instance is provided, it will be updated in place in addition to being returned.
```c#
    var order = await api.GetAsync(order); // use to update status in place.
    // or...
    var order = await api.GetOrderAsync(user, Symbol.BTC_USDT, orderId);
    // or...
    var order = await api.GetOrderAsync(user, Symbol.BTC_USDT, clientOrderId);
```

#### Cancel an Order
Cancel an order. \
Order lookup requires an order instance or the combination of a symbol and the order ID or original client order ID.
```c#
    await api.CancelAsync(order);
    // or...
    await api.CancelOrderAsync(user, Symbol.BTC_USDT, orderId);
    // or...
    await api.CancelOrderAsync(user, Symbol.BTC_USDT, clientOrderId);
```

#### Open Orders
Get all open orders for a symbol with optional limit [1-500].
```c#
    var orders = await api.GetOpenOrdersAsync(user, Symbol.BTC_USDT);
```

#### Orders
Get all orders; active, canceled, or filled with optional limit [1-500].
```c#
    var orders = await api.GetOrdersAsync(user, Symbol.BTC_USDT);
```

#### Account Information
Get current account information.
```c#
    var account = await api.GetAccountAsync(user);
```

#### Account Trades
Get trades for a specific account and symbol with optional limit [1-500].
```c#
    var account = await api.GetTradesAsync(user, Symbol.BTC_USDT);
```

#### Withdraw
Submit a withdraw request... *optionally donate to me* :)
```c#
    await api.WithdrawAsync(user, Asset.BTC, "3JjG3tRR1dx98UJyNdpzpkrxRjXmPfQHk9", 0.01m);
```

#### Deposit History
Get deposit history.
```c#
    var deposits = await api.GetDepositsAsync(user);
```

#### Withdraw History
Get withdraw history.
```c#
    var withdrawals = await api.GetWithdrawalsAsync(user);
```

### User Stream
#### Start User Stream
Start a new user data stream.
```c#
    var listenKey = await api.UserStreamStartAsync(user);
```

#### Keepalive User Stream
Ping a user data stream to prevent a timeout.
```c#
    await api.UserStreamKeepAliveAsync(user, listenKey);
```

#### Close User Stream
Close a user data stream.
```c#
    await api.UserStreamCloseAsync(user, listenKey);
```

### WebSocket
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
