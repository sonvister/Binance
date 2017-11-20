using System;
using System.Threading;
using System.Threading.Tasks;
using Binance.Account;
using Binance.Account.Orders;
using Binance.Market;

namespace Binance.Api.Json
{
    public interface IBinanceJsonApi : IDisposable
    {
        #region Public Properties

        /// <summary>
        /// The API request rate limiter.
        /// </summary>
        IApiRateLimiter RateLimiter { get; }

        #endregion Public Properties

        #region Connectivity

        /// <summary>
        /// Test connectivity to the server.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<string> PingAsync(CancellationToken token = default);

        /// <summary>
        /// Test connectivity to the server and get the current time.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<string> GetServerTimeAsync(CancellationToken token = default);

        #endregion Connectivity

        #region Market Data

        /// <summary>
        /// Get exchange information.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<string> GetExchangeInfoAsync(CancellationToken token = default);

        /// <summary>
        /// Get order book (market depth) of a symbol.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="limit">Default 100; max 100.</param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<string> GetOrderBookAsync(string symbol, int limit = default, CancellationToken token = default);

        /// <summary>
        /// Get compressed, aggregate trades. Trades that fill at the time, from the same order, with the same price will have the quantity aggregated.
        /// If fromdId, startTime, and endTime are not sent, the most recent aggregate trades will be returned.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="fromId">ID to get aggregate trades from INCLUSIVE.</param>
        /// <param name="limit">Default 500; max 500.</param>
        /// <param name="startTime">Timestamp in ms to get aggregate trades from INCLUSIVE.</param>
        /// <param name="endTime">Timestamp in ms to get aggregate trades until INCLUSIVE.</param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<string> GetAggregateTradesAsync(string symbol, long fromId = BinanceApi.NullId, int limit = default, long startTime = default, long endTime = default, CancellationToken token = default);

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
        Task<string> GetCandlesticksAsync(string symbol, CandlestickInterval interval, int limit = default, long startTime = default, long endTime = default, CancellationToken token = default);

        /// <summary>
        /// Get 24 hour price change statistics for a symbol.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<string> Get24HourStatisticsAsync(string symbol, CancellationToken token = default);

        /// <summary>
        /// Get latest price for all symbols.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<string> GetPricesAsync(CancellationToken token = default);

        /// <summary>
        /// Get best price/quantity on the order book for all symbols.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<string> GetOrderBookTopsAsync(CancellationToken token = default);

        #endregion Market Data

        #region Account

        /// <summary>
        /// Send in a new order.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="symbol"></param>
        /// <param name="side"></param>
        /// <param name="type"></param>
        /// <param name="quantity"></param>
        /// <param name="price"></param>
        /// <param name="newClientOrderId">A unique id for the order. Automatically generated if not sent.</param>
        /// <param name="timeInForce"></param>
        /// <param name="stopPrice">Used with stop orders.</param>
        /// <param name="icebergQty">Used with iceberg orders.</param>
        /// <param name="recvWindow"></param>
        /// <param name="isTestOnly">If true, test new order creation and signature/recvWindow; creates and validates a new order but does not send it into the matching engine.</param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<string> PlaceOrderAsync(IBinanceApiUser user, string symbol, OrderSide side, OrderType type, decimal quantity, decimal price, string newClientOrderId = null, TimeInForce? timeInForce = null, decimal stopPrice = 0, decimal icebergQty = 0, long recvWindow = default, bool isTestOnly = false, CancellationToken token = default);

        /// <summary>
        /// Check an order's status. Either orderId or origClientOrderId must be sent.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="symbol"></param>
        /// <param name="orderId"></param>
        /// <param name="origClientOrderId"></param>
        /// <param name="recvWindow"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<string> GetOrderAsync(IBinanceApiUser user, string symbol, long orderId = BinanceApi.NullId, string origClientOrderId = null, long recvWindow = default, CancellationToken token = default);

        /// <summary>
        /// Cancel an active order.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="symbol"></param>
        /// <param name="orderId"></param>
        /// <param name="origClientOrderId"></param>
        /// <param name="newClientOrderId">Used to uniquely identify this cancel. Automatically generated by default.</param>
        /// <param name="recvWindow"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<string> CancelOrderAsync(IBinanceApiUser user, string symbol, long orderId = BinanceApi.NullId, string origClientOrderId = null, string newClientOrderId = null, long recvWindow = default, CancellationToken token = default);

        /// <summary>
        /// Get all open orders on a symbol.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="symbol"></param>
        /// <param name="recvWindow"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<string> GetOpenOrdersAsync(IBinanceApiUser user, string symbol, long recvWindow = default, CancellationToken token = default);

        /// <summary>
        /// Get all account orders; active, canceled, or filled.
        /// If orderId is set, this will return orders >= orderId; otherwise return most recent orders.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="symbol"></param>
        /// <param name="orderId"></param>
        /// <param name="limit">Default 500; max 500.</param>
        /// <param name="recvWindow"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<string> GetOrdersAsync(IBinanceApiUser user, string symbol, long orderId = BinanceApi.NullId, int limit = default, long recvWindow = default, CancellationToken token = default);

        /// <summary>
        /// Get current account information.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="recvWindow"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<string> GetAccountInfoAsync(IBinanceApiUser user, long recvWindow = default, CancellationToken token = default);

        /// <summary>
        /// Get trades for a specific account and symbol.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="symbol"></param>
        /// <param name="fromId">TradeId to fetch from. Default gets most recent trades.</param>
        /// <param name="limit">Default 500; max 500.</param>
        /// <param name="recvWindow"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<string> GetTradesAsync(IBinanceApiUser user, string symbol, long fromId = BinanceApi.NullId, int limit = default, long recvWindow = default, CancellationToken token = default);

        /// <summary>
        /// Submit a withdraw request.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="asset"></param>
        /// <param name="address"></param>
        /// <param name="amount"></param>
        /// <param name="name">A description of the address (optional).</param>
        /// <param name="recvWindow"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<string> WithdrawAsync(IBinanceApiUser user, string asset, string address, decimal amount, string name = null, long recvWindow = default, CancellationToken token = default);

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
        Task<string> GetDepositsAsync(IBinanceApiUser user, string asset = null, DepositStatus? status = null, long startTime = default, long endTime = default, long recvWindow = default, CancellationToken token = default);

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
        Task<string> GetWithdrawalsAsync(IBinanceApiUser user, string asset = null, WithdrawalStatus? status = null, long startTime = default, long endTime = default, long recvWindow = default, CancellationToken token = default);

        #endregion Account

        #region User Stream

        /// <summary>
        /// Start a new user data stream.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<string> UserStreamStartAsync(IBinanceApiUser user, CancellationToken token = default);

        /// <summary>
        /// Ping a user data stream to prevent a time out.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="listenKey"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<string> UserStreamKeepAliveAsync(IBinanceApiUser user, string listenKey, CancellationToken token = default);

        /// <summary>
        /// Close out a user data stream.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="listenKey"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<string> UserStreamCloseAsync(IBinanceApiUser user, string listenKey, CancellationToken token = default);

        #endregion User Stream
    }
}
