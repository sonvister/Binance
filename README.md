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
* **Complete** implementation of [Binance API](https://www.binance.com/restapipub.html) including latest deposit/withdrawal features and WebSocket feeds. 
* **Simple** API abstraction using domain/value objects that do not expose underlying (*HTTP/REST*) behavior.
* Unique **dual-layer API design** returning either raw JSON (*low-level*) or deserialized domain/value objects.
* API exceptions include the Binance response **ERROR code and message** for easier troubleshooting.
* Implementation supports **multiple users** (*authentication details passed via method injection*).
* Web API interface includes automatic **rate limiting** and system-to-server **time synchronization**.
* Easy to use **websocket endpoint clients** and a ready-to-use **order book cache** (*w/ event subscribing*).
* Multiple .NET Core **sample applications**, including a 'minimal' live display of market depth for a symbol.
* **Limited dependencies** and use of Microsoft extensions: **dependency injection**, **logging**, and **options**
* The APIs are implemented as singletons (w/in DI framework) with a **cached HttpClient** for performance.

## Getting Started
### General Information
- All `IEnumerable<>` data is returned in **ascending** order. Oldest first, newest last.
- All timestamp related fields are in milliseconds (Unix time).

### Example Applications
#### Minimal
The first example (*recommended*) uses dependency injection, while the second *minimal* example does not.
**NOTE**: C# 7.1 is required for async Main (*set language version in project advanced build properties*).

```c#
using Binance;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args) // C# 7.1
        {
            var services = new ServiceCollection()
                .AddBinance()
                .BuildServiceProvider();

            using (var api = services.GetService<IBinanceApi>())
            {
                if (await api.PingAsync())
                    Console.WriteLine("SUCCESSFUL!");

                Console.ReadKey(true);
            }
        }
    }
}
```
```c#
using Binance.Api;
using System;
using System.Threading.Tasks;

namespace ConsoleApp
{
    class Program
    {
        static async Task Main(string[] args) // C# 7.1
        {
            using (var api = new BinanceApi())
            {
                if (await api.PingAsync())
                    Console.WriteLine("SUCCESSFUL!");

                Console.ReadKey(true);
            }
        }
    }
}
```

#### Exception Handling
This example demonstrates how to handle exceptions from API methods.

NOTE: Handling exceptions with this level of precision is only applicable to actions that an application may retry should the first attempt fail (*e.g. new order placement*).

```c#
try
{
    using (var api = new BinanceApi())
    using (var user = new BinanceUser("api-key", "api-secret"))
    {
        var order = await api.PlaceAsync(user, new MarketOrder()
        {
            Symbol = Symbol.BTC_USDT,
            Side = OrderSide.Sell,
            Quantity = 1
        });
    }
}
catch (BinanceUnknownStatusException)
{
    // Respond to HTTP 504 response errors:
    //   HTTP 504 return code is used when the API successfully sent the message but not get a response within the timeout period.
    //   It is important to NOT treat this as a failure; the execution status is UNKNOWN and could have been a success.
}
catch (BinanceHttpException)
{
    // Respond to HTTP response errors/codes:
    //   HTTP 4XX return codes are used for malformed requests; client side issue.
    //   HTTP 5XX return codes are used for internal errors; server side issue.
}
catch (BinanceApiException)
{
    // Respond to other Binance API exceptions (typically JSON deserialization failures).
}
catch (Exception)
{
    // ...
}
```
#### Sample Application Configuration
If using the `BinanceConsoleApp` sample you may see this message when accessing non-public API methods:

To access some Binance endpoint features, your **API Key and Secret** may be required.
You can either modify the '**ApiKey**' and '**ApiSecret**' configuration values in **appsettings.json**.
Or use the following commands to configure the .NET user secrets for the project:
```
dotnet user-secrets set BinanceApiKey <your api key>
dotnet user-secrets set BinanceApiSecret <your api secret>
```
For more information: <https://docs.microsoft.com/en-us/aspnet/core/security/app-secrets>


## API Methods
## Connectivity
### Ping
```c#
    var successful = await api.PingAsync();
```

### Server Time
```c#
    var time = await api.GetTimeAsync();
```

## Market Data
### Order Book
Get order book (depth of market) for a symbol with optional limit [1-100].
```c#
    var book = await api.GetOrderBookAsync(Symbol.BTC_USDT);
```

### Trades
Get compressed/aggregate trades for a symbol with optional limit [1-500].
```c#
    var trades = await api.GetTradesAsync(Symbol.BTC_USDT);
```

### Candlesticks
Get candlesticks for a symbol with optional limit [1-500].
```c#
    var candles = await api.GetCandlesticksAsync(Symbol.BTC_USDT, KlineInterval.Hour);
```

### 24-hour Statistics
Get 24-hour statistics for a symbol.
```c#
    var stats = await api.Get24hrStatsAsync(Symbol.BTC_USDT);
```

### Prices
Get current prices for all symbols.
```c#
    var prices = await api.GetPricesAsync();
```

### Order Book Ticker
Get best (top) price and quantity on the order book for all symbols.
```c#
    var tops = await api.GetOrderBookTopsAsync();
```

### Order Book Cache
Utilize the depth of market WebSocket client to create a real-time, synchronized order book for a symbol.
```c#
    using (var book = serviceProvider.GetService<IOrderBookCache>()) // ...is IOrderBook too
    {
        book.Update += OnOrderBookUpdate; // subscribe to order book update events.
        
        var task = Task.Run(() => book.SubscribeAsync(Symbol.BTC_USDT, cts.Token)); // start synchronization.
        
        // ...
        
        var bid = book.Top.Bid.Price; // access the live order book directly (thread-safe).
        var copy = book.Clone(); // take a snapshot of the order book (for a consistent/complete state).
        
        // ...
        
        cts.Cancel(); // end the order book task.
        await task; // wait for task to complete.
    }
```
```
void OnOrderBookUpdate(object sender, OrderBookUpdateEventArgs e)
{
    e.OrderBook.Top.Bid.Price; // safely use a snapshot of the updated order book.
}
```

## Account
### Authentication
Create a user authentication object using your Binance account API Key and Secret.
```c#
    var user = new BinanceUser("<Your API Key>", <your API Secret>);
```
NOTE: User authentication is method injected so that a single Binance API instance can support multiple users.

### Limit Order
Create and place a new *Limit* order.
NOTE: Client (*or pending*) orders are created as a mutable order placeholder, only after the client order is placed does an Order (*with status*) exist.
```c#
    using (var user = new BinanceUser("api-key", "api-secret"))
    {
        var order = await api.PlaceAsync(user, new LimitOrder()
        {
            Symbol = Symbol.BTC_USDT,
            Side = OrderSide.Buy,
            Quantity = 1,
            Price = 5000
        });
    }
```

### Market Order
Create and place a new *Market* order. You do not set the price for Market orders.
NOTE: Client (*or pending*) orders are created as a mutable order placeholder, only after the client order is placed does an Order (*with status*) exist.
```c#
    using (var user = new BinanceUser("api-key", "api-secret"))
    {
        var order = await api.PlaceAsync(user, new MarketOrder()
        {
            Symbol = Symbol.BTC_USDT,
            Side = OrderSide.Sell,
            Quantity = 1
        });
    }
```

### Test Order
Create and place a new *Test* order.
NOTE: To specify an order is for test-only, the **IsTestOnly** flag is set true (*default: false*).
```c#
    using (var user = new BinanceUser("api-key", "api-secret"))
    {
        var order = await api.PlaceAsync(user, new MarketOrder()
        {
            Symbol = Symbol.BTC_USDT,
            Side = OrderSide.Sell,
            Quantity = 1,
            IsTestOnly = true
        });
    }
```

### Query Order
Get an order to determine current status.
Order lookup requires an order instance or the combination of a symbol and the order ID or client order ID.
If an order instance is provided, it will be updated in place in addition to being returned.
```c#
    var order = await api.GetOrderAsync(user, order); // use to update status in place.
    // ...or...
    var order = await api.GetOrderAsync(user, Symbol.BTC_USDT, orderId);
    // ...or...
    var order = await api.GetOrderAsync(user, Symbol.BTC_USDT, clientOrderId);
```

### Cancel Order
Cancel an order
Order lookup requires an order instance or the combination of a symbol and the order ID or client order ID.
```c#
    await api.CancelAsync(user, order);
    // ...or...
    await api.CancelOrderAsync(user, Symbol.BTC_USDT, orderId);
    // ...or...
    await api.CancelOrderAsync(user, Symbol.BTC_USDT, clientOrderId);
```

### Open Orders
Get all open orders for a symbol with optional limit [1-500].
```c#
    var orders = await api.GetOpenOrdersAsync(user, Symbol.BTC_USDT);
```

### Orders
Get all orders; active, canceled, or filled with optional limit [1-500].
```c#
    var orders = await api.GetOrdersAsync(user, Symbol.BTC_USDT);
```

### Account Information
Get current account information.
```c#
    var account = await api.GetAccountAsync(user);
```

### Account Trades
Get trades for a specific account and symbol with optional limit [1-500].
```c#
    var account = await api.GetTradesAsync(user, Symbol.BTC_USDT);
```

### Deposit History
Get deposit history.
```c#
    var deposits = await api.GetDepositsAsync(user);
```

### Withdraw History
Get withdraw history.
```c#
    var withdrawals = await api.GetWithdrawalsAsync(user);
```

## User Stream
### Start User Stream
Start a new user data stream.
```c#
    var listenKey = await api.UserStreamStartAsync(user);
```

### Keepalive User Stream
Ping a user data stream to prevent a timeout.
```c#
    await api.UserStreamKeepAliveAsync(user, listenKey);
```

### Close User Stream
Close a user data stream.
```c#
    await api.UserStreamCloseAsync(user, listenKey);
```

## WebSocket
### Depth Endpoint
Get real-time depth update events.
```c#
    using (var client = serviceProvider.GetService<IDepthWebSocketClient>())
    {
        client.DepthUpdate += OnDepthUpdateEvent;
        
        var task = Task.Run(() => client.SubscribeAsync(Symbol.BTC_USDT, cts.Token));
        
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

### Kline Endpoint
Get real-time kline/candlestick events.
```c#
    using (var client = serviceProvider.GetService<IKlineWebSocketClient>())
    {
        client.Kline += OnKlineEvent;
        
        var task = Task.Run(() => client.SubscribeAsync(Symbol.BTC_USDT, KlineInterval.Hour, cts.Token));
        
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

### Trades Endpoint
Get real-time aggregate trade events.
```c#
    using (var client = serviceProvider.GetService<ITradesWebSocketClient>())
    {
        client.AggregateTrade += OnAggregateTradeEvent;
        
        var task = Task.Run(() => client.SubscribeAsync(Symbol.BTC_USDT, cts.Token));
        
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

### User Data Endpoint
Get real-time account update events.
```c#
    using (var client = serviceProvider.GetService<IUserDataWebSocketClient>())
    {
        client.AccountUpdate += OnAccountUpdateEvent;
        client.OrderUpdate += OnOrderUpdateEvent;
        client.TradeUpdate += OnTradeUpdateEvent;
        
        var task = Task.Run(() => client.SubscribeAsync(user, cts.Token));
        
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
