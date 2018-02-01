# Binance ![](https://github.com/sonvister/Binance/blob/master/images/logo.png?raw=true)
A full-featured .NET **[Binance API](https://github.com/binance-exchange/binance-official-api-docs)** designed for ease of use.

[![](https://img.shields.io/github/last-commit/sonvister/Binance.svg)](https://github.com/sonvister/Binance)

## Features
* Compatible with **.NET Standard 2.0** and **.NET Framework 4.7.1**.
* **Complete** coverage of the [Binance API](https://github.com/binance-exchange/binance-official-api-docs) including all REST and WebSocket endpoints (*with combined streams*).
  * A Binance account API Key is *not required* to access *public* REST and WebSocket endpoints (*most market data*).
* **Simple** API abstraction using domain/value objects that do not expose underlying (*HTTP/REST*) behavior.
* **Convenient** assets and symbols (e.g. `Symbol.BTC_USDT`) with exchange info (*price/quantity: min, max, etc.*).
  * With methods for validating (*w/ or w/o exceptions*) client order price, quantity, and type for a symbol.
* Consistent use of **domain models** whether you're querying the API or using real-time WebSocket client events.
* Customizable **dual-layer API** with access to JSON responses (*low-level*) or deserialized domain/value objects.
  * Same serializers used in `BinanceApi` are available to provide application-level deserialization of JSON data.
* API exceptions provide the Binance server response **ERROR code and message** for easier troubleshooting.
* Unique implementation supports **multiple users** and requires user authentication only where necessary.
* Web API interface includes automatic **rate limiting** and system-to-server **time synchronization** for reliability.
  * Advanced rate limiting includes distinct (request and order) rate limiters with **endpoint weights** incorporated. 
* Easy-to-use **WebSocket endpoint clients** and various ready-to-use **caching** implementations (*with events*).
* Low-level `BinanceHttpClient` API utilizes a single, cached HttpClient for performance (*and implemented as singleton*).
* **Limited dependencies** and use of Microsoft extensions for **dependency injection**, **logging**, and **options**.
* Multiple .NET **sample applications** including live displays of market depth, trades, and candlesticks for a symbol.
  * Alternative `IWebSocketClients` for using **WebSocketSharp** or **WebSocket4Net** (*for Windows 7 compatibility*).
  * How to **efficiently** use combined streams with a single, application-wide, web socket (`BinanceWebSocketStream`).

## Getting Started
### Binance Sign-up
To use the private (*authenticated*) API methods you must have an account with Binance and create an API Key. Please use my Referral ID: **10899093** when you [**Register**](https://www.binance.com/register.html?ref=10899093) (*an account is not required to access the public market data*).

### Installation
Using [Nuget](https://www.nuget.org/packages/Binance/) Package Manager:
```
PM> Install-Package Binance
```
[![](https://img.shields.io/nuget/v/Binance.svg)](https://www.nuget.org/packages/Binance)\
[![](https://img.shields.io/nuget/dt/Binance.svg)](https://www.nuget.org/packages/Binance)

## Development
The master branch is *currently* used for development and may differ from the latest release.\
To get the source code for a particular release, first select the corresponding **Tag**.

### Build Environment
[Microsoft Visual Studio Community 2017](https://www.visualstudio.com/vs/community/)

## Documentation
[**Wiki**](https://github.com/sonvister/Binance/wiki)\
*NOTE: Some information is currently out-of-date ...will be updated soon.*

Binance API information: [Binance Official Documentation](https://github.com/binance-exchange/binance-official-api-docs)\
Binance API questions: [Binance API Telegram](https://t.me/binance_api_english)

## Donate
DCR: Dsog2jYLS65Y3N2jDQSxsiBYC3SRqq7TGd4\
LTC: MNhGkftcFDE7TsFFvtE6W9VVKhxH74T3eM\
XEM: NC3HR4-V46BTS-LIKEE4-2GZQBB-FHBUXO-CG7EBO-VIMP\
DASH: XmFvpRgwfDRdN9wbrZjHZeH63Rt9CwHqUf\
ZEC: t1Ygz58dkcx2WXuGCjGiPo8w7Q1dcCSscGJ\
ETH: 0x3BFd7B3EAA6aE6BCF861B9B1803d67abe9360bca\
BTC: 3JjG3tRR1dx98UJyNdpzpkrxRjXmPfQHk9

Thank you.
