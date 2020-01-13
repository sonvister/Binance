# Binance ![](https://github.com/sonvister/Binance/blob/master/images/logo.png?raw=true)
A .NET API library (a.k.a. wrapper) for the **[official Binance web API](https://github.com/binance-exchange/binance-official-api-docs)**.

Compatible with **.NET Standard 2.0** and **.NET Framework 4.7.1**

Built using [TAP (Task-based Asynchronous Pattern)](https://docs.microsoft.com/en-us/dotnet/standard/asynchronous-programming-patterns/task-based-asynchronous-pattern-tap).

[![](https://img.shields.io/github/last-commit/sonvister/Binance.svg)](https://github.com/sonvister/Binance)

## Features
* **Beta** release with majority coverage of the official [Binance API](https://github.com/binance-exchange/binance-official-api-docs) including REST API and Web Sockets.
  * *NOTE: Not actively adding new features or providing support...*
  * Binance account API-Key is *not required* to access the *public* REST and Web Socket endpoints (*most market data*).
* Easy-to-use **Web Socket managers** (*with combined streams*) and **in-memory cache** implementations (*with events*).
* **Convenient** assets and symbols (e.g. `Symbol.BTC_USDT`) with exchange info (*price/quantity: min, max, etc.*).
  * With methods for validating (*w/ or w/o exceptions*) client order price, quantity, and type for a symbol.
* REST API includes automatic **rate limiting** and system-to-server **time synchronization** for reliability.
  * Advanced rate limiting includes distinct (request and order) rate limiters with **endpoint weights** incorporated. 
* Unique REST API implementation supports **multiple users** and requires user authentication only where necessary.
* REST API exceptions provide the Binance server response **ERROR code and message** for easier troubleshooting.
* REST API (*low-level*) utilizes a single, cached `HttpClient` for performance (*implemented as singleton*).
* **Simple** API abstraction using domain/value objects that do not expose underlying (*HTTP/REST*) behavior.
  * Consistent use of **domain models** between REST API queries and real-time Web Socket client events.
* Customizable **multi-layer API** with access to (*low-level*) JSON responses or deserialized domain/value objects.
  * Same serializers used in `BinanceApi` are available for application-level deserialization of JSON data.
* **Limited dependencies** with use of Microsoft extensions for **dependency injection**, **logging**, and **options**.
* Multiple .NET **sample applications** including live displays of market depth, trades, and candlesticks for a symbol.
  * Alternative `IWebSocketClients` for using **WebSocketSharp** or **WebSocket4Net** (*for Windows 7 compatibility*).
  * How to **efficiently** use combined streams with a single, application-wide, web socket (`BinanceWebSocketStream`).

## Getting Started
### Binance Sign-up
To use the private (*authenticated*) API methods you must have an account with Binance and create an API-Key. Please use my Referral ID: **10899093** when you [**Register**](https://www.binance.com/register.html?ref=10899093) (*it's an easy way to give back at no cost to you*).

*NOTE*: An account is *not* required to access the public market data.

### Installation
Using [Nuget](https://www.nuget.org/packages/Binance/) Package Manager:
```
PM> Install-Package Binance
```
[![](https://img.shields.io/nuget/v/Binance.svg)](https://www.nuget.org/packages/Binance)\
[![](https://img.shields.io/nuget/dt/Binance.svg)](https://www.nuget.org/packages/Binance)

### Example Usage
#### REST API
Test connectivity.

```C#
using Binance;

// Initialize REST API client.
var api = new BinanceApi();

// Check connectivity.
if (await api.PingAsync())
{
    Console.WriteLine("Successful!");
}
```

Place a TEST market order.

```C#
using Binance;

var api = new BinanceApi();

// Create user with API-Key and API-Secret.
using (var user = new BinanceApiUser("<API-Key>", "<API-Secret>"))
{
    // Create a client (MARKET) order.
    var clientOrder = new MarketOrder(user)
    {
        Symbol = Symbol.BTC_USDT,
        Side = OrderSide.Buy,
        Quantity = 0.01m
    };

    try
    {
        // Send the TEST order.
        await api.TestPlaceAsync(clientOrder);
        
        Console.WriteLine("TEST Order Successful!");
    }
    catch (Exception e)
    {
        Console.WriteLine($"TEST Order Failed: \"{e.Message}\"");
    }
}
```

#### Web Socket
Get real-time aggregate trades (*with automatic web socket re-connect*).

```C#
using Binance;
using Binance.WebSocket;

// Initialize web socket client (with automatic streaming).
var webSocketClient = new AggregateTradeWebSocketClient();

// Handle error events.
webSocketClient.Error += (s, e) => { Console.WriteLine(e.Exception.Message); };

// Subscribe callback to BTC/USDT (automatically begin streaming).
webSocketClient.Subscribe(Symbol.BTC_USDT, evt =>
{
    var side = evt.Trade.IsBuyerMaker ? "SELL" : "BUY ";
	
    // Handle aggregate trade events.
    Console.WriteLine($"{evt.Trade.Symbol} {side} {evt.Trade.Quantity} @ {evt.Trade.Price}");
});

// ...

// Unsubscribe (automatically end streaming).
webSocketClient.Unsubscribe();
```

Maintain real-time order book (market depth) cache.

```C#
using Binance;
using Binance.Cache;
using Binance.WebSocket;

// Initialize web socket cache (with automatic streaming).
var webSocketCache = new DepthWebSocketCache();

// Handle error events.
webSocketCache.Error += (s, e) => { Console.WriteLine(e.Exception.Message); };

// Subscribe callback to BTC/USDT (automatically begin streaming).
webSocketCache.Subscribe(Symbol.BTC_USDT, evt =>
{
    // Get symbol from cache (update cache if a symbol is missing).
    var symbol = Symbol.Cache.Get(evt.OrderBook.Symbol);

    var minBidPrice = evt.OrderBook.Bids.Last().Price;
    var maxAskPrice = evt.OrderBook.Asks.Last().Price;

    // Handle order book update events.
    Console.WriteLine($"Bid Quantity: {evt.OrderBook.Depth(minBidPrice)} {symbol.BaseAsset} - " +
                      $"Ask Quantity: {evt.OrderBook.Depth(maxAskPrice)} {symbol.BaseAsset}");
});

// ...

// Unsubscribe (automatically end streaming).
webSocketCache.Unsubscribe();
```

## Documentation
See: [**Wiki**](https://github.com/sonvister/Binance/wiki)

***NOTE**: The [samples](https://github.com/sonvister/Binance/blob/master/samples) demonstrate up-to-date usage of this library.*

### Binance Exchange API (*for reference*)
REST/WebSocket details: [Binance Official Documentation](https://github.com/binance-exchange/binance-official-api-docs)\
REST/WebSocket questions: [Binance Official API Telegram](https://t.me/binance_api_english) (*not for questions about this library*)

## Development
The master branch is *currently* used for development and may differ from the latest release.\
To get the source code for a particular release, first select the corresponding **Tag**.

### Build Environment
[Microsoft Visual Studio Community 2017](https://www.visualstudio.com/vs/community/)

![Build status](https://travis-ci.org/sonvister/Binance.svg?branch=master)

## Donate
**Decred (DCR)**: Dsd7amDBfC7n6G93NJzbaJjUdiSeN5D3tmR

Thank you.

Follow [@sonvister](https://twitter.com/sonvister)

[![Donate NIM](https://www.nimiq.com/accept-donations/img/donationBtnImg/orange-small.svg)](https://safe.nimiq.com/#_request/NQ13481VT3VVKRENJJL39B5J7DJMHF4CRCVL_)
