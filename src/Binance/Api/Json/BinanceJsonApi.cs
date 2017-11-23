using System;
using System.Threading;
using System.Threading.Tasks;
using Binance.Account;
using Binance.Account.Orders;
using Binance.Market;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Binance.Api.Json
{
    public class BinanceJsonApi : IBinanceJsonApi
    {
        #region Public Constants

        public static readonly string SuccessfulTestResponse = "{}";

        #endregion Public Constants

        #region Public Properties

        public IBinanceHttpClient HttpClient { get; }

        #endregion Public Properties

        #region Private Fields

        private long _timestampOffset;
        
        private DateTime _timestampOffsetUpdatedAt;

        private readonly SemaphoreSlim _timestampOffsetSync;

        private readonly ILogger<BinanceJsonApi> _logger;

        #endregion Private Fields

        #region Constructors

        /// <summary>
        /// Default constructor provides no rate limiter implementation,
        /// no configuration options, and no logging functionality.
        /// </summary>
        public BinanceJsonApi()
            : this(new BinanceHttpClient())
        { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="httpClient">The HTTP client.</param>
        /// <param name="logger">The logger</param>
        public BinanceJsonApi(IBinanceHttpClient httpClient, ILogger<BinanceJsonApi> logger = null)
        {
            Throw.IfNull(httpClient, nameof(httpClient));

            HttpClient = httpClient;

            _logger = logger;

            _timestampOffsetSync = new SemaphoreSlim(1, 1);
        }

        #endregion Constructors

        #region Connectivity

        public virtual Task<string> PingAsync(CancellationToken token = default)
        {
            return HttpClient.GetAsync("/api/v1/ping", token);
        }

        public virtual Task<string> GetServerTimeAsync(CancellationToken token = default)
        {
            return HttpClient.GetAsync("/api/v1/time", token);
        }

        #endregion Connectivity

        #region Market Data

        public virtual Task<string> GetExchangeInfoAsync(CancellationToken token = default)
        {
            return HttpClient.GetAsync("/api/v1/exchangeInfo", token);
        }

        public virtual Task<string> GetOrderBookAsync(string symbol, int limit = default, CancellationToken token = default)
        {
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            var request = new BinanceHttpRequest("/api/v1/depth");

            request.AddParameter("symbol", symbol.FormatSymbol());

            if (limit > 0)
            {
                request.AddParameter("limit", limit);
            }

            return HttpClient.GetAsync(request, token);
        }

        public virtual Task<string> GetAggregateTradesAsync(string symbol, long fromId = BinanceApi.NullId, int limit = default, long startTime = default, long endTime = default, CancellationToken token = default)
        {
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

            return HttpClient.GetAsync(request, token);
        }

        public virtual Task<string> GetCandlesticksAsync(string symbol, CandlestickInterval interval, int limit = default, long startTime = default, long endTime = default, CancellationToken token = default)
        {
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

            return HttpClient.GetAsync(request, token);
        }

        public virtual Task<string> Get24HourStatisticsAsync(string symbol, CancellationToken token = default)
        {
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            var request = new BinanceHttpRequest("/api/v1/ticker/24hr");

            request.AddParameter("symbol", symbol.FormatSymbol());

            return HttpClient.GetAsync(request, token);
        }

        public virtual Task<string> GetPricesAsync(CancellationToken token = default)
        {
            return HttpClient.GetAsync("/api/v1/ticker/allPrices", token);
        }

        public virtual Task<string> GetOrderBookTopsAsync(CancellationToken token = default)
        {
            return HttpClient.GetAsync("/api/v1/ticker/allBookTickers", token);
        }

        #endregion Market Data

        #region Account

        public virtual async Task<string> PlaceOrderAsync(IBinanceApiUser user, string symbol, OrderSide side, OrderType type, decimal quantity, decimal price, string newClientOrderId = null, TimeInForce? timeInForce = null, decimal stopPrice = 0, decimal icebergQty = 0, long recvWindow = default, bool isTestOnly = false, CancellationToken token = default)
        {
            Throw.IfNull(user, nameof(user));
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            if (quantity <= 0)
                throw new ArgumentException("Order quantity must be greater than 0.", nameof(quantity));

            if (recvWindow <= 0)
                recvWindow = HttpClient.Options.RecvWindowDefault ?? 0;

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
                request.AddParameter("icebergQty", icebergQty);

            if (recvWindow > 0)
                request.AddParameter("recvWindow", recvWindow);

            var timestamp = await GetTimestampAsync(token)
                .ConfigureAwait(false);

            request.AddParameter("timestamp", timestamp);

            var signature = user.Sign(request.QueryString);

            request.AddParameter("signature", signature);

            return await HttpClient.PostAsync(request, token, user.RateLimiter)
                .ConfigureAwait(false);
        }

        public virtual async Task<string> GetOrderAsync(IBinanceApiUser user, string symbol, long orderId = BinanceApi.NullId, string origClientOrderId = null, long recvWindow = default, CancellationToken token = default)
        {
            Throw.IfNull(user, nameof(user));
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            if (orderId < 0 && string.IsNullOrWhiteSpace(origClientOrderId))
                throw new ArgumentException($"Either '{nameof(orderId)}' or '{nameof(origClientOrderId)}' must be provided, but both were invalid.");

            if (recvWindow <= 0)
                recvWindow = HttpClient.Options.RecvWindowDefault ?? 0;

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

            var timestamp = await GetTimestampAsync(token)
                .ConfigureAwait(false);

            request.AddParameter("timestamp", timestamp);

            var signature = user.Sign(request.QueryString);

            request.AddParameter("signature", signature);

            return await HttpClient.GetAsync(request, token, user.RateLimiter)
                .ConfigureAwait(false);
        }

        public virtual async Task<string> CancelOrderAsync(IBinanceApiUser user, string symbol, long orderId = BinanceApi.NullId, string origClientOrderId = null, string newClientOrderId = null, long recvWindow = default, CancellationToken token = default)
        {
            Throw.IfNull(user, nameof(user));
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            if (orderId < 0 && string.IsNullOrWhiteSpace(origClientOrderId))
                throw new ArgumentException($"Either '{nameof(orderId)}' or '{nameof(origClientOrderId)}' must be provided, but both were invalid.");

            if (recvWindow <= 0)
                recvWindow = HttpClient.Options.RecvWindowDefault ?? 0;

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

            var timestamp = await GetTimestampAsync(token)
                .ConfigureAwait(false);

            request.AddParameter("timestamp", timestamp);

            var signature = user.Sign(request.QueryString);

            request.AddParameter("signature", signature);

            return await HttpClient.DeleteAsync(request, token, user.RateLimiter)
                .ConfigureAwait(false);
        }

        public virtual async Task<string> GetOpenOrdersAsync(IBinanceApiUser user, string symbol, long recvWindow = default, CancellationToken token = default)
        {
            Throw.IfNull(user, nameof(user));
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            if (recvWindow <= 0)
                recvWindow = HttpClient.Options.RecvWindowDefault ?? 0;

            var request = new BinanceHttpRequest($"/api/v3/openOrders")
            {
                ApiKey = user.ApiKey
            };

            request.AddParameter("symbol", symbol.FormatSymbol());

            if (recvWindow > 0)
                request.AddParameter("recvWindow", recvWindow);

            var timestamp = await GetTimestampAsync(token)
                .ConfigureAwait(false);

            request.AddParameter("timestamp", timestamp);

            var signature = user.Sign(request.QueryString);

            request.AddParameter("signature", signature);

            return await HttpClient.GetAsync(request, token)
                .ConfigureAwait(false);
        }

        public virtual async Task<string> GetOrdersAsync(IBinanceApiUser user, string symbol, long orderId = BinanceApi.NullId, int limit = default, long recvWindow = default, CancellationToken token = default)
        {
            Throw.IfNull(user, nameof(user));
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            if (recvWindow <= 0)
                recvWindow = HttpClient.Options.RecvWindowDefault ?? 0;

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

            var timestamp = await GetTimestampAsync(token)
                .ConfigureAwait(false);

            request.AddParameter("timestamp", timestamp);

            var signature = user.Sign(request.QueryString);

            request.AddParameter("signature", signature);

            return await HttpClient.GetAsync(request, token)
                .ConfigureAwait(false);
        }

        public virtual async Task<string> GetAccountInfoAsync(IBinanceApiUser user, long recvWindow = default, CancellationToken token = default)
        {
            Throw.IfNull(user, nameof(user));

            if (recvWindow <= 0)
                recvWindow = HttpClient.Options.RecvWindowDefault ?? 0;

            var request = new BinanceHttpRequest($"/api/v3/account")
            {
                ApiKey = user.ApiKey
            };

            var timestamp = await GetTimestampAsync(token)
                .ConfigureAwait(false);

            request.AddParameter("timestamp", timestamp);

            if (recvWindow > 0)
                request.AddParameter("recvWindow", recvWindow);

            var signature = user.Sign(request.QueryString);

            request.AddParameter("signature", signature);

            return await HttpClient.GetAsync(request, token)
                .ConfigureAwait(false);
        }

        public virtual async Task<string> GetTradesAsync(IBinanceApiUser user, string symbol, long fromId = BinanceApi.NullId, int limit = default, long recvWindow = default, CancellationToken token = default)
        {
            Throw.IfNull(user, nameof(user));
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            if (recvWindow <= 0)
                recvWindow = HttpClient.Options.RecvWindowDefault ?? 0;

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

            var timestamp = await GetTimestampAsync(token).ConfigureAwait(false);

            request.AddParameter("timestamp", timestamp);

            var signature = user.Sign(request.QueryString);

            request.AddParameter("signature", signature);

            return await HttpClient.GetAsync(request, token)
                .ConfigureAwait(false);
        }

        public virtual async Task<string> WithdrawAsync(IBinanceApiUser user, string asset, string address, decimal amount, string name = null, long recvWindow = default, CancellationToken token = default)
        {
            Throw.IfNull(user, nameof(user));
            Throw.IfNullOrWhiteSpace(asset, nameof(asset));
            Throw.IfNullOrWhiteSpace(address, nameof(address));

            if (amount <= 0)
                throw new ArgumentException("Withdraw amount must be greater than 0.", nameof(amount));

            if (recvWindow <= 0)
                recvWindow = HttpClient.Options.RecvWindowDefault ?? 0;

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

            var timestamp = await GetTimestampAsync(token)
                .ConfigureAwait(false);

            request.AddParameter("timestamp", timestamp);

            var signature = user.Sign(request.QueryString);

            request.AddParameter("signature", signature);

            return await HttpClient.PostAsync(request, token)
                .ConfigureAwait(false);
        }

        public virtual async Task<string> GetDepositsAsync(IBinanceApiUser user, string asset = null, DepositStatus? status = null, long startTime = default, long endTime = default, long recvWindow = default, CancellationToken token = default)
        {
            Throw.IfNull(user, nameof(user));

            if (recvWindow <= 0)
                recvWindow = HttpClient.Options.RecvWindowDefault ?? 0;

            var request = new BinanceHttpRequest($"/wapi/v1/getDepositHistory.html")
            {
                ApiKey = user.ApiKey
            };

            var timestamp = await GetTimestampAsync(token)
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

            return await HttpClient.PostAsync(request, token)
                .ConfigureAwait(false);
        }

        public virtual async Task<string> GetWithdrawalsAsync(IBinanceApiUser user, string asset = null, WithdrawalStatus? status = null, long startTime = default, long endTime = default, long recvWindow = default, CancellationToken token = default)
        {
            Throw.IfNull(user, nameof(user));

            if (recvWindow <= 0)
                recvWindow = HttpClient.Options.RecvWindowDefault ?? 0;

            var request = new BinanceHttpRequest($"/wapi/v1/getWithdrawHistory.html")
            {
                ApiKey = user.ApiKey
            };

            var timestamp = await GetTimestampAsync(token)
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

            return await HttpClient.PostAsync(request, token)
                .ConfigureAwait(false);
        }

        #endregion Account

        #region User Stream

        public virtual Task<string> UserStreamStartAsync(IBinanceApiUser user, CancellationToken token = default)
        {
            Throw.IfNull(user, nameof(user));

            var request = new BinanceHttpRequest("/api/v1/userDataStream")
            {
                ApiKey = user.ApiKey
            };

            return HttpClient.PostAsync(request, token);
        }

        public virtual Task<string> UserStreamKeepAliveAsync(IBinanceApiUser user, string listenKey, CancellationToken token = default)
        {
            Throw.IfNull(user, nameof(user));
            Throw.IfNullOrWhiteSpace(listenKey, nameof(listenKey));

            var request = new BinanceHttpRequest("/api/v1/userDataStream")
            {
                ApiKey = user.ApiKey
            };

            request.AddParameter("listenKey", listenKey);

            return HttpClient.PutAsync(request, token);
        }

        public virtual Task<string> UserStreamCloseAsync(IBinanceApiUser user, string listenKey, CancellationToken token = default)
        {
            Throw.IfNull(user, nameof(user));
            Throw.IfNullOrWhiteSpace(listenKey, nameof(listenKey));

            var request = new BinanceHttpRequest("/api/v1/userDataStream")
            {
                ApiKey = user.ApiKey
            };

            request.AddParameter("listenKey", listenKey);

            return HttpClient.DeleteAsync(request, token);
        }

        #endregion User Stream

        #region Private Methods

        /// <summary>
        /// Get local system timestamp synchronized with server time.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task<long> GetTimestampAsync(CancellationToken token = default)
        {
            await _timestampOffsetSync.WaitAsync(token)
                .ConfigureAwait(false);

            try
            {
                if (DateTime.UtcNow - _timestampOffsetUpdatedAt > TimeSpan.FromMinutes(HttpClient.Options.TimestampOffsetRefreshPeriodMinutes))
                {
                    var systemTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                    var json = await GetServerTimeAsync(token).ConfigureAwait(false);

                    // Calculate timestamp offset to account for time differences and delays.
                    _timestampOffset = JObject.Parse(json)["serverTime"].Value<long>() - systemTime;

                    // Record the current system time to determine when to refresh offset.
                    _timestampOffsetUpdatedAt = DateTime.UtcNow;
                }
            }
            catch (Exception e)
            {
                _logger?.LogWarning(e, $"{nameof(BinanceJsonApi)}.{nameof(GetTimestampAsync)} failed to update timestamp offset.");
            }
            finally
            {
                _timestampOffsetSync.Release();
            }

            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + _timestampOffset;
        }

        #endregion Private Methods

        #region IDisposable

        private bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _timestampOffsetSync?.Dispose();
                HttpClient?.Dispose();
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
        }
        
        #endregion IDisposable
    }
}
