using Binance.Accounts;
using Binance.Api;
using Binance.Orders;
using Binance.Orders.Book;
using Binance.Trades;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Binance
{
    public interface IBinanceApi
    {
        #region Public Properties

        /// <summary>
        /// The (low-level) JSON web API.
        /// </summary>
        IBinanceJsonApi JsonApi { get; }

        #endregion Public Properties

        #region Connectivity

        /// <summary>
        /// Test connectivity to the server.
        /// </summary>
        /// <returns></returns>
        Task<bool> PingAsync(CancellationToken token = default);

        /// <summary>
        /// Test connectivity to the server and get the current time.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<DateTime> GetTimeAsync(CancellationToken token = default);

        #endregion Connectivity

        #region Market Data

        /// <summary>
        /// Get order book (market depth) of a symbol.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="limit">Default 100; max 100.</param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<OrderBook> GetOrderBookAsync(string symbol, int limit = BinanceApi.OrderBookLimitDefault, CancellationToken token = default);

        /// <summary>
        /// Get compressed, aggregate trades. Trades that fill at the time, from the same order, with the same price will have the quantity aggregated.
        /// If fromdId, startTime, and endTime are not sent, the most recent aggregate trades will be returned.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="fromId">ID to get aggregate trades from INCLUSIVE.</param>
        /// <param name="startTime">Timestamp in ms to get aggregate trades from INCLUSIVE.</param>
        /// <param name="endTime">Timestamp in ms to get aggregate trades until INCLUSIVE.</param>
        /// <param name="limit">Default 500; max 500.</param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<IEnumerable<AggregateTrade>> GetAggregateTradesAsync(string symbol, long fromId = BinanceApi.NullTradeId, long startTime = 0, long endTime = 0, int limit = BinanceApi.TradesLimitDefault, CancellationToken token = default);

        /// <summary>
        /// Get Kline/candlestick bars for a symbol. Klines are uniquely identified by their open time.
        /// If startTime and endTime are not sent, the most recent klines are returned.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="interval"></param>
        /// <param name="limit">Default 500; max 500.</param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<IEnumerable<Candlestick>> GetCandlesticksAsync(string symbol, KlineInterval interval, int limit = BinanceApi.CandlesticksLimitDefault, long startTime = 0, long endTime = 0, CancellationToken token = default);

        /// <summary>
        /// Get 24 hour price change statistics for a symbol.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<Symbol24hrStats> Get24hrStatsAsync(string symbol, CancellationToken token = default);

        /// <summary>
        /// Get latest price for all symbols.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<IEnumerable<SymbolPrice>> GetPricesAsync(CancellationToken token = default);

        /// <summary>
        /// Get best price/quantity on the order book for all symbols.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<IEnumerable<OrderBookTop>> GetOrderBookTopsAsync(CancellationToken token = default);

        #endregion Market Data

        #region Account

        /// <summary>
        /// Place an order. The client order properties determine type of order, if test-only, etc.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="clientOrder"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<Order> PlaceAsync(IBinanceUser user, ClientOrder clientOrder, long recvWindow = BinanceApi.RecvWindowDefault, CancellationToken token = default);

        /// <summary>
        /// Get order by ID.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="symbol"></param>
        /// <param name="orderId"></param>
        /// <param name="recvWindow"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<Order> GetOrderAsync(IBinanceUser user, string symbol, long orderId, long recvWindow = BinanceApi.RecvWindowDefault, CancellationToken token = default);

        /// <summary>
        /// Get order by original client order ID.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="symbol"></param>
        /// <param name="origClientOrderId"></param>
        /// <param name="recvWindow"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<Order> GetOrderAsync(IBinanceUser user, string symbol, string origClientOrderId, long recvWindow = BinanceApi.RecvWindowDefault, CancellationToken token = default);

        /// <summary>
        /// Get latest order status (fill in place and return order instance).
        /// </summary>
        /// <param name="user"></param>
        /// <param name="order"></param>
        /// <param name="recvWindow"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<Order> GetOrderAsync(IBinanceUser user, Order order, long recvWindow = BinanceApi.RecvWindowDefault, CancellationToken token = default);

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
        Task<string> CancelOrderAsync(IBinanceUser user, string symbol, long orderId, string newClientOrderId = null, long recvWindow = BinanceApi.RecvWindowDefault, CancellationToken token = default);

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
        Task<string> CancelOrderAsync(IBinanceUser user, string symbol, string origClientOrderId, string newClientOrderId = null, long recvWindow = BinanceApi.RecvWindowDefault, CancellationToken token = default);

        /// <summary>
        /// Cancel an order.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="order"></param>
        /// <param name="newClientOrderId"></param>
        /// <param name="recvWindow"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<string> CancelAsync(IBinanceUser user, Order order, string newClientOrderId = null, long recvWindow = BinanceApi.RecvWindowDefault, CancellationToken token = default);

        /// <summary>
        /// Get all open orders on a symbol.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="symbol"></param>
        /// <param name="recvWindow"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<IEnumerable<Order>> GetOpenOrdersAsync(IBinanceUser user, string symbol, long recvWindow = BinanceApi.RecvWindowDefault, CancellationToken token = default);

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
        Task<IEnumerable<Order>> GetOrdersAsync(IBinanceUser user, string symbol, long orderId = 0, int limit = BinanceApi.OrdersLimitDefault, long recvWindow = BinanceApi.RecvWindowDefault, CancellationToken token = default);

        /// <summary>
        /// Get current account information.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<Account> GetAccountAsync(IBinanceUser user, long recvWindow = BinanceApi.RecvWindowDefault, CancellationToken token = default);

        /// <summary>
        /// Get trades for a specific account and symbol.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="symbol"></param>
        /// <param name="limit"></param>
        /// <param name="fromId"></param>
        /// <param name="recvWindow"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<IEnumerable<AccountTrade>> GetTradesAsync(IBinanceUser user, string symbol, int limit = BinanceApi.TradesLimitDefault, long fromId = 0, long recvWindow = BinanceApi.RecvWindowDefault, CancellationToken token = default);

        /// <summary>
        /// Submit a withdraw request.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="asset"></param>
        /// <param name="address"></param>
        /// <param name="amount"></param>
        /// <param name="name"></param>
        /// <param name="recvWindow"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task WithdrawAsync(IBinanceUser user, string asset, string address, decimal amount, string name, long recvWindow = BinanceApi.RecvWindowDefault, CancellationToken token = default);

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
        Task<IEnumerable<Deposit>> GetDepositsAsync(IBinanceUser user, string asset, DepositStatus? status, long startTime = 0, long endTime = 0, long recvWindow = BinanceApi.RecvWindowDefault, CancellationToken token = default);

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
        Task<IEnumerable<Withdrawal>> GetWithdrawalsAsync(IBinanceUser user, string asset, WithdrawalStatus? status, long startTime = 0, long endTime = 0, long recvWindow = BinanceApi.RecvWindowDefault, CancellationToken token = default);

        #endregion Account

        #region User Stream

        /// <summary>
        /// Start a new user data stream.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<string> UserStreamStartAsync(IBinanceUser user, CancellationToken token = default);

        /// <summary>
        /// Ping a user data stream to prevent a time out.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="listenKey"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task UserStreamKeepAliveAsync(IBinanceUser user, string listenKey, CancellationToken token = default);

        /// <summary>
        /// Close out a user data stream.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="listenKey"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task UserStreamCloseAsync(IBinanceUser user, string listenKey, CancellationToken token = default);

        #endregion User Stream
    }
}
