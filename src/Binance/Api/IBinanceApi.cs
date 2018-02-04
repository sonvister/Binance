using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Binance.Account;
using Binance.Account.Orders;
using Binance.Api.RateLimit;
using Binance.Market;

namespace Binance.Api
{
    public interface IBinanceApi
    {
        #region Properties

        /// <summary>
        /// The (low-level) HTTP client.
        /// </summary>
        IBinanceHttpClient HttpClient { get; }

        #endregion Properties

        #region Connectivity

        /// <summary>
        /// Test connectivity to the server.
        /// </summary>
        /// <returns></returns>
        Task<bool> PingAsync(CancellationToken token = default);

        /// <summary>
        /// Test connectivity to the server and get the current time (Unix time milliseconds).
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<long> GetTimestampAsync(CancellationToken token = default);

        /// <summary>
        /// Get rate limit information.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<IEnumerable<RateLimitInfo>> GetRateLimitInfoAsync(CancellationToken token = default);

        #endregion Connectivity

        #region Market Data

        /// <summary>
        /// Get order book (market depth) of a symbol.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="limit">Default 100; max 100.</param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<OrderBook> GetOrderBookAsync(string symbol, int limit = default, CancellationToken token = default);

        /// <summary>
        /// Get latest (non-compressed) trades.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="limit">Default 500; max 500.</param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<IEnumerable<Trade>> GetTradesAsync(string symbol, int limit = default, CancellationToken token = default);

        /// <summary>
        /// Get older (non-compressed) trades.
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="symbol"></param>
        /// <param name="fromId">ID to get trades from INCLUSIVE.</param>
        /// <param name="limit">Default 500; max 500.</param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<IEnumerable<Trade>> GetTradesFromAsync(string apiKey, string symbol, long fromId, int limit = default, CancellationToken token = default);

        /// <summary>
        /// Get latest compressed, aggregate trades. Trades that fill at the time, from the same order, with the same price will have the quantity aggregated.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="limit">Default 500; max 500.</param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<IEnumerable<AggregateTrade>> GetAggregateTradesAsync(string symbol, int limit = default, CancellationToken token = default);

        /// <summary>
        /// Get compressed, aggregate trades. Trades that fill at the time, from the same order, with the same price will have the quantity aggregated.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="fromId">ID to get aggregate trades from INCLUSIVE.</param>
        /// <param name="limit">Default 500; max 500.</param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<IEnumerable<AggregateTrade>> GetAggregateTradesFromAsync(string symbol, long fromId, int limit = default, CancellationToken token = default);

        /// <summary>
        /// Get compressed, aggregate trades. Trades that fill at the time, from the same order, with the same price will have the quantity aggregated.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="startTime">Timestamp in ms to get aggregate trades from INCLUSIVE.</param>
        /// <param name="endTime">Timestamp in ms to get aggregate trades until INCLUSIVE.</param>
        /// <param name="token"></param>
        /// <returns></returns>
        [Obsolete("GetAggregateTradesInAsync is obsolete, please use GetAggregateTradesAsync(string, DateTime, DateTime) instead.")]
        Task<IEnumerable<AggregateTrade>> GetAggregateTradesInAsync(string symbol, long startTime, long endTime, CancellationToken token = default);

        /// <summary>
        /// Get compressed, aggregate trades. Trades that fill at the time, from the same order, with the same price will have the quantity aggregated.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="startTime">Time to get aggregate trades from INCLUSIVE.</param>
        /// <param name="endTime">Time to get aggregate trades until INCLUSIVE.</param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<IEnumerable<AggregateTrade>> GetAggregateTradesAsync(string symbol, DateTime startTime, DateTime endTime, CancellationToken token = default);

        /// <summary>
        /// Get candlesticks for a symbol. Candlesticks/K-Lines are uniquely identified by their open time.
        /// If startTime and endTime are not sent, the most recent candlesticks are returned.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="interval"></param>
        /// <param name="limit">Default 500; max 500.</param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<IEnumerable<Candlestick>> GetCandlesticksAsync(string symbol, CandlestickInterval interval, int limit = default, long startTime = default, long endTime = default, CancellationToken token = default);

        /// <summary>
        /// Get 24-hour price change statistics for a symbol.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<SymbolStatistics> Get24HourStatisticsAsync(string symbol, CancellationToken token = default);

        /// <summary>
        /// Get 24-hour price change statistics for all symbols.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<IEnumerable<SymbolStatistics>> Get24HourStatisticsAsync(CancellationToken token = default);

        /// <summary>
        /// Get latest price for a symbol.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<SymbolPrice> GetPriceAsync(string symbol, CancellationToken token = default);

        /// <summary>
        /// Get latest price for all symbols.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<IEnumerable<SymbolPrice>> GetPricesAsync(CancellationToken token = default);

        /// <summary>
        /// Get best price/quantity on the order book for a symbol.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<OrderBookTop> GetOrderBookTopAsync(string symbol, CancellationToken token = default);

        /// <summary>
        /// Get best price/quantity on the order book for all symbols.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<IEnumerable<OrderBookTop>> GetOrderBookTopsAsync(CancellationToken token = default);

        /// <summary>
        /// Get all symbols.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<IEnumerable<Symbol>> GetSymbolsAsync(CancellationToken token = default);

        #endregion Market Data

        #region Account

        /// <summary>
        /// Place an order. The client order properties determine order side, type, etc.
        /// </summary>
        /// <param name="clientOrder"></param>
        /// <param name="recvWindow"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<Order> PlaceAsync(ClientOrder clientOrder, long recvWindow = default, CancellationToken token = default);

