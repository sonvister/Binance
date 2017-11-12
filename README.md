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
To use the private (*authenticated*) API methods you must have an account with Binance and create an API Key. Please use my Referral ID: **10899093** when you [**Register**](https://www.binance.com/register.html?ref=10899093). \
*NOTE*: An account is not required to access the public market data.

### Installation
Using [Nuget](https://www.nuget.org/packages/Binance/) Package Manager:
```
PM> Install-Package Binance
```
[![](https://img.shields.io/nuget/v/Binance.svg)](https://www.nuget.org/packages/Binance)\
[![](https://img.shields.io/nuget/dt/Binance.svg)](https://www.nuget.org/packages/Binance)


## Documentation
See [**Wiki**](https://github.com/sonvister/Binance/wiki)
