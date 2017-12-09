using System;
using System.Threading;
using System.Threading.Tasks;
using Binance.Account;
using Binance.Account.Orders;
using Binance.Market;
using Newtonsoft.Json.Linq;

// ReSharper disable once CheckNamespace
namespace Binance.Api
{
    public static class BinanceHttpClientExtensions
    {
        #region Private Fields

        private static DateTime _timestampOffsetUpdatedAt;

        private static readonly SemaphoreSlim _timestampOffsetSync = new SemaphoreSlim(1, 1);

        #endregion Private Fields

        #region Internal Fields

        internal static long _timestampOffset;

        #endregion Internal Fields

        /// <summary>
        /// Test connectivity to the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task<string> PingAsync(this IBinanceHttpClient client, CancellationToken token = default)
        {
            Throw.IfNull(client, nameof(client));

            return client.GetAsync("/api/v1/ping", token);
        }

        /// <summary>
        /// Get exchange information.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task<string> GetExchangeInfoAsync(this IBinanceHttpClient client, CancellationToken token = default)
        {
            Throw.IfNull(client, nameof(client));

            return client.GetAsync("/api/v1/exchangeInfo", token);
        }

        #region Time

        /// <summary>
        /// Test connectivity to the server and get the current time.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task<string> GetServerTimeAsync(this IBinanceHttpClient client, CancellationToken token = default)
        {
            Throw.IfNull(client, nameof(client));

            return client.GetAsync("/api/v1/time", token);
        }

        /// <summary>
        /// Get local system timestamp synchronized with server time.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<long> GetTimestampAsync(this IBinanceHttpClient client, CancellationToken token = default)
        {
            Throw.IfNull(client, nameof(client));

            // Acquire synchronization lock.
            await _timestampOffsetSync.WaitAsync(token)
                .ConfigureAwait(false);

            try
            {
                if (DateTime.UtcNow - _timestampOffsetUpdatedAt > TimeSpan.FromMinutes(client.Options.TimestampOffsetRefreshPeriodMinutes))
                {
                    const long N = 3;

                    long sum = 0;
                    var count = N;
                    do
                    {
                        var systemTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                        var json = await GetServerTimeAsync(client, token)
                            .ConfigureAwait(false);

                        systemTime = (systemTime + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()) / 2;

                        // Calculate timestamp offset to account for time differences and delays.
                        sum += JObject.Parse(json)["serverTime"].Value<long>() - systemTime;
                    } while (--count > 0);

                    // Calculate average offset.
                    _timestampOffset = sum / N;

                    // Record the current system time to determine when to refresh offset.
                    _timestampOffsetUpdatedAt = DateTime.UtcNow;
                }
            }
            catch (Exception) { /* ignore */ }
            finally
            {
                // Release synchronization lock.
                _timestampOffsetSync.Release();
            }

            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + _timestampOffset;
        }

        #endregion Time

        #region Market Data

        /// <summary>
        /// Get order book (market depth) of a symbol.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="symbol"></param>
        /// <param name="limit">Default 100; max 1000. Valid limits:[5, 10, 20, 50, 100, 500, 1000].</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task<string> GetOrderBookAsync(this IBinanceHttpClient client, string symbol, int limit = default, CancellationToken token = default)
        {
            Throw.IfNull(client, nameof(client));
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            var request = new BinanceHttpRequest("/api/v1/depth", limit >= 1000 ? 50 : limit >= 500 ? 25 : 1);

            request.AddParameter("symbol", symbol.FormatSymbol());

            if (limit > 0)
            {
                request.AddParameter("limit", limit);
            }

            return client.GetAsync(request, token);
        }

        /// <summary>
        /// Get latest (non-compressed) trades.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="symbol"></param>
        /// <param name="limit"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task<string> GetTradesAsync(this IBinanceHttpClient client, string symbol, int limit = default, CancellationToken token = default)
        {
            Throw.IfNull(client, nameof(client));
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            var request = new BinanceHttpRequest("/api/v1/trades");

            request.AddParameter("symbol", symbol.FormatSymbol());

            if (limit > 0)
                request.AddParameter("limit", limit);

            return client.GetAsync(request, token);
        }