        /// <summary>
        /// Place a TEST order. The client order properties determine order side, type, etc.
        /// Throws a <see cref="BinanceApiException"/> if the order placement test fails.
        /// </summary>
        /// <param name="clientOrder"></param>
        /// <param name="recvWindow"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task TestPlaceAsync(ClientOrder clientOrder, long recvWindow = default, CancellationToken token = default);

        /// <summary>
        /// Get order by ID.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="symbol"></param>
        /// <param name="orderId"></param>
        /// <param name="recvWindow"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<Order> GetOrderAsync(IBinanceApiUser user, string symbol, long orderId, long recvWindow = default, CancellationToken token = default);

        /// <summary>
        /// Get order by original client order ID.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="symbol"></param>
        /// <param name="origClientOrderId"></param>
        /// <param name="recvWindow"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<Order> GetOrderAsync(IBinanceApiUser user, string symbol, string origClientOrderId, long recvWindow = default, CancellationToken token = default);

        /// <summary>
        /// Get latest order status (fill in place and return the order instance).
        /// </summary>
        /// <param name="order"></param>
        /// <param name="recvWindow"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<Order> GetAsync(Order order, long recvWindow = default, CancellationToken token = default);

        /// <summary>
        /// Cancel and order by ID.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="symbol"></param>
        /// <param name="orderId"></param>
        /// <param name="newClientOrderId"></param>
        /// <param name="recvWindow"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<string> CancelOrderAsync(IBinanceApiUser user, string symbol, long orderId, string newClientOrderId = null, long recvWindow = default, CancellationToken token = default);

        /// <summary>
        /// Cancel an order by original client order ID.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="symbol"></param>
        /// <param name="origClientOrderId"></param>
        /// <param name="newClientOrderId">Used to uniquely identify this cancel. Automatically generated by default.</param>
        /// <param name="recvWindow"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<string> CancelOrderAsync(IBinanceApiUser user, string symbol, string origClientOrderId, string newClientOrderId = null, long recvWindow = default, CancellationToken token = default);

        /// <summary>
        /// Get all open orders on a symbol or all symbols.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="symbol"></param>
        /// <param name="recvWindow"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<IEnumerable<Order>> GetOpenOrdersAsync(IBinanceApiUser user, string symbol = null, long recvWindow = default, CancellationToken token = default);

        /// <summary>
        /// Get all account orders; active, canceled, or filled.
        /// If orderId is set, this will return orders >= orderId; otherwise return most recent orders.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="symbol"></param>
        /// <param name="orderId"></param>
        /// <param name="limit"></param>
        /// <param name="recvWindow"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<IEnumerable<Order>> GetOrdersAsync(IBinanceApiUser user, string symbol, long orderId = BinanceApi.NullId, int limit = default, long recvWindow = default, CancellationToken token = default);

        /// <summary>
        /// Get current account information.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="recvWindow"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<AccountInfo> GetAccountInfoAsync(IBinanceApiUser user, long recvWindow = default, CancellationToken token = default);

        /// <summary>
        /// Get trades for a specific account and symbol.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="symbol"></param>
        /// <param name="fromId"></param>
        /// <param name="limit"></param>
        /// <param name="recvWindow"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<IEnumerable<AccountTrade>> GetAccountTradesAsync(IBinanceApiUser user, string symbol, long fromId = BinanceApi.NullId, int limit = default, long recvWindow = default, CancellationToken token = default);

        /// <summary>
        /// Submit a withdraw request.
        /// </summary>
        /// <param name="withdrawRequest"></param>
        /// <param name="recvWindow"></param>
        /// <param name="token"></param>
        /// <returns>The withdrawal ID if successful.</returns>
        Task<string> WithdrawAsync(WithdrawRequest withdrawRequest, long recvWindow = default, CancellationToken token = default);

        /// <summary>
        /// Get the deposit history.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="asset"></param>
        /// <param name="status"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="recvWindow"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<IEnumerable<Deposit>> GetDepositsAsync(IBinanceApiUser user, string asset, DepositStatus? status = null, long startTime = 0, long endTime = 0, long recvWindow = 0, CancellationToken token = default);

        /// <summary>
        /// Get the withdrawal history.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="asset"></param>
        /// <param name="status"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="recvWindow"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<IEnumerable<Withdrawal>> GetWithdrawalsAsync(IBinanceApiUser user, string asset, WithdrawalStatus? status = null, long startTime = 0, long endTime = 0, long recvWindow = 0, CancellationToken token = default);

        /// <summary>
        /// Get the deposit address for an asset.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="asset"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<DepositAddress> GetDepositAddressAsync(IBinanceApiUser user, string asset, CancellationToken token = default);

        /// <summary>
        /// Get account status.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<string> GetAccountStatusAsync(IBinanceApiUser user, CancellationToken token = default);

        #endregion Account

        #region User Stream

        /// <summary>
        /// Start a new user data stream.
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<string> UserStreamStartAsync(string apiKey, CancellationToken token = default);

        /// <summary>
        /// Ping a user data stream to prevent a time out.
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="listenKey"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task UserStreamKeepAliveAsync(string apiKey, string listenKey, CancellationToken token = default);

        /// <summary>
        /// Close out a user data stream.
        /// </summary>
        /// <param name="apiKey"></param>
        /// <param name="listenKey"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task UserStreamCloseAsync(string apiKey, string listenKey, CancellationToken token = default);

        #endregion User Stream
    }
}
