using System;
using System.Threading;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace Binance
{
    /// <summary>
    /// C# adapter for Binance Rest API using <see cref="IBinanceHttpClient"/>.
    /// All return values are either a JSON object or array.
    /// </summary>
    public static class BinanceHttpClientExtensions
    {
        #region General

        /// <summary>
        /// Test connectivity to the server.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<string> PingAsync(this IBinanceHttpClient client, CancellationToken token = default)
        {
            Throw.IfNull(client, nameof(client));

            if (client.RateLimiter != null)
            {
                await client.RateLimiter.DelayAsync(token: token)
                    .ConfigureAwait(false);
            }

            return await client.GetAsync("/api/v1/ping", token)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Get exchange information.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<string> GetExchangeInfoAsync(this IBinanceHttpClient client, CancellationToken token = default)
        {
            Throw.IfNull(client, nameof(client));

            if (client.RateLimiter != null)
            {
                await client.RateLimiter.DelayAsync(token: token)
                    .ConfigureAwait(false);
            }

            return await client.GetAsync("/api/v1/exchangeInfo", token)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Test connectivity to the server and get the current time.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<string> GetServerTimeAsync(this IBinanceHttpClient client, CancellationToken token = default)
        {
            Throw.IfNull(client, nameof(client));

            if (client.RateLimiter != null)
            {
                await client.RateLimiter.DelayAsync(token: token)
                    .ConfigureAwait(false);
            }

            return await client.GetAsync("/api/v1/time", token)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Get system status.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<string> GetSystemStatusAsync(this IBinanceHttpClient client, CancellationToken token = default)
        {
            Throw.IfNull(client, nameof(client));

            if (client.RateLimiter != null)
            {
                await client.RateLimiter.DelayAsync(token: token)
                    .ConfigureAwait(false);
            }

            return await client.GetAsync("/wapi/v3/systemStatus.html", token)
                .ConfigureAwait(false);
        }

        #endregion General

        #region Market Data

        /// <summary>
        /// Get order book (market depth) of a symbol.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="symbol"></param>
        /// <param name="limit">Valid values: [5, 10, 20, 50, 100, 500, 1000] (default: 100).</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<string> GetOrderBookAsync(this IBinanceHttpClient client, string symbol, int limit = default, CancellationToken token = default)
        {
            Throw.IfNull(client, nameof(client));
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            if (client.RateLimiter != null)
            {
                await client.RateLimiter
                    .DelayAsync(limit >= 1000 ? 10 : limit >= 500 ? 5 : 1, token)
                    .ConfigureAwait(false);
            }

            var request = new BinanceHttpRequest("/api/v1/depth");

            request.AddParameter("symbol", symbol.FormatSymbol());

            if (limit > 0)
            {
                request.AddParameter("limit", limit);
            }

            return await client.GetAsync(request, token)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Get latest (non-compressed) trades.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="symbol"></param>
        /// <param name="limit"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<string> GetTradesAsync(this IBinanceHttpClient client, string symbol, int limit = default, CancellationToken token = default)
        {
            Throw.IfNull(client, nameof(client));
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            if (client.RateLimiter != null)
            {
                await client.RateLimiter.DelayAsync(token: token)
                    .ConfigureAwait(false);
            }

            var request = new BinanceHttpRequest("/api/v1/trades");

            request.AddParameter("symbol", symbol.FormatSymbol());

            if (limit > 0)
                request.AddParameter("limit", limit);

            return await client.GetAsync(request, token)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Get older (non-compressed) trades.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="apiKey"></param>
        /// <param name="symbol"></param>
        /// <param name="fromId">TradeId to fetch from. Default gets most recent trades.</param>
        /// <param name="limit">Default 500; max 1000.</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<string> GetTradesAsync(this IBinanceHttpClient client, string apiKey, string symbol, long fromId = BinanceApi.NullId, int limit = default, CancellationToken token = default)
        {
            Throw.IfNull(client, nameof(client));
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            if (client.RateLimiter != null)
            {
                await client.RateLimiter.DelayAsync(5, token)
                    .ConfigureAwait(false);
            }

            var request = new BinanceHttpRequest("/api/v1/historicalTrades")
            {
                ApiKey = apiKey
            };

            request.AddParameter("symbol", symbol.FormatSymbol());

            if (fromId >= 0)
                request.AddParameter("fromId", fromId);

            if (limit > 0)
                request.AddParameter("limit", limit);

            return await client.GetAsync(request, token)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Get compressed, aggregate trades. Trades that fill at the time, from the same order, with the same price will have the quantity aggregated.
        /// If fromdId, startTime, and endTime are not sent, the most recent aggregate trades will be returned.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="symbol"></param>
        /// <param name="fromId">ID to get aggregate trades from INCLUSIVE.</param>
        /// <param name="limit">Default 500; max 1000.</param>
        /// <param name="startTime">Timestamp in ms to get aggregate trades from INCLUSIVE.</param>
        /// <param name="endTime">Timestamp in ms to get aggregate trades until INCLUSIVE.</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<string> GetAggregateTradesAsync(this IBinanceHttpClient client, string symbol, long fromId = BinanceApi.NullId, int limit = default, DateTime startTime = default, DateTime endTime = default, CancellationToken token = default)
        {
            Throw.IfNull(client, nameof(client));
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            if (client.RateLimiter != null)
            {
                await client.RateLimiter.DelayAsync(token: token)
                    .ConfigureAwait(false);
            }

            var request = new BinanceHttpRequest("/api/v1/aggTrades");

            request.AddParameter("symbol", symbol.FormatSymbol());

            if (fromId >= 0)
                request.AddParameter("fromId", fromId);

            if (startTime != default)
            {
                if (startTime.Kind != DateTimeKind.Utc)
                    throw new ArgumentException("Date/Time must be UTC.", nameof(startTime));

                request.AddParameter("startTime", startTime.ToTimestamp());
            }

            if (endTime != default)
            {
                if (endTime.Kind != DateTimeKind.Utc)
                    throw new ArgumentException("Date/Time must be UTC.", nameof(endTime));

                request.AddParameter("endTime", endTime.ToTimestamp());
            }

            if (startTime == default || endTime == default)
            {
                if (limit > 0)
                {
                    request.AddParameter("limit", limit);
                }
            }
            else
            {
                if (endTime < startTime)
                    throw new ArgumentException($"Time ({nameof(endTime)}) must not be less than {nameof(startTime)} ({startTime}).", nameof(endTime));

                if ((endTime - startTime).Duration() >= TimeSpan.FromHours(1))
                    throw new ArgumentException($"The interval between {nameof(startTime)} and {nameof(endTime)} must be less than 1 hour.", nameof(endTime));
            }

            return await client.GetAsync(request, token)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Get candlesticks for a symbol. Candlesticks/K-Lines are uniquely identified by their open time.
        /// If startTime and endTime are not sent, the most recent candlesticks are returned.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="symbol"></param>
        /// <param name="interval"></param>
        /// <param name="limit">Default 500; max 1000.</param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<string> GetCandlesticksAsync(this IBinanceHttpClient client, string symbol, CandlestickInterval interval, int limit = default, DateTime startTime = default, DateTime endTime = default, CancellationToken token = default)
        {
            Throw.IfNull(client, nameof(client));
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            if (client.RateLimiter != null)
            {
                await client.RateLimiter.DelayAsync(token: token)
                    .ConfigureAwait(false);
            }

            var request = new BinanceHttpRequest("/api/v1/klines");

            request.AddParameter("symbol", symbol.FormatSymbol());
            request.AddParameter("interval", interval.AsString());

            if (limit > 0)
                request.AddParameter("limit", limit);

            if (startTime != default)
            {
                if (startTime.Kind != DateTimeKind.Utc)
                    throw new ArgumentException("Date/Time must be UTC.", nameof(startTime));

                request.AddParameter("startTime", startTime.ToTimestamp());
            }

            // ReSharper disable once InvertIf
            if (endTime != default)
            {
                if (endTime.Kind != DateTimeKind.Utc)
                    throw new ArgumentException("Date/Time must be UTC.", nameof(endTime));

                request.AddParameter("endTime", endTime.ToTimestamp());
            }

            return await client.GetAsync(request, token)
                .ConfigureAwait(false);
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
        public static async Task<string> GetCandlesticksAsync(this IBinanceHttpClient client, string symbol, string interval, int limit = default, DateTime startTime = default, DateTime endTime = default, CancellationToken token = default)
        {
            Throw.IfNull(client, nameof(client));

            if (client.RateLimiter != null)
            {
                await client.RateLimiter.DelayAsync(token: token)
                    .ConfigureAwait(false);
            }

            return await client.GetCandlesticksAsync(symbol, interval.ToCandlestickInterval(), limit, startTime, endTime, token)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Get 24 hour price change statistics for a symbol or all symbols.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="symbol"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<string> Get24HourStatisticsAsync(this IBinanceHttpClient client, string symbol = null, CancellationToken token = default)
        {
            Throw.IfNull(client, nameof(client));

            if (client.RateLimiter != null)
            {
                await client.RateLimiter
                    .DelayAsync(string.IsNullOrWhiteSpace(symbol) ? 40 : 1, token)
                    .ConfigureAwait(false);
            }

            // When symbol is not provided use number of symbols that are TRADING for weight.
            var request = new BinanceHttpRequest("/api/v1/ticker/24hr");

            if (!string.IsNullOrWhiteSpace(symbol))
            {
                request.AddParameter("symbol", symbol.FormatSymbol());
            }

            return await client.GetAsync(request, token)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Get latest price for a symbol or all symbols.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="symbol"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<string> GetPriceAsync(this IBinanceHttpClient client, string symbol = null, CancellationToken token = default)
        {
            Throw.IfNull(client, nameof(client));

            if (client.RateLimiter != null)
            {
                await client.RateLimiter
                    .DelayAsync(string.IsNullOrWhiteSpace(symbol) ? 2 : 1, token)
                    .ConfigureAwait(false);
            }

            var request = new BinanceHttpRequest("/api/v3/ticker/price");

            if (!string.IsNullOrWhiteSpace(symbol))
            {
                request.AddParameter("symbol", symbol.FormatSymbol());
            }

            return await client.GetAsync(request, token)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Get current average price for a symbol.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="symbol"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<string> GetAvgPriceAsync(this IBinanceHttpClient client, string symbol, CancellationToken token = default)
        {
            Throw.IfNull(client, nameof(client));
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            if (client.RateLimiter != null)
            {
                await client.RateLimiter.DelayAsync(token: token)
                    .ConfigureAwait(false);
            }

            var request = new BinanceHttpRequest("/api/v3/avgPrice");

            request.AddParameter("symbol", symbol.FormatSymbol());

            return await client.GetAsync(request, token)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Get best price/quantity on the order book for a symbol.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="symbol"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<string> GetOrderBookTopAsync(this IBinanceHttpClient client, string symbol, CancellationToken token = default)
        {
            Throw.IfNull(client, nameof(client));
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            if (client.RateLimiter != null)
            {
                await client.RateLimiter.DelayAsync(token: token)
                    .ConfigureAwait(false);
            }

            var request = new BinanceHttpRequest("/api/v1/ticker/bookTicker");

            request.AddParameter("symbol", symbol.FormatSymbol());

            return await client.GetAsync(request, token)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Get best price/quantity on the order book for all symbols.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<string> GetOrderBookTopsAsync(this IBinanceHttpClient client, CancellationToken token = default)
        {
            Throw.IfNull(client, nameof(client));

            if (client.RateLimiter != null)
            {
                await client.RateLimiter.DelayAsync(2, token)
                   .ConfigureAwait(false);
            }

            return await client.GetAsync("/api/v1/ticker/bookTicker", token)
                .ConfigureAwait(false);
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
        /// <param name="stopPrice">Used with STOP_LOSS, STOP_LOSS_LIMIT, TAKE_PROFIT, and TAKE_PROFIT_LIMIT orders.</param>
        /// <param name="icebergQty">Used with iceberg orders.</param>
        /// <param name="recvWindow"></param>
        /// <param name="isTestOnly">If true, test new order creation and signature/recvWindow; creates and validates a new order but does not send it into the matching engine.</param>
        /// <param name="newOrderRespType">Set the response JSON.</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<string> PlaceOrderAsync(this IBinanceHttpClient client, IBinanceApiUser user, string symbol, OrderSide side, OrderType type, decimal quantity, decimal price, string newClientOrderId = null, TimeInForce? timeInForce = null, decimal stopPrice = 0, decimal icebergQty = 0, long recvWindow = default, bool isTestOnly = false, PlaceOrderResponseType newOrderRespType = PlaceOrderResponseType.Result, CancellationToken token = default)
        {
            Throw.IfNull(client, nameof(client));
            Throw.IfNull(user, nameof(user));
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            if (quantity <= 0)
                throw new ArgumentException("Order quantity must be greater than 0.", nameof(quantity));

            if (recvWindow == default)
                recvWindow = client.DefaultRecvWindow;

            if (user.RateLimiter != null)
            {
                await user.RateLimiter.DelayAsync(token: token)
                    .ConfigureAwait(false);
            }

            var request = new BinanceHttpRequest($"/api/v3/order{(isTestOnly ? "/test" : string.Empty)}")
            {
                ApiKey = user.ApiKey
            };

            request.AddParameter("symbol", symbol.FormatSymbol());
            request.AddParameter("side", side.ToString().ToUpperInvariant());
            request.AddParameter("type", type.AsString());
            request.AddParameter("newOrderRespType", newOrderRespType.ToString().ToUpperInvariant());
            request.AddParameter("quantity", quantity);

            if (price > 0)
                request.AddParameter("price", price);

            if (timeInForce.HasValue)
                request.AddParameter("timeInForce", timeInForce.ToString().ToUpperInvariant());

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

            await client.SignAsync(request, user, token)
                .ConfigureAwait(false);

            return await client.PostAsync(request, token)
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

            if (recvWindow == default)
                recvWindow = client.DefaultRecvWindow;

            if (user.RateLimiter != null)
            {
                await user.RateLimiter.DelayAsync(token: token)
                    .ConfigureAwait(false);
            }

            var request = new BinanceHttpRequest("/api/v3/order")
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

            await client.SignAsync(request, user, token)
                .ConfigureAwait(false);

            return await client.GetAsync(request, token)
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

            if (recvWindow == default)
                recvWindow = client.DefaultRecvWindow;

            if (user.RateLimiter != null)
            {
                await user.RateLimiter.DelayAsync(token: token)
                    .ConfigureAwait(false);
            }

            var request = new BinanceHttpRequest("/api/v3/order")
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

            await client.SignAsync(request, user, token)
                .ConfigureAwait(false);

            return await client.DeleteAsync(request, token)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Get all open orders on a symbol or all symbols.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="user"></param>
        /// <param name="symbol"></param>
        /// <param name="recvWindow"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<string> GetOpenOrdersAsync(this IBinanceHttpClient client, IBinanceApiUser user, string symbol = null, long recvWindow = default, CancellationToken token = default)
        {
            Throw.IfNull(client, nameof(client));
            Throw.IfNull(user, nameof(user));

            if (recvWindow == default)
                recvWindow = client.DefaultRecvWindow;

            if (client.RateLimiter != null)
            {
                await client.RateLimiter
                    .DelayAsync(string.IsNullOrWhiteSpace(symbol) ? 40 : 1, token)
                    .ConfigureAwait(false);
            }

            var request = new BinanceHttpRequest("/api/v3/openOrders")
            {
                ApiKey = user.ApiKey
            };

            if (!string.IsNullOrWhiteSpace(symbol))
                request.AddParameter("symbol", symbol.FormatSymbol());

            if (recvWindow > 0)
                request.AddParameter("recvWindow", recvWindow);

            await client.SignAsync(request, user, token)
                .ConfigureAwait(false);

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
        /// <param name="limit">Default 500; max 1000.</param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="recvWindow"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<string> GetOrdersAsync(this IBinanceHttpClient client, IBinanceApiUser user, string symbol, long orderId = BinanceApi.NullId, int limit = default, DateTime startTime = default, DateTime endTime = default, long recvWindow = default, CancellationToken token = default)
        {
            Throw.IfNull(client, nameof(client));
            Throw.IfNull(user, nameof(user));
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            if (recvWindow == default)
                recvWindow = client.DefaultRecvWindow;

            if (client.RateLimiter != null)
            {
                await client.RateLimiter.DelayAsync(5, token)
                    .ConfigureAwait(false);
            }

            var request = new BinanceHttpRequest("/api/v3/allOrders")
            {
                ApiKey = user.ApiKey
            };

            request.AddParameter("symbol", symbol.FormatSymbol());

            if (orderId >= 0)
                request.AddParameter("orderId", orderId);

            if (startTime != default)
            {
                if (startTime.Kind != DateTimeKind.Utc)
                    throw new ArgumentException("Date/Time must be UTC.", nameof(startTime));

                request.AddParameter("startTime", startTime.ToTimestamp());
            }

            if (endTime != default)
            {
                if (endTime.Kind != DateTimeKind.Utc)
                    throw new ArgumentException("Date/Time must be UTC.", nameof(endTime));

                request.AddParameter("endTime", endTime.ToTimestamp());
            }

            if (startTime == default || endTime == default)
            {
                if (limit > 0)
                {
                    request.AddParameter("limit", limit);
                }
            }
            else
            {
                if (endTime < startTime)
                    throw new ArgumentException($"Time ({nameof(endTime)}) must not be less than {nameof(startTime)} ({startTime}).", nameof(endTime));
            }

            if (recvWindow > 0)
                request.AddParameter("recvWindow", recvWindow);

            await client.SignAsync(request, user, token)
                .ConfigureAwait(false);

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

            if (recvWindow == default)
                recvWindow = client.DefaultRecvWindow;

            if (client.RateLimiter != null)
            {
                await client.RateLimiter.DelayAsync(5, token)
                    .ConfigureAwait(false);
            }

            var request = new BinanceHttpRequest("/api/v3/account")
            {
                ApiKey = user.ApiKey
            };

            if (recvWindow > 0)
                request.AddParameter("recvWindow", recvWindow);

            await client.SignAsync(request, user, token)
                .ConfigureAwait(false);

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
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="recvWindow"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<string> GetAccountTradesAsync(this IBinanceHttpClient client, IBinanceApiUser user, string symbol, long fromId = BinanceApi.NullId, int limit = default, DateTime startTime = default, DateTime endTime = default, long recvWindow = default, CancellationToken token = default)
        {
            Throw.IfNull(client, nameof(client));
            Throw.IfNull(user, nameof(user));
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            if (recvWindow == default)
                recvWindow = client.DefaultRecvWindow;

            if (client.RateLimiter != null)
            {
                await client.RateLimiter.DelayAsync(5, token)
                    .ConfigureAwait(false);
            }

            var request = new BinanceHttpRequest("/api/v3/myTrades")
            {
                ApiKey = user.ApiKey
            };

            request.AddParameter("symbol", symbol.FormatSymbol());

            if (fromId >= 0)
                request.AddParameter("fromId", fromId);

            if (startTime != default)
            {
                if (startTime.Kind != DateTimeKind.Utc)
                    throw new ArgumentException("Date/Time must be UTC.", nameof(startTime));

                request.AddParameter("startTime", startTime.ToTimestamp());
            }

            if (endTime != default)
            {
                if (endTime.Kind != DateTimeKind.Utc)
                    throw new ArgumentException("Date/Time must be UTC.", nameof(endTime));

                request.AddParameter("endTime", endTime.ToTimestamp());
            }

            if (startTime == default || endTime == default)
            {
                if (limit > 0)
                {
                    request.AddParameter("limit", limit);
                }
            }
            else
            {
                if (endTime < startTime)
                    throw new ArgumentException($"Time ({nameof(endTime)}) must not be less than {nameof(startTime)} ({startTime}).", nameof(endTime));
            }

            if (recvWindow > 0)
                request.AddParameter("recvWindow", recvWindow);

            await client.SignAsync(request, user, token)
                .ConfigureAwait(false);

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
        /// <param name="addressTag"></param>
        /// <param name="amount"></param>
        /// <param name="name">A description of the address (optional).</param>
        /// <param name="recvWindow"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<string> WithdrawAsync(this IBinanceHttpClient client, IBinanceApiUser user, string asset, string address, string addressTag, decimal amount, string name = null, long recvWindow = default, CancellationToken token = default)
        {
            Throw.IfNull(client, nameof(client));
            Throw.IfNull(user, nameof(user));
            Throw.IfNullOrWhiteSpace(asset, nameof(asset));
            Throw.IfNullOrWhiteSpace(address, nameof(address));

            if (amount <= 0)
                throw new ArgumentException("Withdraw amount must be greater than 0.", nameof(amount));

            if (recvWindow == default)
                recvWindow = client.DefaultRecvWindow;

            if (client.RateLimiter != null)
            {
                await client.RateLimiter.DelayAsync(token: token)
                    .ConfigureAwait(false);
            }

            var request = new BinanceHttpRequest("/wapi/v3/withdraw.html")
            {
                ApiKey = user.ApiKey
            };

            request.AddParameter("asset", asset.FormatSymbol());
            request.AddParameter("address", address);
            request.AddParameter("amount", amount);

            if (!string.IsNullOrWhiteSpace(addressTag))
                request.AddParameter("addressTag", addressTag);

            if (!string.IsNullOrWhiteSpace(name))
                request.AddParameter("name", name);

            if (recvWindow > 0)
                request.AddParameter("recvWindow", recvWindow);

            await client.SignAsync(request, user, token)
                .ConfigureAwait(false);

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
        public static async Task<string> GetDepositsAsync(this IBinanceHttpClient client, IBinanceApiUser user, string asset = null, DepositStatus? status = null, DateTime startTime = default, DateTime endTime = default, long recvWindow = default, CancellationToken token = default)
        {
            Throw.IfNull(client, nameof(client));
            Throw.IfNull(user, nameof(user));

            if (recvWindow == default)
                recvWindow = client.DefaultRecvWindow;

            if (client.RateLimiter != null)
            {
                await client.RateLimiter.DelayAsync(token: token)
                    .ConfigureAwait(false);
            }

            var request = new BinanceHttpRequest("/wapi/v3/depositHistory.html")
            {
                ApiKey = user.ApiKey
            };

            if (!string.IsNullOrWhiteSpace(asset))
                request.AddParameter("asset", asset.FormatSymbol());

            if (status.HasValue)
                request.AddParameter("status", (int)status);

            if (startTime != default)
            {
                if (startTime.Kind != DateTimeKind.Utc)
                    throw new ArgumentException("Date/Time must be UTC.", nameof(startTime));

                request.AddParameter("startTime", startTime.ToTimestamp());
            }

            if (endTime != default)
            {
                if (endTime.Kind != DateTimeKind.Utc)
                    throw new ArgumentException("Date/Time must be UTC.", nameof(endTime));

                request.AddParameter("endTime", endTime.ToTimestamp());
            }

            if (recvWindow > 0)
                request.AddParameter("recvWindow", recvWindow);

            await client.SignAsync(request, user, token)
                .ConfigureAwait(false);

            return await client.GetAsync(request, token)
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
        public static async Task<string> GetWithdrawalsAsync(this IBinanceHttpClient client, IBinanceApiUser user, string asset = null, WithdrawalStatus? status = null, DateTime startTime = default, DateTime endTime = default, long recvWindow = default, CancellationToken token = default)
        {
            Throw.IfNull(client, nameof(client));
            Throw.IfNull(user, nameof(user));

            if (recvWindow == default)
                recvWindow = client.DefaultRecvWindow;

            if (client.RateLimiter != null)
            {
                await client.RateLimiter.DelayAsync(token: token)
                    .ConfigureAwait(false);
            }

            var request = new BinanceHttpRequest("/wapi/v3/withdrawHistory.html")
            {
                ApiKey = user.ApiKey
            };

            if (!string.IsNullOrWhiteSpace(asset))
                request.AddParameter("asset", asset.FormatSymbol());

            if (status.HasValue)
                request.AddParameter("status", (int)status);

            if (startTime != default)
            {
                if (startTime.Kind != DateTimeKind.Utc)
                    throw new ArgumentException("Date/Time must be UTC.", nameof(startTime));

                request.AddParameter("startTime", startTime.ToTimestamp());
            }

            if (endTime != default)
            {
                if (endTime.Kind != DateTimeKind.Utc)
                    throw new ArgumentException("Date/Time must be UTC.", nameof(endTime));

                request.AddParameter("endTime", endTime.ToTimestamp());
            }

            if (recvWindow > 0)
                request.AddParameter("recvWindow", recvWindow);

            await client.SignAsync(request, user, token)
                .ConfigureAwait(false);

            return await client.GetAsync(request, token)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Get the deposit address for an asset.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="user"></param>
        /// <param name="asset"></param>
        /// <param name="recvWindow"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<string> GetDepositAddressAsync(this IBinanceHttpClient client, IBinanceApiUser user, string asset, long recvWindow = default, CancellationToken token = default)
        {
            Throw.IfNull(client, nameof(client));
            Throw.IfNullOrWhiteSpace(asset, nameof(asset));

            if (client.RateLimiter != null)
            {
                await client.RateLimiter.DelayAsync(token: token)
                    .ConfigureAwait(false);
            }

            var request = new BinanceHttpRequest("/wapi/v3/depositAddress.html")
            {
                ApiKey = user.ApiKey
            };

            request.AddParameter("asset", asset.FormatSymbol());

            if (recvWindow > 0)
                request.AddParameter("recvWindow", recvWindow);

            await client.SignAsync(request, user, token)
                .ConfigureAwait(false);

            return await client.GetAsync(request, token)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Get the withdraw fee for an asset.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="user"></param>
        /// <param name="asset"></param>
        /// <param name="recvWindow"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<string> GetWithdrawFeeAsync(this IBinanceHttpClient client, IBinanceApiUser user, string asset, long recvWindow = default, CancellationToken token = default)
        {
            Throw.IfNull(client, nameof(client));
            Throw.IfNullOrWhiteSpace(asset, nameof(asset));

            if (client.RateLimiter != null)
            {
                await client.RateLimiter.DelayAsync(token: token)
                    .ConfigureAwait(false);
            }

            var request = new BinanceHttpRequest("/wapi/v3/withdrawFee.html")
            {
                ApiKey = user.ApiKey
            };

            request.AddParameter("asset", asset.FormatSymbol());

            if (recvWindow > 0)
                request.AddParameter("recvWindow", recvWindow);

            await client.SignAsync(request, user, token)
                .ConfigureAwait(false);

            return await client.GetAsync(request, token)
                .ConfigureAwait(false);
        }

        /// <summary>
        /// Get the account status.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="user"></param>
        /// <param name="recvWindow"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<string> GetAccountStatusAsync(this IBinanceHttpClient client, IBinanceApiUser user, long recvWindow = default, CancellationToken token = default)
        {
            Throw.IfNull(client, nameof(client));

            if (client.RateLimiter != null)
            {
                await client.RateLimiter.DelayAsync(token: token)
                    .ConfigureAwait(false);
            }

            var request = new BinanceHttpRequest("/wapi/v3/accountStatus.html")
            {
                ApiKey = user.ApiKey
            };

            if (recvWindow > 0)
                request.AddParameter("recvWindow", recvWindow);

            await client.SignAsync(request, user, token)
                .ConfigureAwait(false);

            return await client.GetAsync(request, token)
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
        public static async Task<string> UserStreamStartAsync(this IBinanceHttpClient client, string apiKey, CancellationToken token = default)
        {
            Throw.IfNullOrWhiteSpace(apiKey, nameof(apiKey));

            if (client.RateLimiter != null)
            {
                await client.RateLimiter.DelayAsync(token: token)
                    .ConfigureAwait(false);
            }

            var request = new BinanceHttpRequest("/api/v1/userDataStream")
            {
                ApiKey = apiKey
            };

            return await client.PostAsync(request, token)
                .ConfigureAwait(false);
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
        public static async Task<string> UserStreamKeepAliveAsync(this IBinanceHttpClient client, string apiKey, string listenKey, CancellationToken token = default)
        {
            Throw.IfNullOrWhiteSpace(apiKey, nameof(apiKey));
            Throw.IfNullOrWhiteSpace(listenKey, nameof(listenKey));

            if (client.RateLimiter != null)
            {
                await client.RateLimiter.DelayAsync(token: token)
                    .ConfigureAwait(false);
            }

            var request = new BinanceHttpRequest("/api/v1/userDataStream")
            {
                ApiKey = apiKey
            };

            request.AddParameter("listenKey", listenKey);

            // TODO
            // A POST will either get back your current stream (and effectively do a PUT) or get back a new stream.
            //return await client.PostAsync(request, token)
            return await client.PutAsync(request, token)
                .ConfigureAwait(false);
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
        public static async Task<string> UserStreamCloseAsync(this IBinanceHttpClient client, string apiKey, string listenKey, CancellationToken token = default)
        {
            Throw.IfNullOrWhiteSpace(apiKey, nameof(apiKey));
            Throw.IfNullOrWhiteSpace(listenKey, nameof(listenKey));

            if (client.RateLimiter != null)
            {
                await client.RateLimiter.DelayAsync(token: token)
                    .ConfigureAwait(false);
            }

            var request = new BinanceHttpRequest("/api/v1/userDataStream")
            {
                ApiKey = apiKey
            };

            request.AddParameter("listenKey", listenKey);

            return await client.DeleteAsync(request, token)
                .ConfigureAwait(false);
        }

        #endregion User Stream
    }
}