        /// <summary>
        /// Get older (non-compressed) trades.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="apiKey"></param>
        /// <param name="symbol"></param>
        /// <param name="fromId">TradeId to fetch from. Default gets most recent trades.</param>
        /// <param name="limit">Default 500; max 500.</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task<string> GetTradesAsync(this IBinanceHttpClient client, string apiKey, string symbol, long fromId = BinanceApi.NullId, int limit = default, CancellationToken token = default)
        {
            Throw.IfNull(client, nameof(client));
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            var request = new BinanceHttpRequest("/api/v1/historicalTrades", 100)
            {
                ApiKey = apiKey
            };

            request.AddParameter("symbol", symbol.FormatSymbol());

            if (fromId >= 0)
                request.AddParameter("fromId", fromId);

            if (limit > 0)
                request.AddParameter("limit", limit);

            return client.GetAsync(request, token);
        }

        /// <summary>
        /// Get compressed, aggregate trades. Trades that fill at the time, from the same order, with the same price will have the quantity aggregated.
        /// If fromdId, startTime, and endTime are not sent, the most recent aggregate trades will be returned.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="symbol"></param>
        /// <param name="fromId">ID to get aggregate trades from INCLUSIVE.</param>
        /// <param name="limit">Default 500; max 500.</param>
        /// <param name="startTime">Timestamp in ms to get aggregate trades from INCLUSIVE.</param>
        /// <param name="endTime">Timestamp in ms to get aggregate trades until INCLUSIVE.</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task<string> GetAggregateTradesAsync(this IBinanceHttpClient client, string symbol, long fromId = BinanceApi.NullId, int limit = default, long startTime = default, long endTime = default, CancellationToken token = default)
        {
            Throw.IfNull(client, nameof(client));
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            var request = new BinanceHttpRequest("/api/v1/aggTrades");

            request.AddParameter("symbol", symbol.FormatSymbol());

            if (fromId >= 0)
                request.AddParameter("fromId", fromId);

            if (startTime > 0)
                request.AddParameter("startTime", startTime);

            if (endTime > 0)
                request.AddParameter("endTime", endTime);

            if (startTime <= 0 || endTime <= 0)
            {
                if (limit > 0)
                    request.AddParameter("limit", limit);
            }
            else
            {
                var start = DateTimeOffset.FromUnixTimeMilliseconds(startTime);
                var end = DateTimeOffset.FromUnixTimeMilliseconds(endTime);

                if ((end - start).Duration() >= TimeSpan.FromHours(24))
                    throw new ArgumentException($"The interval between {nameof(startTime)} and {nameof(endTime)} must be less than 24 hours.", nameof(endTime));
            }

            return client.GetAsync(request, token);
        }

        /// <summary>
        /// Get candlesticks for a symbol. Candlesticks/K-Lines are uniquely identified by their open time.
        /// If startTime and endTime are not sent, the most recent candlesticks are returned.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="symbol"></param>
        /// <param name="interval"></param>
        /// <param name="limit">Default 500; max 500.</param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task<string> GetCandlesticksAsync(this IBinanceHttpClient client, string symbol, CandlestickInterval interval, int limit = default, long startTime = default, long endTime = default, CancellationToken token = default)
        {
            Throw.IfNull(client, nameof(client));
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            var request = new BinanceHttpRequest("/api/v1/klines");

            request.AddParameter("symbol", symbol.FormatSymbol());
            request.AddParameter("interval", interval.AsString());

            if (limit > 0)
                request.AddParameter("limit", limit);

            if (startTime > 0)
                request.AddParameter("startTime", startTime);

            if (endTime > 0)
                request.AddParameter("endTime", endTime);

            return client.GetAsync(request, token);
        }

