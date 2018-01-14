using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Binance.Account;
using Binance.Account.Orders;
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
        /// <param name="logger"></param>
        public BinanceApi(
            IBinanceHttpClient client,
            IOrderBookSerializer orderBookSerializer = null,
            ITradeSerializer tradeSerializer = null,
            IOrderSerializer orderSerializer = null,
            ILogger<BinanceApi> logger = null)
        {
            Throw.IfNull(client, nameof(client));

            HttpClient = client;

            _orderBookSerializer = orderBookSerializer ?? new OrderBookSerializer();
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

        #endregion Connectivity

        #region Market Data

        public virtual async Task<OrderBook> GetOrderBookAsync(string symbol, int limit = default, CancellationToken token = default)
        {
            var json = await HttpClient.GetOrderBookAsync(symbol, limit, token)
                .ConfigureAwait(false);

            try
            {
                return _orderBookSerializer.Deserialize(OrderBookJsonConverter.InsertSymbol(json, symbol));
            }
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
            var json = await HttpClient.GetAggregateTradesAsync(symbol, NullId, limit, 0, 0, token)
                .ConfigureAwait(false);

            try { return DeserializeAggregateTrades(symbol, json); }
            catch (Exception e)
            {
                throw NewFailedToParseJsonException(nameof(GetAggregateTradesAsync), json, e);
            }
        }

        public virtual async Task<IEnumerable<AggregateTrade>> GetAggregateTradesFromAsync(string symbol, long fromId, int limit = default, CancellationToken token = default)
        {
            if (fromId < 0)
                throw new ArgumentException($"ID ({nameof(fromId)}) must not be less than 0.", nameof(fromId));

            var json = await HttpClient.GetAggregateTradesAsync(symbol, fromId, limit, 0, 0, token)
                .ConfigureAwait(false);

            try { return DeserializeAggregateTrades(symbol, json); }
            catch (Exception e)
            {
                throw NewFailedToParseJsonException(nameof(GetAggregateTradesFromAsync), json, e);
            }
        }

        public virtual async Task<IEnumerable<AggregateTrade>> GetAggregateTradesInAsync(string symbol, long startTime, long endTime, CancellationToken token = default)
        {
            if (startTime <= 0)
                throw new ArgumentException($"Timestamp ({nameof(startTime)}) must be greater than 0.", nameof(startTime));
            if (endTime < startTime)
                throw new ArgumentException($"Timestamp ({nameof(endTime)}) must not be less than {nameof(startTime)} ({startTime}).", nameof(endTime));

            // NOTE: Limit does not apply when using start and end time.
            var json = await HttpClient.GetAggregateTradesAsync(symbol, NullId, default, startTime, endTime, token)
                .ConfigureAwait(false);

            try { return DeserializeAggregateTrades(symbol, json); }
            catch (Exception e)
            {
                throw NewFailedToParseJsonException(nameof(GetAggregateTradesInAsync), json, e);
            }
        }

        public virtual async Task<IEnumerable<Candlestick>> GetCandlesticksAsync(string symbol, CandlestickInterval interval, int limit = default, long startTime = default, long endTime = default, CancellationToken token = default)
        {
            var json = await HttpClient.GetCandlesticksAsync(symbol, interval, limit, startTime, endTime, token)
                .ConfigureAwait(false);

            try
            {
                return DeserializeCandlesticks(symbol, interval, json);
            }
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

            try
            {
                return ConvertTo24HourStatistics(JObject.Parse(json));
            }
            catch (Exception e)
            {
                throw NewFailedToParseJsonException(nameof(Get24HourStatisticsAsync), json, e);
            }
        }

        public virtual async Task<IEnumerable<SymbolStatistics>> Get24HourStatisticsAsync(CancellationToken token = default)
        {
            var json = await HttpClient.Get24HourStatisticsAsync(token: token)
                .ConfigureAwait(false);

            try
            {
                return JArray.Parse(json)
                    .Select(ConvertTo24HourStatistics)
                    .ToArray();
            }
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

            try
            {
                return ConvertToSymbolPrice(JObject.Parse(json));
            }
            catch (Exception e)
            {
                throw NewFailedToParseJsonException(nameof(GetPricesAsync), json, e);
            }
        }

        public virtual async Task<IEnumerable<SymbolPrice>> GetPricesAsync(CancellationToken token = default)
        {
            var json = await HttpClient.GetPriceAsync(token: token)
                .ConfigureAwait(false);

            try
            {
                return JArray.Parse(json)
                    .Select(ConvertToSymbolPrice)
                    .Where(_ => _.Symbol != "123456" && _.Symbol != "ETC") // HACK
                    .ToArray();
            }
            catch (Exception e)
            {
                throw NewFailedToParseJsonException(nameof(GetPricesAsync), json, e);
            }
        }

        public virtual async Task<OrderBookTop> GetOrderBookTopAsync(string symbol, CancellationToken token = default)
        {
            var json = await HttpClient.GetOrderBookTopAsync(symbol, token)
                .ConfigureAwait(false);

            try
            {
                return ConvertToOrderBookTop(JObject.Parse(json));
            }
            catch (Exception e)
            {
                throw NewFailedToParseJsonException(nameof(GetOrderBookTopAsync), json, e);
            }
        }

        public virtual async Task<IEnumerable<OrderBookTop>> GetOrderBookTopsAsync(CancellationToken token = default)
        {
            var json = await HttpClient.GetOrderBookTopsAsync(token)
                .ConfigureAwait(false);

            try
            {
                return JArray.Parse(json).Select(ConvertToOrderBookTop).ToArray();
            }
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
                stopOrder?.StopPrice ?? 0, limitOrder?.IcebergQuantity ?? 0, recvWindow, false, PlaceOrderResponseType.Result, token);

            try
            {
                // Update existing order properties.
                _orderSerializer.Deserialize(json, order);

                // Update client order properties.
                clientOrder.Id = order.ClientOrderId;
                clientOrder.Timestamp = order.Timestamp;
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
                stopOrder?.StopPrice ?? 0, limitOrder?.IcebergQuantity ?? 0, recvWindow, true, token: token);

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

                return new AccountInfo(user, commissions, status, jObject["updateTime"].Value<long>(), balances);
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

            try
            {
                var jArray = JArray.Parse(json);

                return jArray
                    .Select(jToken => new AccountTrade(
                        symbol.FormatSymbol(),
                        jToken["id"].Value<long>(),
                        jToken["orderId"].Value<long>(),
                        jToken["price"].Value<decimal>(),
                        jToken["qty"].Value<decimal>(),
                        jToken["commission"].Value<decimal>(),
                        jToken["commissionAsset"].Value<string>(),
                        jToken["time"].Value<long>(),
                        jToken["isBuyer"].Value<bool>(),
                        jToken["isMaker"].Value<bool>(),
                        jToken["isBestMatch"].Value<bool>()))
                    .ToArray();
            }
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
            string msg;
            string id;

            try
            {
                var jObject = JObject.Parse(json);

                success = jObject["success"].Value<bool>();
                msg = jObject["msg"]?.Value<string>();
                id = jObject["id"]?.Value<string>();
            }
            catch (Exception e)
            {
                throw NewFailedToParseJsonException(nameof(WithdrawAsync), json, e);
            }

            // ReSharper disable once InvertIf
            if (!success)
            {
                var message = $"{nameof(BinanceApi)}.{nameof(WithdrawAsync)} failed: \"{msg ?? "[No Message]"}\"";
                _logger?.LogError(message);
                throw new BinanceApiException(message);
            }

            return id;
        }

        public virtual async Task<IEnumerable<Deposit>> GetDepositsAsync(IBinanceApiUser user, string asset, DepositStatus? status = null, long startTime = default, long endTime = default, long recvWindow = default, CancellationToken token = default)
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
                                jToken["insertTime"].Value<long>(),
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
                var message = $"{nameof(BinanceApi)}.{nameof(GetDepositsAsync)} unsuccessful.";
                _logger?.LogError(message);
                throw new BinanceApiException(message);
            }

            return deposits;
        }

        public virtual async Task<IEnumerable<Withdrawal>> GetWithdrawalsAsync(IBinanceApiUser user, string asset, WithdrawalStatus? status = null, long startTime = default, long endTime = default, long recvWindow = default, CancellationToken token = default)
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
                                jToken["applyTime"].Value<long>(),
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
                var message = $"{nameof(BinanceApi)}.{nameof(GetWithdrawalsAsync)} unsuccessful.";
                _logger?.LogError(message);
                throw new BinanceApiException(message);
            }

            return withdrawals;
        }

        public virtual async Task<DepositAddress> GetDepositAddressAsync(IBinanceApiUser user, string asset, CancellationToken token = default)
        {
            var json = await HttpClient.GetDepositAddressAsync(user, asset, token)
                .ConfigureAwait(false);

            try
            {
                var jObject = JObject.Parse(json);

                return new DepositAddress(
                    jObject["asset"].Value<string>(),
                    jObject["address"].Value<string>(),
                    jObject["addressTag"]?.Value<string>());
            }
            catch (Exception e)
            {
                throw NewFailedToParseJsonException(nameof(GetDepositAddressAsync), json, e);
            }
        }

        public virtual async Task<string> GetAccountStatusAsync(IBinanceApiUser user, CancellationToken token = default)
        {
            var json = await HttpClient.GetAccountStatusAsync(user, token)
                .ConfigureAwait(false);

            try
            {
                return JObject.Parse(json)["msg"].Value<string>();
            }
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
        /// Deserialize aggregate trades.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="json"></param>
        /// <returns></returns>
        private static IEnumerable<AggregateTrade> DeserializeAggregateTrades(string symbol, string json)
        {
            var jArray = JArray.Parse(json);

            return jArray.Select(item => new AggregateTrade(
                    symbol.FormatSymbol(),
                    item["a"].Value<long>(), // ID
                    item["p"].Value<decimal>(), // price
                    item["q"].Value<decimal>(), // quantity
                    item["f"].Value<long>(), // first trade ID
                    item["l"].Value<long>(), // last trade ID
                    item["T"].Value<long>(), // timestamp
                    item["m"].Value<bool>(), // is buyer maker
                    item["M"].Value<bool>())) // is best price match
                .ToArray();
        }

        /// <summary>
        /// Deserialize candlesticks.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="interval"></param>
        /// <param name="json"></param>
        /// <returns></returns>
        private static IEnumerable<Candlestick> DeserializeCandlesticks(string symbol, CandlestickInterval interval, string json)
        {
            var jArray = JArray.Parse(json);

            return jArray.Select(item => new Candlestick(
                symbol.FormatSymbol(), // symbol
                interval, // interval
                item[0].Value<long>(), // open time
                item[1].Value<decimal>(), // open
                item[2].Value<decimal>(), // high
                item[3].Value<decimal>(), // low
                item[4].Value<decimal>(), // close
                item[5].Value<decimal>(), // volume
                item[6].Value<long>(), // close time
                item[7].Value<decimal>(), // quote asset volume
                item[8].Value<long>(), // number of trades
                item[9].Value<decimal>(), // taker buy base asset volume
                item[10].Value<decimal>() // taker buy quote asset volume
            )).ToArray();
        }

        /// <summary>
        /// Convert to order book top.
        /// </summary>
        /// <param name="jToken"></param>
        /// <returns></returns>
        private OrderBookTop ConvertToOrderBookTop(JToken jToken)
        {
            return OrderBookTop.Create(
                jToken["symbol"].Value<string>(),
                jToken["bidPrice"].Value<decimal>(),
                jToken["bidQty"].Value<decimal>(),
                jToken["askPrice"].Value<decimal>(),
                jToken["askQty"].Value<decimal>());
        }

        /// <summary>
        /// Convert to 24-hour statistics.
        /// </summary>
        /// <param name="jToken"></param>
        /// <returns></returns>
        private SymbolStatistics ConvertTo24HourStatistics(JToken jToken)
        {
            return new SymbolStatistics(
                jToken["symbol"].Value<string>(),
                TimeSpan.FromHours(24),
                jToken["priceChange"].Value<decimal>(),
                jToken["priceChangePercent"].Value<decimal>(),
                jToken["weightedAvgPrice"].Value<decimal>(),
                jToken["prevClosePrice"].Value<decimal>(),
                jToken["lastPrice"].Value<decimal>(),
                jToken["lastQty"].Value<decimal>(),
                jToken["bidPrice"].Value<decimal>(),
                jToken["bidQty"].Value<decimal>(),
                jToken["askPrice"].Value<decimal>(),
                jToken["askQty"].Value<decimal>(),
                jToken["openPrice"].Value<decimal>(),
                jToken["highPrice"].Value<decimal>(),
                jToken["lowPrice"].Value<decimal>(),
                jToken["volume"].Value<decimal>(),
                jToken["quoteVolume"].Value<decimal>(),
                jToken["openTime"].Value<long>(),
                jToken["closeTime"].Value<long>(),
                jToken["firstId"].Value<long>(),
                jToken["lastId"].Value<long>(),
                jToken["count"].Value<long>());
        }

        /// <summary>
        /// Convert to symbol price.
        /// </summary>
        /// <param name="jToken"></param>
        /// <returns></returns>
        private SymbolPrice ConvertToSymbolPrice(JToken jToken)
        {
            return new SymbolPrice(
                jToken["symbol"].Value<string>(),
                jToken["price"].Value<decimal>());
        }

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

        #endregion Private Methods
    }
}
