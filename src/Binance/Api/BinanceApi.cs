using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Binance.Account;
using Binance.Account.Orders;
using Binance.Api.RateLimit;
using Binance.Market;
using Binance.Serialization;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Binance.Api
{
    /// <summary>
    /// Binance API <see cref="IBinanceApi"/> implementation.
    /// </summary>
    public class BinanceApi : IBinanceApi
    {
        #region Public Constants

        /// <summary>
        /// Constant used to represent an invalid/null identifier of:
        /// orders, account trades, aggregate trades, etc.
        /// </summary>
        public const long NullId = -1;

        #endregion Public Constants

        #region Public Properties

        /// <summary>
        /// Get the low-level <see cref="IBinanceHttpClient"/> singleton.
        /// </summary>
        public IBinanceHttpClient HttpClient { get; }

        #endregion Public Properties

        #region Private Fields

        private readonly IOrderBookSerializer _orderBookSerializer;

        private readonly IOrderBookTopSerializer _orderBookTopSerializer;

        private readonly IAggregateTradeSerializer _aggregateTradeSerializer;

        private readonly ICandlestickSerializer _candlestickSerializer;

        private readonly ISymbolPriceSerializer _symbolPriceSerializer;

        private readonly ISymbolStatisticsSerializer _symbolStatisticsSerializer;

        private readonly IAccountTradeSerializer _accountTradeSerializer;

        private readonly ITradeSerializer _tradeSerializer;

        private readonly IOrderSerializer _orderSerializer;

        private readonly ILogger<BinanceApi> _logger;

        #endregion Private Fields

        #region Constructors

        /// <summary>
        /// Default constructor provides default timestamp provider, default
        /// rate limiter, and default options, but no logging functionality.
        /// </summary>
        public BinanceApi()
            : this(BinanceHttpClient.Instance)
        { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="orderBookSerializer"></param>
        /// <param name="orderBookTopSerializer"></param>
        /// <param name="aggregateTradeSerializer"></param>
        /// <param name="candlestickSerializer"></param>
        /// <param name="symbolPriceSerializer"></param>
        /// <param name="symbolStatisticsSerializer"></param>
        /// <param name="accountTradeSerializer"></param>
        /// <param name="tradeSerializer"></param>
        /// <param name="orderSerializer"></param>
        /// <param name="logger"></param>
        public BinanceApi(
            IBinanceHttpClient client,
            IOrderBookSerializer orderBookSerializer = null,
            IOrderBookTopSerializer orderBookTopSerializer = null,
            IAggregateTradeSerializer aggregateTradeSerializer = null,
            ICandlestickSerializer candlestickSerializer = null,
            ISymbolPriceSerializer symbolPriceSerializer = null,
            ISymbolStatisticsSerializer symbolStatisticsSerializer = null,
            IAccountTradeSerializer accountTradeSerializer = null,
            ITradeSerializer tradeSerializer = null,
            IOrderSerializer orderSerializer = null,
            ILogger<BinanceApi> logger = null)
        {
            Throw.IfNull(client, nameof(client));

            HttpClient = client;

            _orderBookSerializer = orderBookSerializer ?? new OrderBookSerializer();
            _orderBookTopSerializer = orderBookTopSerializer ?? new OrderBookTopSerializer();
            _aggregateTradeSerializer = aggregateTradeSerializer ?? new AggregateTradeSerializer();
            _candlestickSerializer = candlestickSerializer ?? new CandlestickSerializer();
            _symbolPriceSerializer = symbolPriceSerializer ?? new SymbolPriceSerializer();
            _symbolStatisticsSerializer = symbolStatisticsSerializer ?? new SymbolStatisticsSerializer();
            _accountTradeSerializer = accountTradeSerializer ?? new AccountTradeSerializer();
            _tradeSerializer = tradeSerializer ?? new TradeSerializer();
            _orderSerializer = orderSerializer ?? new OrderSerializer();

            _logger = logger;
        }

        #endregion Constructors

        #region Connectivity

        public virtual async Task<bool> PingAsync(CancellationToken token = default)
        {
            return await HttpClient.PingAsync(token).ConfigureAwait(false)
                   == BinanceHttpClient.SuccessfulTestResponse;
        }

        public virtual async Task<long> GetTimestampAsync(CancellationToken token = default)
        {
            var json = await HttpClient.GetServerTimeAsync(token)
                .ConfigureAwait(false);

            try
            {
                return JObject.Parse(json)["serverTime"].Value<long>();
            }
            catch (Exception e)
            {
                throw NewFailedToParseJsonException(nameof(GetTimestampAsync), json, e);
            }
        }

        public virtual async Task<IEnumerable<RateLimitInfo>> GetRateLimitInfoAsync(CancellationToken token = default)
        {
            var json = await HttpClient.GetExchangeInfoAsync(token)
                .ConfigureAwait(false);

            var rateLimits = new List<RateLimitInfo>();

            try
            {
                var jObject = JObject.Parse(json);

                var jArray = jObject["rateLimits"];

                if (jArray != null)
                {
                    rateLimits.AddRange(
                        jArray.Select(jToken =>
                            new RateLimitInfo(
                                jToken["rateLimitType"].Value<string>(),
                                jToken["interval"].Value<string>(),
                                jToken["limit"].Value<int>())));
                }
            }
            catch (Exception e)
            {
                throw NewFailedToParseJsonException(nameof(GetRateLimitInfoAsync), json, e);
            }

            return rateLimits;
        }

        public virtual async Task<BinanceStatus> GetSystemStatusAsync(CancellationToken token = default)
        {
            var json = await HttpClient.GetSystemStatusAsync(token)
                .ConfigureAwait(false);

            try
            {
                var jObject = JObject.Parse(json);

                var status = jObject["status"].Value<int>();
                var msg = jObject["msg"].Value<string>();

                switch (status)
                {
                    case 0: return BinanceStatus.Normal;
                    case 1: return BinanceStatus.Maintenance;
                    default:
                        throw new BinanceApiException($"Unknown Status ({status}): \"{msg}\"");
                }
            }
            catch (Exception e)
            {
                throw NewFailedToParseJsonException(nameof(GetSystemStatusAsync), json, e);
            }
        }

        #endregion Connectivity

        #region Market Data

        public virtual async Task<OrderBook> GetOrderBookAsync(string symbol, int limit = default, CancellationToken token = default)
        {
            var json = await HttpClient.GetOrderBookAsync(symbol, limit, token)
                .ConfigureAwait(false);

            try { return _orderBookSerializer.Deserialize(OrderBookJsonConverter.InsertSymbol(json, symbol)); }
            catch (Exception e)
            {
                throw NewFailedToParseJsonException(nameof(GetOrderBookAsync), json, e);
            }
        }

        public virtual async Task<IEnumerable<Trade>> GetTradesAsync(string symbol, int limit = default, CancellationToken token = default)
        {
            var json = await HttpClient.GetTradesAsync(symbol, limit, token)
                .ConfigureAwait(false);

            try { return _tradeSerializer.DeserializeMany(json, symbol); }
            catch (Exception e)
            {
                throw NewFailedToParseJsonException(nameof(GetTradesAsync), json, e);
            }
        }

        public virtual async Task<IEnumerable<Trade>> GetTradesFromAsync(string apiKey, string symbol, long fromId, int limit = default, CancellationToken token = default)
        {
            if (fromId < 0)
                throw new ArgumentException($"ID ({nameof(fromId)}) must not be less than 0.", nameof(fromId));

            var json = await HttpClient.GetTradesAsync(apiKey, symbol, fromId, limit, token)
                .ConfigureAwait(false);

            try { return _tradeSerializer.DeserializeMany(json, symbol); }
            catch (Exception e)
            {
                throw NewFailedToParseJsonException(nameof(GetTradesFromAsync), json, e);
            }
        }

        public virtual async Task<IEnumerable<AggregateTrade>> GetAggregateTradesAsync(string symbol, int limit = default, CancellationToken token = default)
        {
            var json = await HttpClient.GetAggregateTradesAsync(symbol, NullId, limit, token: token)
                .ConfigureAwait(false);

            try { return _aggregateTradeSerializer.DeserializeMany(json, symbol); }
            catch (Exception e)
            {
                throw NewFailedToParseJsonException(nameof(GetAggregateTradesAsync), json, e);
            }
        }

        public virtual async Task<IEnumerable<AggregateTrade>> GetAggregateTradesFromAsync(string symbol, long fromId, int limit = default, CancellationToken token = default)
        {
            if (fromId < 0)
                throw new ArgumentException($"ID ({nameof(fromId)}) must not be less than 0.", nameof(fromId));

            var json = await HttpClient.GetAggregateTradesAsync(symbol, fromId, limit, token: token)
                .ConfigureAwait(false);

            try { return _aggregateTradeSerializer.DeserializeMany(json, symbol); }
            catch (Exception e)
            {
                throw NewFailedToParseJsonException(nameof(GetAggregateTradesFromAsync), json, e);
            }
        }

        public virtual async Task<IEnumerable<AggregateTrade>> GetAggregateTradesAsync(string symbol, DateTime startTime, DateTime endTime, CancellationToken token = default)
        {
            // NOTE: Limit does not apply when using start and end time.
            var json = await HttpClient.GetAggregateTradesAsync(symbol, NullId, default, startTime, endTime, token)
                .ConfigureAwait(false);

            try { return _aggregateTradeSerializer.DeserializeMany(json, symbol); }
            catch (Exception e)
            {
                throw NewFailedToParseJsonException(nameof(GetAggregateTradesAsync), json, e);
            }
        }

        public virtual async Task<IEnumerable<Candlestick>> GetCandlesticksAsync(string symbol, CandlestickInterval interval, int limit = default, DateTime startTime = default, DateTime endTime = default, CancellationToken token = default)
        {
            var json = await HttpClient.GetCandlesticksAsync(symbol, interval, limit, startTime, endTime, token)
                .ConfigureAwait(false);

            try { return _candlestickSerializer.DeserializeMany(json, symbol, interval); }
            catch (Exception e)
            {
                throw NewFailedToParseJsonException(nameof(GetCandlesticksAsync), json, e);
            }
        }

        public virtual async Task<SymbolStatistics> Get24HourStatisticsAsync(string symbol, CancellationToken token = default)
        {
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            var json = await HttpClient.Get24HourStatisticsAsync(symbol, token)
                .ConfigureAwait(false);

            try { return _symbolStatisticsSerializer.Deserialize(json); }
            catch (Exception e)
            {
                throw NewFailedToParseJsonException(nameof(Get24HourStatisticsAsync), json, e);
            }
        }

        public virtual async Task<IEnumerable<SymbolStatistics>> Get24HourStatisticsAsync(CancellationToken token = default)
        {
            var json = await HttpClient.Get24HourStatisticsAsync(token: token)
                .ConfigureAwait(false);

            try { return _symbolStatisticsSerializer.DeserializeMany(json); }
            catch (Exception e)
            {
                throw NewFailedToParseJsonException(nameof(Get24HourStatisticsAsync), json, e);
            }
        }

        public virtual async Task<SymbolPrice> GetPriceAsync(string symbol, CancellationToken token = default)
        {
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            var json = await HttpClient.GetPriceAsync(symbol, token)
                .ConfigureAwait(false);

            try { return _symbolPriceSerializer.Deserialize(json); }
            catch (Exception e)
            {
                throw NewFailedToParseJsonException(nameof(GetPricesAsync), json, e);
            }
        }

        public virtual async Task<IEnumerable<SymbolPrice>> GetPricesAsync(CancellationToken token = default)
        {
            var json = await HttpClient.GetPriceAsync(token: token)
                .ConfigureAwait(false);

            try { return _symbolPriceSerializer.DeserializeMany(json); }
            catch (Exception e)
            {
                throw NewFailedToParseJsonException(nameof(GetPricesAsync), json, e);
            }
        }

        public virtual async Task<OrderBookTop> GetOrderBookTopAsync(string symbol, CancellationToken token = default)
        {
            var json = await HttpClient.GetOrderBookTopAsync(symbol, token)
                .ConfigureAwait(false);

            try { return _orderBookTopSerializer.Deserialize(json); }
            catch (Exception e)
            {
                throw NewFailedToParseJsonException(nameof(GetOrderBookTopAsync), json, e);
            }
        }

        public virtual async Task<IEnumerable<OrderBookTop>> GetOrderBookTopsAsync(CancellationToken token = default)
        {
            var json = await HttpClient.GetOrderBookTopsAsync(token)
                .ConfigureAwait(false);

            try { return _orderBookTopSerializer.DeserializeMany(json); }
            catch (Exception e)
            {
                throw NewFailedToParseJsonException(nameof(GetOrderBookTopsAsync), json, e);
            }
        }

        public virtual async Task<IEnumerable<Symbol>> GetSymbolsAsync(CancellationToken token = default)
        {
            var json = await HttpClient.GetExchangeInfoAsync(token)
                .ConfigureAwait(false);

            var symbols = new List<Symbol>();

            try
            {
                var jObject = JObject.Parse(json);

                var jArray = jObject["symbols"];

                if (jArray != null)
                {
                    symbols.AddRange(
                        jArray.Select(jToken =>
                        {
                            var status = jToken["status"].Value<string>().ConvertSymbolStatus();
                            var icebergAllowed = jToken["icebergAllowed"].Value<bool>();

                            // HACK: Support inconsistent precision naming and possible future changes.
                            var baseAssetPrecision = jToken["baseAssetPrecision"]?.Value<int>() ?? jToken["basePrecision"]?.Value<int>() ?? 0;
                            var quoteAssetPrecision = jToken["quoteAssetPrecision"]?.Value<int>() ?? jToken["quotePrecision"]?.Value<int>() ?? 0;

                            var baseAsset = new Asset(jToken["baseAsset"].Value<string>(), baseAssetPrecision);
                            var quoteAsset = new Asset(jToken["quoteAsset"].Value<string>(), quoteAssetPrecision);

                            var orderTypes = jToken["orderTypes"]
                                .Select(orderType => orderType.Value<string>().ConvertOrderType())
                                .ToArray();

                            var filters = jToken["filters"];

                            var quoteMinPrice = filters[0]["minPrice"].Value<decimal>();
                            var quoteMaxPrice = filters[0]["maxPrice"].Value<decimal>();
                            var quoteIncrement = filters[0]["tickSize"].Value<decimal>();

                            var baseMinQty = filters[1]["minQty"].Value<decimal>();
                            var baseMaxQty = filters[1]["maxQty"].Value<decimal>();
                            var baseIncrement = filters[1]["stepSize"].Value<decimal>();

                            var minNotional = filters[2]["minNotional"].Value<decimal>();

                            var symbol = new Symbol(status, baseAsset, quoteAsset, (baseMinQty, baseMaxQty, baseIncrement), (quoteMinPrice, quoteMaxPrice, quoteIncrement), minNotional, icebergAllowed, orderTypes);

                            if (symbol.ToString() == jToken["symbol"].Value<string>())
                                return symbol;

                            _logger?.LogDebug($"Symbol does not match trading pair assets ({jToken["symbol"].Value<string>()} != {symbol}).");
                            return null; // invalid symbol (e.g. 'ETC').
                        }));
                }
            }
            catch (Exception e)
            {
                throw NewFailedToParseJsonException(nameof(GetSymbolsAsync), json, e);
            }

            return symbols
                .Where(_ => _ != null && _.ToString() != "123456"); // HACK
        }

        #endregion Market Data

        #region Account

        public virtual async Task<Order> PlaceAsync(ClientOrder clientOrder, long recvWindow = default, CancellationToken token = default)
        {
            Throw.IfNull(clientOrder, nameof(clientOrder));

            var limitOrder = clientOrder as LimitOrder;
            var stopOrder = clientOrder as IStopOrder;

            var order = new Order(clientOrder.User)
            {
                Symbol = clientOrder.Symbol.FormatSymbol(),
                OriginalQuantity = clientOrder.Quantity,
                Price = limitOrder?.Price ?? 0,
                Side = clientOrder.Side,
                Type = clientOrder.Type,
                Status = OrderStatus.New,
                TimeInForce = limitOrder?.TimeInForce ?? TimeInForce.GTC
            };

            // Place the order.
            var json = await HttpClient.PlaceOrderAsync(clientOrder.User, clientOrder.Symbol, clientOrder.Side, clientOrder.Type,
                clientOrder.Quantity, limitOrder?.Price ?? 0, clientOrder.Id, clientOrder.Type == OrderType.LimitMaker ? null : limitOrder?.TimeInForce,
                stopOrder?.StopPrice ?? 0, limitOrder?.IcebergQuantity ?? 0, recvWindow, false, PlaceOrderResponseType.Full, token);

            try
            {
                // Update existing order properties.
                _orderSerializer.Deserialize(json, order);

                // Update client order properties.
                clientOrder.Id = order.ClientOrderId;
                clientOrder.Time = order.Time;
            }
            catch (Exception e)
            {
                throw NewFailedToParseJsonException(nameof(PlaceAsync), json, e);
            }

            return order;
        }

        public virtual async Task TestPlaceAsync(ClientOrder clientOrder, long recvWindow = default, CancellationToken token = default)
        {
            Throw.IfNull(clientOrder, nameof(clientOrder));

            var limitOrder = clientOrder as LimitOrder;
            var stopOrder = clientOrder as IStopOrder;

            // Place the TEST order.
            var json = await HttpClient.PlaceOrderAsync(clientOrder.User, clientOrder.Symbol, clientOrder.Side, clientOrder.Type,
                clientOrder.Quantity, limitOrder?.Price ?? 0, clientOrder.Id, clientOrder.Type == OrderType.LimitMaker ? null : limitOrder?.TimeInForce,
                stopOrder?.StopPrice ?? 0, limitOrder?.IcebergQuantity ?? 0, recvWindow, true, PlaceOrderResponseType.Ack, token);

            if (json != BinanceHttpClient.SuccessfulTestResponse)
            {
                var message = $"{nameof(BinanceApi)}.{nameof(TestPlaceAsync)} failed order placement test.";
                _logger?.LogError(message);
                throw new BinanceApiException(message);
            }
        }

        public virtual async Task<Order> GetOrderAsync(IBinanceApiUser user, string symbol, long orderId, long recvWindow = default, CancellationToken token = default)
        {
            // Get order using order ID.
            var json = await HttpClient.GetOrderAsync(user, symbol, orderId, null, recvWindow, token)
                .ConfigureAwait(false);

            try { return _orderSerializer.Deserialize(json, user); }
            catch (Exception e)
            {
                throw NewFailedToParseJsonException(nameof(GetOrderAsync), json, e);
            }
        }

        public virtual async Task<Order> GetOrderAsync(IBinanceApiUser user, string symbol, string origClientOrderId, long recvWindow = default, CancellationToken token = default)
        {
            // Get order using original client order ID.
            var json = await HttpClient.GetOrderAsync(user, symbol, NullId, origClientOrderId, recvWindow, token)
                .ConfigureAwait(false);

            try { return _orderSerializer.Deserialize(json, user); }
            catch (Exception e)
            {
                throw NewFailedToParseJsonException(nameof(GetOrderAsync), json, e);
            }
        }

        public virtual async Task<Order> GetAsync(Order order, long recvWindow = default, CancellationToken token = default)
        {
            Throw.IfNull(order, nameof(order));

            // Get order using order ID.
            var json = await HttpClient.GetOrderAsync(order.User, order.Symbol, order.Id, null, recvWindow, token)
                .ConfigureAwait(false);

            // Update existing order properties.
            try { return _orderSerializer.Deserialize(json, order); }
            catch (Exception e)
            {
                throw NewFailedToParseJsonException($"{nameof(GetAsync)}({nameof(Order)})", json, e);
            }
        }

        public virtual async Task<string> CancelOrderAsync(IBinanceApiUser user, string symbol, long orderId, string newClientOrderId = null, long recvWindow = default, CancellationToken token = default)
        {
            if (orderId < 0)
                throw new ArgumentException("ID must not be less than 0.", nameof(orderId));

            // Cancel order using order ID.
            var json = await HttpClient.CancelOrderAsync(user, symbol, orderId, null, newClientOrderId, recvWindow, token)
                .ConfigureAwait(false);

            try { return JObject.Parse(json)["clientOrderId"].Value<string>(); }
            catch (Exception e)
            {
                throw NewFailedToParseJsonException(nameof(CancelOrderAsync), json, e);
            }
        }

        public virtual async Task<string> CancelOrderAsync(IBinanceApiUser user, string symbol, string origClientOrderId, string newClientOrderId = null, long recvWindow = default, CancellationToken token = default)
        {
            Throw.IfNullOrWhiteSpace(origClientOrderId, nameof(origClientOrderId));

            // Cancel order using original client order ID.
            var json = await HttpClient
                .CancelOrderAsync(user, symbol, NullId, origClientOrderId, newClientOrderId, recvWindow, token)
                .ConfigureAwait(false);

            try { return JObject.Parse(json)["clientOrderId"].Value<string>(); }
            catch (Exception e)
            {
                throw NewFailedToParseJsonException(nameof(CancelOrderAsync), json, e);
            }
        }

        public virtual async Task<IEnumerable<Order>> GetOpenOrdersAsync(IBinanceApiUser user, string symbol = null, long recvWindow = default, CancellationToken token = default)
        {
            var json = await HttpClient.GetOpenOrdersAsync(user, symbol, recvWindow, token)
                .ConfigureAwait(false);

            try { return _orderSerializer.DeserializeMany(json, user); }
            catch (Exception e)
            {
                throw NewFailedToParseJsonException(nameof(GetOpenOrdersAsync), json, e);
            }
        }

        public virtual async Task<IEnumerable<Order>> GetOrdersAsync(IBinanceApiUser user, string symbol, long orderId = NullId, int limit = default, long recvWindow = default, CancellationToken token = default)
        {
            var json = await HttpClient.GetOrdersAsync(user, symbol, orderId, limit, recvWindow, token)
                .ConfigureAwait(false);

            try { return _orderSerializer.DeserializeMany(json, user); }
            catch (Exception e)
            {
                throw NewFailedToParseJsonException(nameof(GetOrdersAsync), json, e);
            }
        }

        public virtual async Task<AccountInfo> GetAccountInfoAsync(IBinanceApiUser user, long recvWindow = default, CancellationToken token = default)
        {
            var json = await HttpClient.GetAccountInfoAsync(user, recvWindow, token)
                .ConfigureAwait(false);

            try
            {
                var jObject = JObject.Parse(json);

                var commissions = new AccountCommissions(
                    jObject["makerCommission"].Value<int>(),
                    jObject["takerCommission"].Value<int>(),
                    jObject["buyerCommission"].Value<int>(),
                    jObject["sellerCommission"].Value<int>());

                var status = new AccountStatus(
                    jObject["canTrade"].Value<bool>(),
                    jObject["canWithdraw"].Value<bool>(),
                    jObject["canDeposit"].Value<bool>());

                var balances = jObject["balances"]
                    .Select(entry => new AccountBalance(
                        entry["asset"].Value<string>(),
                        entry["free"].Value<decimal>(),
                        entry["locked"].Value<decimal>()))
                    .ToArray();

                return new AccountInfo(user, commissions, status, jObject["updateTime"].Value<long>().ToDateTime(), balances);
            }
            catch (Exception e)
            {
                throw NewFailedToParseJsonException(nameof(GetAccountInfoAsync), json, e);
            }
        }

        public virtual async Task<IEnumerable<AccountTrade>> GetAccountTradesAsync(IBinanceApiUser user, string symbol, long fromId = NullId, int limit = default, long recvWindow = default, CancellationToken token = default)
        {
            var json = await HttpClient.GetAccountTradesAsync(user, symbol, fromId, limit, recvWindow, token)
                .ConfigureAwait(false);

            try { return _accountTradeSerializer.DeserializeMany(json, symbol); }
            catch (Exception e)
            {
                throw NewFailedToParseJsonException(nameof(GetAccountTradesAsync), json, e);
            }
        }

        public virtual async Task<string> WithdrawAsync(WithdrawRequest withdrawRequest, long recvWindow = default, CancellationToken token = default)
        {
            Throw.IfNull(withdrawRequest, nameof(withdrawRequest));

            var json = await HttpClient.WithdrawAsync(withdrawRequest.User, withdrawRequest.Asset, withdrawRequest.Address, withdrawRequest.AddressTag, withdrawRequest.Amount, withdrawRequest.Name, recvWindow, token)
                .ConfigureAwait(false);

            bool success;

            try
            {
                var jObject = JObject.Parse(json);

                success = jObject["success"].Value<bool>();
                withdrawRequest.Message = jObject["msg"]?.Value<string>();
                withdrawRequest.Id = jObject["id"]?.Value<string>();
            }
            catch (Exception e)
            {
                throw NewFailedToParseJsonException(nameof(WithdrawAsync), json, e);
            }

            // ReSharper disable once InvertIf
            if (!success)
            {
                throw NewBinanceWApiException(nameof(WithdrawAsync), json, withdrawRequest.Asset);
            }

            return withdrawRequest.Id;
        }

        public async Task<IEnumerable<Deposit>> GetDepositsAsync(IBinanceApiUser user, string asset = null, DepositStatus? status = null, DateTime startTime = default, DateTime endTime = default, long recvWindow = 0, CancellationToken token = default)
        {
            var json = await HttpClient.GetDepositsAsync(user, asset, status, startTime, endTime, recvWindow, token)
                .ConfigureAwait(false);

            bool success;
            var deposits = new List<Deposit>();

            try
            {
                var jObject = JObject.Parse(json);

                success = jObject["success"].Value<bool>();

                if (success)
                {
                    var depositList = jObject["depositList"];

                    if (depositList != null)
                    {
                        deposits.AddRange(
                            depositList.Select(jToken => new Deposit(
                                jToken["asset"].Value<string>(),
                                jToken["amount"].Value<decimal>(),
                                jToken["insertTime"].Value<long>().ToDateTime(),
                                (DepositStatus)jToken["status"].Value<int>(),
                                jToken["address"]?.Value<string>(),
                                jToken["addressTag"]?.Value<string>(),
                                jToken["txId"]?.Value<string>())));
                    }
                }
            }
            catch (Exception e)
            {
                throw NewFailedToParseJsonException(nameof(GetDepositsAsync), json, e);
            }

            // ReSharper disable once InvertIf
            if (!success)
            {
                throw NewBinanceWApiException(nameof(GetDepositsAsync), json, asset);
            }

            return deposits;
        }

        public virtual async Task<IEnumerable<Withdrawal>> GetWithdrawalsAsync(IBinanceApiUser user, string asset = null, WithdrawalStatus? status = null, DateTime startTime = default, DateTime endTime = default, long recvWindow = default, CancellationToken token = default)
        {
            var json = await HttpClient.GetWithdrawalsAsync(user, asset, status, startTime, endTime, recvWindow, token)
                .ConfigureAwait(false);

            bool success;
            var withdrawals = new List<Withdrawal>();

            try
            {
                var jObject = JObject.Parse(json);

                success = jObject["success"].Value<bool>();

                if (success)
                {
                    var withdrawList = jObject["withdrawList"];

                    if (withdrawList != null)
                    {
                        withdrawals.AddRange(
                            withdrawList.Select(jToken => new Withdrawal(
                                jToken["id"].Value<string>(),
                                jToken["asset"].Value<string>(),
                                jToken["amount"].Value<decimal>(),
                                jToken["applyTime"].Value<long>().ToDateTime(),
                                (WithdrawalStatus)jToken["status"].Value<int>(),
                                jToken["address"].Value<string>(),
                                jToken["addressTag"]?.Value<string>(),
                                jToken["txId"]?.Value<string>())));
                    }
                }
            }
            catch (Exception e)
            {
                throw NewFailedToParseJsonException(nameof(GetWithdrawalsAsync), json, e);
            }

            // ReSharper disable once InvertIf
            if (!success)
            {
                throw NewBinanceWApiException(nameof(GetWithdrawalsAsync), json, asset);
            }

            return withdrawals;
        }

        public virtual async Task<DepositAddress> GetDepositAddressAsync(IBinanceApiUser user, string asset, CancellationToken token = default)
        {
            var json = await HttpClient.GetDepositAddressAsync(user, asset, token)
                .ConfigureAwait(false);

            bool success;
            DepositAddress depositAddress = null;

            try
            {
                var jObject = JObject.Parse(json);

                success = jObject["success"].Value<bool>();

                if (success)
                {
                    depositAddress = new DepositAddress(
                        asset,
                        jObject["address"].Value<string>(),
                        jObject["addressTag"]?.Value<string>());
                }
            }
            catch (Exception e)
            {
                throw NewFailedToParseJsonException(nameof(GetDepositAddressAsync), json, e);
            }

            // ReSharper disable once InvertIf
            if (!success)
            {
                throw NewBinanceWApiException(nameof(GetDepositAddressAsync), json, asset);
            }

            return depositAddress;
        }

        public virtual async Task<string> GetAccountStatusAsync(IBinanceApiUser user, CancellationToken token = default)
        {
            var json = await HttpClient.GetAccountStatusAsync(user, token)
                .ConfigureAwait(false);

            try { return JObject.Parse(json)["msg"].Value<string>(); }
            catch (Exception e)
            {
                throw NewFailedToParseJsonException(nameof(GetAccountStatusAsync), json, e);
            }
        }

        #endregion Account

        #region User Data Stream

        public async Task<string> UserStreamStartAsync(string apiKey, CancellationToken token = default)
        {
            var json = await HttpClient.UserStreamStartAsync(apiKey, token)
                .ConfigureAwait(false);

            try
            {
                return JObject.Parse(json)["listenKey"].Value<string>();
            }
            catch (Exception e)
            {
                throw NewFailedToParseJsonException(nameof(UserStreamStartAsync), json, e);
            }
        }

        public async Task UserStreamKeepAliveAsync(string apiKey, string listenKey, CancellationToken token = default)
        {
            var json = await HttpClient.UserStreamKeepAliveAsync(apiKey, listenKey, token)
                .ConfigureAwait(false);

            if (json != BinanceHttpClient.SuccessfulTestResponse)
            {
                var message = $"{nameof(BinanceApi)}.{nameof(UserStreamKeepAliveAsync)} failed.";
                _logger?.LogError(message);
                throw new BinanceApiException(message);
            }
        }

        public async Task UserStreamCloseAsync(string apiKey, string listenKey, CancellationToken token = default)
        {
            var json = await HttpClient.UserStreamCloseAsync(apiKey, listenKey, token)
                .ConfigureAwait(false);

            if (json != BinanceHttpClient.SuccessfulTestResponse)
            {
                var message = $"{nameof(BinanceApi)}.{nameof(UserStreamCloseAsync)} failed.";
                throw new BinanceApiException(message);
            }
        }

        #endregion User Data Stream

        #region Private Methods

        /// <summary>
        /// Throw exception when JSON parsing fails.
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="json"></param>
        /// <param name="e"></param>
        /// <returns></returns>
        private BinanceApiException NewFailedToParseJsonException(string methodName, string json, Exception e)
        {
            var message = $"{nameof(BinanceApi)}.{methodName} failed to parse JSON api response: \"{json}\"";

            _logger?.LogError(e, message);

            return new BinanceApiException(message, e);
        }

        /// <summary>
        /// Throw WAPI exception.
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="json"></param>
        /// <param name="asset"></param>
        private BinanceApiException NewBinanceWApiException(string methodName, string json, string asset)
        {
            asset = asset.FormatSymbol();

            var errorCode = 0;
            var errorMessage = "[NO MSG]";

            var jObject = JObject.Parse(json);

            var error = jObject["msg"].Value<string>();

            if (!string.IsNullOrWhiteSpace(error) && error.IsJsonObject())
            {
                try // to parse server error response.
                {
                    var jError = JObject.Parse(error);

                    errorCode = jError["code"]?.Value<int>() ?? 0;
                    errorMessage = jError["msg"]?.Value<string>();
                }
                catch (Exception e)
                {
                    _logger?.LogError(e, $"{nameof(BinanceApi)}.{methodName} failed to parse server error response: \"{error}\"");
                    throw;
                }
            }

            var message = $"{nameof(BinanceApi)}.{methodName}: Failed (asset: \"{asset}\") - \"{errorMessage}\"{(errorCode != 0 ? $" ({errorCode})" : " [NO CODE]")}";
            _logger?.LogError(message);
            return new BinanceApiException(message);
        }

        #endregion Private Methods
    }
}