        /// <summary>
        /// Extension allowing candlestick interval as a string.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="symbol"></param>
        /// <param name="interval"></param>
        /// <param name="limit"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task<string> GetCandlesticksAsync(this IBinanceHttpClient client, string symbol, string interval, int limit = default, long startTime = default, long endTime = default, CancellationToken token = default)
        {
            Throw.IfNull(client, nameof(client));

            return client.GetCandlesticksAsync(symbol, interval.ToCandlestickInterval(), limit, startTime, endTime, token);
        }

        /// <summary>
        /// Get 24 hour price change statistics for a symbol.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="symbol"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task<string> Get24HourStatisticsAsync(this IBinanceHttpClient client, string symbol, CancellationToken token = default)
        {
            Throw.IfNull(client, nameof(client));

            // HACK: When symbol is not provided estimate number of symbols that are TRADING for weight.
            var request = new BinanceHttpRequest("/api/v1/ticker/24hr", string.IsNullOrWhiteSpace(symbol) ? 200 : 1);

            request.AddParameter("symbol", symbol.FormatSymbol());

            return client.GetAsync(request, token);
        }

        /// <summary>
        /// Get latest price for all symbols.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task<string> GetPricesAsync(this IBinanceHttpClient client, CancellationToken token = default)
        {
            Throw.IfNull(client, nameof(client));

            return client.GetAsync("/api/v1/ticker/allPrices", token);
        }

        /// <summary>
        /// Get best price/quantity on the order book for a symbol.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="symbol"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task<string> GetOrderBookTopAsync(this IBinanceHttpClient client, string symbol, CancellationToken token = default)
        {
            Throw.IfNull(client, nameof(client));
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            var request = new BinanceHttpRequest("/api/v1/ticker/bookTicker");

            request.AddParameter("symbol", symbol.FormatSymbol());

            return client.GetAsync(request, token);
        }

        /// <summary>
        /// Get best price/quantity on the order book for all symbols.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task<string> GetOrderBookTopsAsync(this IBinanceHttpClient client, CancellationToken token = default)
        {
            Throw.IfNull(client, nameof(client));

            return client.GetAsync("/api/v1/ticker/bookTicker", token);
        }

        #endregion Market Data

        #region Account

        /// <summary>
        /// Send in a new order.
        /// </summary>
        /// <param name="client"></param>
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
        public static async Task<string> PlaceOrderAsync(this IBinanceHttpClient client, IBinanceApiUser user, string symbol, OrderSide side, OrderType type, decimal quantity, decimal price, string newClientOrderId = null, TimeInForce? timeInForce = null, decimal stopPrice = 0, decimal icebergQty = 0, long recvWindow = default, bool isTestOnly = false, CancellationToken token = default)
        {
            Throw.IfNull(client, nameof(client));
            Throw.IfNull(user, nameof(user));
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            if (quantity <= 0)
                throw new ArgumentException("Order quantity must be greater than 0.", nameof(quantity));

            if (recvWindow <= 0)
                recvWindow = client.Options.RecvWindowDefault ?? 0;

            var request = new BinanceHttpRequest($"/api/v3/order{(isTestOnly ? "/test" : string.Empty)}")
            {
                ApiKey = user.ApiKey
            };

            request.AddParameter("symbol", symbol.FormatSymbol());
            request.AddParameter("side", side.ToString().ToUpper());
            request.AddParameter("type", type.ToString().ToUpper());
            request.AddParameter("quantity", quantity);

            if (price > 0)
                request.AddParameter("price", price);

            if (timeInForce.HasValue)
                request.AddParameter("timeInForce", timeInForce.ToString().ToUpper());

            if (!string.IsNullOrWhiteSpace(newClientOrderId))
                request.AddParameter("newClientOrderId", newClientOrderId);

            if (stopPrice > 0)
                request.AddParameter("stopPrice", stopPrice);

            if (icebergQty > 0)
            {
                // Automatically set time-in-force to GTC if not set.
                if (!timeInForce.HasValue)
                    timeInForce = TimeInForce.GTC;

                if (timeInForce != TimeInForce.GTC)
                    throw new BinanceApiException("Any order with an icebergQty MUST have timeInForce set to GTC.");

                request.AddParameter("icebergQty", icebergQty);
            }

            if (recvWindow > 0)
                request.AddParameter("recvWindow", recvWindow);

            var timestamp = await client.GetTimestampAsync(token)
                .ConfigureAwait(false);

            request.AddParameter("timestamp", timestamp);

            var signature = user.Sign(request.QueryString);

            request.AddParameter("signature", signature);

            return await client.PostAsync(request, token, user.RateLimiter)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Check an order's status. Either orderId or origClientOrderId must be sent.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="user"></param>
        /// <param name="symbol"></param>
        /// <param name="orderId"></param>
        /// <param name="origClientOrderId"></param>
        /// <param name="recvWindow"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<string> GetOrderAsync(this IBinanceHttpClient client, IBinanceApiUser user, string symbol, long orderId = BinanceApi.NullId, string origClientOrderId = null, long recvWindow = default, CancellationToken token = default)
        {
            Throw.IfNull(client, nameof(client));
            Throw.IfNull(user, nameof(user));
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            if (orderId < 0 && string.IsNullOrWhiteSpace(origClientOrderId))
                throw new ArgumentException($"Either '{nameof(orderId)}' or '{nameof(origClientOrderId)}' must be provided, but both were invalid.");

            if (recvWindow <= 0)
                recvWindow = client.Options.RecvWindowDefault ?? 0;

            var request = new BinanceHttpRequest($"/api/v3/order")
            {
                ApiKey = user.ApiKey
            };

            request.AddParameter("symbol", symbol.FormatSymbol());

            if (orderId >= 0)
                request.AddParameter("orderId", orderId);

            if (!string.IsNullOrWhiteSpace(origClientOrderId))
                request.AddParameter("origClientOrderId", origClientOrderId);

            if (recvWindow > 0)
                request.AddParameter("recvWindow", recvWindow);

            var timestamp = await client.GetTimestampAsync(token)
                .ConfigureAwait(false);

            request.AddParameter("timestamp", timestamp);

            var signature = user.Sign(request.QueryString);

            request.AddParameter("signature", signature);

            return await client.GetAsync(request, token, user.RateLimiter)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Cancel an active order.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="user"></param>
        /// <param name="symbol"></param>
        /// <param name="orderId"></param>
        /// <param name="origClientOrderId"></param>
        /// <param name="newClientOrderId">Used to uniquely identify this cancel. Automatically generated by default.</param>
        /// <param name="recvWindow"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<string> CancelOrderAsync(this IBinanceHttpClient client, IBinanceApiUser user, string symbol, long orderId = BinanceApi.NullId, string origClientOrderId = null, string newClientOrderId = null, long recvWindow = default, CancellationToken token = default)
        {
            Throw.IfNull(client, nameof(client));
            Throw.IfNull(user, nameof(user));
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            if (orderId < 0 && string.IsNullOrWhiteSpace(origClientOrderId))
                throw new ArgumentException($"Either '{nameof(orderId)}' or '{nameof(origClientOrderId)}' must be provided, but both were invalid.");

            if (recvWindow <= 0)
                recvWindow = client.Options.RecvWindowDefault ?? 0;

            var request = new BinanceHttpRequest($"/api/v3/order")
            {
                ApiKey = user.ApiKey
            };

            request.AddParameter("symbol", symbol.FormatSymbol());

            if (orderId >= 0)
                request.AddParameter("orderId", orderId);

            if (!string.IsNullOrWhiteSpace(origClientOrderId))
                request.AddParameter("origClientOrderId", origClientOrderId);

            if (!string.IsNullOrWhiteSpace(newClientOrderId))
                request.AddParameter("newClientOrderId", newClientOrderId);

            if (recvWindow > 0)
                request.AddParameter("recvWindow", recvWindow);

            var timestamp = await client.GetTimestampAsync(token)
                .ConfigureAwait(false);

            request.AddParameter("timestamp", timestamp);

            var signature = user.Sign(request.QueryString);

            request.AddParameter("signature", signature);

            return await client.DeleteAsync(request, token, user.RateLimiter)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Get all open orders on a symbol.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="user"></param>
        /// <param name="symbol"></param>
        /// <param name="recvWindow"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<string> GetOpenOrdersAsync(this IBinanceHttpClient client, IBinanceApiUser user, string symbol, long recvWindow = default, CancellationToken token = default)
        {
            Throw.IfNull(client, nameof(client));
            Throw.IfNull(user, nameof(user));
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            if (recvWindow <= 0)
                recvWindow = client.Options.RecvWindowDefault ?? 0;

            var request = new BinanceHttpRequest($"/api/v3/openOrders")
            {
                ApiKey = user.ApiKey
            };

            request.AddParameter("symbol", symbol.FormatSymbol());

            if (recvWindow > 0)
                request.AddParameter("recvWindow", recvWindow);

            var timestamp = await client.GetTimestampAsync(token)
                .ConfigureAwait(false);

            request.AddParameter("timestamp", timestamp);

            var signature = user.Sign(request.QueryString);

            request.AddParameter("signature", signature);

            return await client.GetAsync(request, token)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Get all account orders; active, canceled, or filled.
        /// If orderId is set, this will return orders >= orderId; otherwise return most recent orders.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="user"></param>
        /// <param name="symbol"></param>
        /// <param name="orderId"></param>
        /// <param name="limit">Default 500; max 500.</param>
        /// <param name="recvWindow"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<string> GetOrdersAsync(this IBinanceHttpClient client, IBinanceApiUser user, string symbol, long orderId = BinanceApi.NullId, int limit = default, long recvWindow = default, CancellationToken token = default)
        {
            Throw.IfNull(client, nameof(client));
            Throw.IfNull(user, nameof(user));
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            if (recvWindow <= 0)
                recvWindow = client.Options.RecvWindowDefault ?? 0;

            var request = new BinanceHttpRequest($"/api/v3/allOrders")
            {
                ApiKey = user.ApiKey
            };

            request.AddParameter("symbol", symbol.FormatSymbol());

            if (orderId >= 0)
                request.AddParameter("orderId", orderId);

            if (limit > 0)
                request.AddParameter("limit", limit);

            if (recvWindow > 0)
                request.AddParameter("recvWindow", recvWindow);

            var timestamp = await client.GetTimestampAsync(token)
                .ConfigureAwait(false);

            request.AddParameter("timestamp", timestamp);

            var signature = user.Sign(request.QueryString);

            request.AddParameter("signature", signature);

            return await client.GetAsync(request, token)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Get current account information.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="user"></param>
        /// <param name="recvWindow"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<string> GetAccountInfoAsync(this IBinanceHttpClient client, IBinanceApiUser user, long recvWindow = default, CancellationToken token = default)
        {
            Throw.IfNull(client, nameof(client));
            Throw.IfNull(user, nameof(user));

            if (recvWindow <= 0)
                recvWindow = client.Options.RecvWindowDefault ?? 0;

            var request = new BinanceHttpRequest($"/api/v3/account")
            {
                ApiKey = user.ApiKey
            };

            var timestamp = await client.GetTimestampAsync(token)
                .ConfigureAwait(false);

            request.AddParameter("timestamp", timestamp);

            if (recvWindow > 0)
                request.AddParameter("recvWindow", recvWindow);

            var signature = user.Sign(request.QueryString);

            request.AddParameter("signature", signature);

            return await client.GetAsync(request, token)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Get trades for a specific account and symbol.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="user"></param>
        /// <param name="symbol"></param>
        /// <param name="fromId">TradeId to fetch from. Default gets most recent trades.</param>
        /// <param name="limit">Default 500; max 500.</param>
        /// <param name="recvWindow"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<string> GetAccountTradesAsync(this IBinanceHttpClient client, IBinanceApiUser user, string symbol, long fromId = BinanceApi.NullId, int limit = default, long recvWindow = default, CancellationToken token = default)
        {
            Throw.IfNull(client, nameof(client));
            Throw.IfNull(user, nameof(user));
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            if (recvWindow <= 0)
                recvWindow = client.Options.RecvWindowDefault ?? 0;

            var request = new BinanceHttpRequest($"/api/v3/myTrades")
            {
                ApiKey = user.ApiKey
            };

            request.AddParameter("symbol", symbol.FormatSymbol());

            if (fromId >= 0)
                request.AddParameter("fromId", fromId);

            if (limit > 0)
                request.AddParameter("limit", limit);

            if (recvWindow > 0)
                request.AddParameter("recvWindow", recvWindow);

            var timestamp = await client.GetTimestampAsync(token)
                .ConfigureAwait(false);

            request.AddParameter("timestamp", timestamp);

            var signature = user.Sign(request.QueryString);

            request.AddParameter("signature", signature);

            return await client.GetAsync(request, token)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Submit a withdraw request.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="user"></param>
        /// <param name="asset"></param>
        /// <param name="address"></param>
        /// <param name="amount"></param>
        /// <param name="name">A description of the address (optional).</param>
        /// <param name="recvWindow"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<string> WithdrawAsync(this IBinanceHttpClient client, IBinanceApiUser user, string asset, string address, decimal amount, string name = null, long recvWindow = default, CancellationToken token = default)
        {
            Throw.IfNull(client, nameof(client));
            Throw.IfNull(user, nameof(user));
            Throw.IfNullOrWhiteSpace(asset, nameof(asset));
            Throw.IfNullOrWhiteSpace(address, nameof(address));

            if (amount <= 0)
                throw new ArgumentException("Withdraw amount must be greater than 0.", nameof(amount));

            if (recvWindow <= 0)
                recvWindow = client.Options.RecvWindowDefault ?? 0;

            var request = new BinanceHttpRequest($"/wapi/v1/withdraw.html")
            {
                ApiKey = user.ApiKey
            };

            request.AddParameter("asset", asset.FormatSymbol());
            request.AddParameter("address", address);
            request.AddParameter("amount", amount);

            if (!string.IsNullOrWhiteSpace(name))
                request.AddParameter("name", name);

            if (recvWindow > 0)
                request.AddParameter("recvWindow", recvWindow);

            var timestamp = await client.GetTimestampAsync(token)
                .ConfigureAwait(false);

            request.AddParameter("timestamp", timestamp);

            var signature = user.Sign(request.QueryString);

            request.AddParameter("signature", signature);

            return await client.PostAsync(request, token)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Get the deposit history.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="user"></param>
        /// <param name="asset"></param>
        /// <param name="status"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="recvWindow"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<string> GetDepositsAsync(this IBinanceHttpClient client, IBinanceApiUser user, string asset = null, DepositStatus? status = null, long startTime = default, long endTime = default, long recvWindow = default, CancellationToken token = default)
        {
            Throw.IfNull(client, nameof(client));
            Throw.IfNull(user, nameof(user));

            if (recvWindow <= 0)
                recvWindow = client.Options.RecvWindowDefault ?? 0;

            var request = new BinanceHttpRequest($"/wapi/v1/getDepositHistory.html")
            {
                ApiKey = user.ApiKey
            };

            var timestamp = await client.GetTimestampAsync(token)
                .ConfigureAwait(false);

            request.AddParameter("timestamp", timestamp);

            if (!string.IsNullOrWhiteSpace(asset))
                request.AddParameter("asset", asset.FormatSymbol());

            if (status.HasValue)
                request.AddParameter("status", (int)status);

            if (startTime > 0)
                request.AddParameter("startTime", startTime);

            if (endTime > 0)
                request.AddParameter("endTime", endTime);

            if (recvWindow > 0)
                request.AddParameter("recvWindow", recvWindow);

            var signature = user.Sign(request.QueryString);

            request.AddParameter("signature", signature);

            return await client.PostAsync(request, token)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Get the withdrawal history.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="user"></param>
        /// <param name="asset"></param>
        /// <param name="status"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="recvWindow"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<string> GetWithdrawalsAsync(this IBinanceHttpClient client, IBinanceApiUser user, string asset = null, WithdrawalStatus? status = null, long startTime = default, long endTime = default, long recvWindow = default, CancellationToken token = default)
        {
            Throw.IfNull(client, nameof(client));
            Throw.IfNull(user, nameof(user));

            if (recvWindow <= 0)
                recvWindow = client.Options.RecvWindowDefault ?? 0;

            var request = new BinanceHttpRequest($"/wapi/v1/getWithdrawHistory.html")
            {
                ApiKey = user.ApiKey
            };

            var timestamp = await client.GetTimestampAsync(token)
                .ConfigureAwait(false);

            request.AddParameter("timestamp", timestamp);

            if (!string.IsNullOrWhiteSpace(asset))
                request.AddParameter("asset", asset.FormatSymbol());

            if (status.HasValue)
                request.AddParameter("status", (int)status);

            if (startTime > 0)
                request.AddParameter("startTime", startTime);

            if (endTime > 0)
                request.AddParameter("endTime", endTime);

            if (recvWindow > 0)
                request.AddParameter("recvWindow", recvWindow);

            var signature = user.Sign(request.QueryString);

            return await client.PostAsync(request, token)
                .ConfigureAwait(false);
        }

        #endregion Account

        #region User Stream

        /// <summary>
        /// Start a new user data stream.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="user"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task<string> UserStreamStartAsync(this IBinanceHttpClient client, IBinanceApiUser user, CancellationToken token = default)
        {
            Throw.IfNull(user, nameof(user));

            return UserStreamStartAsync(client, user.ApiKey, token);
        }

        /// <summary>
        /// Start a new user data stream.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="apiKey"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task<string> UserStreamStartAsync(this IBinanceHttpClient client, string apiKey, CancellationToken token = default)
        {
            Throw.IfNullOrWhiteSpace(apiKey, nameof(apiKey));

            var request = new BinanceHttpRequest("/api/v1/userDataStream")
            {
                ApiKey = apiKey
            };

            return client.PostAsync(request, token);
        }

        /// <summary>
        /// Ping a user data stream to prevent a time out.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="user"></param>
        /// <param name="listenKey"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task<string> UserStreamKeepAliveAsync(this IBinanceHttpClient client, IBinanceApiUser user, string listenKey, CancellationToken token = default)
        {
            Throw.IfNull(user, nameof(user));

            return UserStreamKeepAliveAsync(client, user.ApiKey, listenKey, token);
        }

        /// <summary>
        /// Ping a user data stream to prevent a time out.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="apiKey"></param>
        /// <param name="listenKey"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task<string> UserStreamKeepAliveAsync(this IBinanceHttpClient client, string apiKey, string listenKey, CancellationToken token = default)
        {
            Throw.IfNullOrWhiteSpace(apiKey, nameof(apiKey));
            Throw.IfNullOrWhiteSpace(listenKey, nameof(listenKey));

            var request = new BinanceHttpRequest("/api/v1/userDataStream")
            {
                ApiKey = apiKey
            };

            request.AddParameter("listenKey", listenKey);

            // TODO
            // A POST will either get back your current stream (and effectively do a PUT) or get back a new stream.
            //return client.PostAsync(request, token);
            return client.PutAsync(request, token);
        }

        /// <summary>
        /// Close out a user data stream.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="user"></param>
        /// <param name="listenKey"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task<string> UserStreamCloseAsync(this IBinanceHttpClient client, IBinanceApiUser user, string listenKey, CancellationToken token = default)
        {
            Throw.IfNull(user, nameof(user));

            return UserStreamCloseAsync(client, user.ApiKey, listenKey, token);
        }

        /// <summary>
        /// Close out a user data stream.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="apiKey"></param>
        /// <param name="listenKey"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task<string> UserStreamCloseAsync(this IBinanceHttpClient client, string apiKey, string listenKey, CancellationToken token = default)
        {
            Throw.IfNullOrWhiteSpace(apiKey, nameof(apiKey));
            Throw.IfNullOrWhiteSpace(listenKey, nameof(listenKey));

            var request = new BinanceHttpRequest("/api/v1/userDataStream")
            {
                ApiKey = apiKey
            };

            request.AddParameter("listenKey", listenKey);

            return client.DeleteAsync(request, token);
        }

        #endregion User Stream
    }
}
