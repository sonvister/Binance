using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Binance.Account;
using Binance.Account.Orders;
using Binance.Market;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace Binance.Api.Json
{
    public class BinanceJsonApi : IBinanceJsonApi
    {
        #region Public Constants

        public static readonly string EndpointUrl = "https://www.binance.com";

        public static readonly string SuccessfulTestResponse = "{}";

        public const int TimestampOffsetRefreshPeriodMinutesDefault = 60;

        public static readonly int QueryRateLimitCountDefault = 1200;
        public static readonly int QueryRateLimitDurationMinutesDefault = 1;

        public static readonly int QueryRateLimitBurstCountDefault = 100;
        public static readonly int QueryRateLimitBurstDurationSecondsDefault = 1;

        public static readonly int OrderRateLimitCountDefault = 100000;
        public static readonly int OrderRateLimitDurationDaysDefault = 1;

        public static readonly int OrderRateLimitBurstCountDefault = 10;
        public static readonly int OrderRateLimitBurstDurationSecondsDefault = 1;

        #endregion Public Constants

        #region Public Properties

        public IApiRateLimiter RateLimiter { get; }

        #endregion Public Properties

        #region Private Constants

        private const string RequestHeaderKeyName = "X-MBX-APIKEY";

        #endregion Private Constants

        #region Private Fields

        private readonly HttpClient _httpClient;

        private long _timestampOffset;
        
        private DateTime _timestampOffsetUpdatedAt;

        private readonly SemaphoreSlim _timestampOffsetSync;

        private readonly BinanceJsonApiOptions _options;

        private readonly ILogger<BinanceJsonApi> _logger;

        #endregion Private Fields

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="rateLimiter">The rate limiter (auto configured).</param>
        /// <param name="options">The options.</param>
        /// <param name="logger">The logger</param>
        public BinanceJsonApi(IApiRateLimiter rateLimiter = null, IOptions<BinanceJsonApiOptions> options = null, ILogger<BinanceJsonApi> logger = null)
        {
            RateLimiter = rateLimiter;

            _options = options?.Value;
            _logger = logger;

            _timestampOffsetSync = new SemaphoreSlim(1, 1);

            // Configure query rate limiter.
            RateLimiter?.Configure(
                TimeSpan.FromMinutes(options?.Value.QueryRateLimitDurationMinutes ?? QueryRateLimitDurationMinutesDefault),
                options?.Value.QueryRateLimitCount ?? QueryRateLimitCountDefault);

            // Configure query burst rate limiter.
            RateLimiter?.Configure(
                TimeSpan.FromSeconds(options?.Value.QueryRateLimitBurstDurationSeconds ?? QueryRateLimitBurstDurationSecondsDefault),
                options?.Value.QueryRateLimitBurstCount ?? QueryRateLimitBurstCountDefault);

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(EndpointUrl)
            };

            var version = GetType().Assembly.GetName().Version;

            var versionString = $"{version.Major}.{version.Minor}.{version.Build}{(version.Revision > 0 ? $".{version.Revision}" : string.Empty)}";

            _httpClient.DefaultRequestHeaders.Add("User-Agent", $"Binance/{versionString} (.NET; +https://github.com/sonvister/Binance)");
        }

        #endregion Constructors

        #region Connectivity

        public virtual Task<string> PingAsync(CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            return GetAsync("/api/v1/ping", null, RateLimiter, token);
        }

        public virtual Task<string> GetServerTimeAsync(CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            return GetAsync("/api/v1/time", null, RateLimiter, token);
        }

        #endregion Connectivity

        #region Market Data

        public virtual Task<string> GetOrderBookAsync(string symbol, int limit = default, CancellationToken token = default)
        {
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            token.ThrowIfCancellationRequested();

            var totalParams = $"symbol={symbol.FormatSymbol()}";

            if (limit > 0)
            {
                totalParams += $"&limit={limit}";
            }

            return GetAsync($"/api/v1/depth?{totalParams}", null, RateLimiter, token);
        }

        public virtual Task<string> GetAggregateTradesAsync(string symbol, long fromId = BinanceApi.NullId, int limit = default, long startTime = default, long endTime = default, CancellationToken token = default)
        {
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            token.ThrowIfCancellationRequested();

            var totalParams = $"symbol={symbol.FormatSymbol()}";

            if (fromId >= 0)
                totalParams += $"&fromId={fromId}";

            if (startTime > 0)
                totalParams += $"&startTime={startTime}";

            if (endTime > 0)
                totalParams += $"&endTime={endTime}";

            if (startTime <= 0 || endTime <= 0)
            {
                if (limit > 0)
                    totalParams += $"&limit={limit}";
            }
            else
            {
                var start = DateTimeOffset.FromUnixTimeMilliseconds(startTime);
                var end = DateTimeOffset.FromUnixTimeMilliseconds(endTime);

                if ((end - start).Duration() >= TimeSpan.FromHours(24))
                    throw new ArgumentException($"The interval between {nameof(startTime)} and {nameof(endTime)} must be less than 24 hours.", nameof(endTime));
            }

            return GetAsync($"/api/v1/aggTrades?{totalParams}", null, RateLimiter, token);
        }

        public virtual Task<string> GetCandlesticksAsync(string symbol, CandlestickInterval interval, int limit = default, long startTime = default, long endTime = default, CancellationToken token = default)
        {
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            token.ThrowIfCancellationRequested();

            var totalParams = $"symbol={symbol.FormatSymbol()}&interval={interval.AsString()}";

            if (limit > 0)
                totalParams += $"&limit={limit}";

            if (startTime > 0)
                totalParams += $"&startTime={startTime}";

            if (endTime > 0)
                totalParams += $"&endTime={endTime}";

            return GetAsync($"/api/v1/klines?{totalParams}", null, RateLimiter, token);
        }

        public virtual Task<string> Get24HourStatisticsAsync(string symbol, CancellationToken token = default)
        {
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            token.ThrowIfCancellationRequested();

            return GetAsync($"/api/v1/ticker/24hr?symbol={symbol.FormatSymbol()}", null, RateLimiter, token);
        }

        public virtual Task<string> GetPricesAsync(CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            return GetAsync("/api/v1/ticker/allPrices", null, RateLimiter, token);
        }

        public virtual Task<string> GetOrderBookTopsAsync(CancellationToken token = default)
        {
            token.ThrowIfCancellationRequested();

            return GetAsync("/api/v1/ticker/allBookTickers", null, RateLimiter, token);
        }

        #endregion Market Data

        #region Account

        public virtual async Task<string> PlaceOrderAsync(IBinanceApiUser user, string symbol, OrderSide side, OrderType type, decimal quantity, decimal price, string newClientOrderId = null, TimeInForce? timeInForce = null, decimal stopPrice = 0, decimal icebergQty = 0, long recvWindow = default, bool isTestOnly = false, CancellationToken token = default)
        {
            Throw.IfNull(user, nameof(user));
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            token.ThrowIfCancellationRequested();

            if (quantity <= 0)
                throw new ArgumentException("Order quantity must be greater than 0.", nameof(quantity));

            if (recvWindow <= 0)
                recvWindow = _options?.RecvWindowDefault ?? 0;

            var totalParams = $"symbol={symbol.FormatSymbol()}&side={side.ToString().ToUpper()}&type={type.ToString().ToUpper()}&quantity={quantity}";

            if (price > 0)
                totalParams += $"&price={price}";

            if (timeInForce.HasValue)
                totalParams += $"&timeInForce={timeInForce.ToString().ToUpper()}";

            if (!string.IsNullOrWhiteSpace(newClientOrderId))
                totalParams += $"&newClientOrderId={newClientOrderId}";

            if (stopPrice > 0)
                totalParams += $"&stopPrice={stopPrice}";

            if (icebergQty > 0)
                totalParams += $"&icebergQty={icebergQty}";

            if (recvWindow > 0)
                totalParams += $"&recvWindow={recvWindow}";

            var timestamp = await GetTimestampAsync(token)
                .ConfigureAwait(false);

            totalParams += $"&timestamp={timestamp}";

            var signature = user.Sign(totalParams);

            var query = $"{totalParams}&signature={signature}";

            return await PostAsync($"/api/v3/order{(isTestOnly ? "/test" : string.Empty)}?{query}", null, user, user.RateLimiter, token)
                .ConfigureAwait(false);
        }

        public virtual async Task<string> GetOrderAsync(IBinanceApiUser user, string symbol, long orderId = BinanceApi.NullId, string origClientOrderId = null, long recvWindow = default, CancellationToken token = default)
        {
            Throw.IfNull(user, nameof(user));
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            if (orderId < 0 && string.IsNullOrWhiteSpace(origClientOrderId))
                throw new ArgumentException($"Either '{nameof(orderId)}' or '{nameof(origClientOrderId)}' must be provided, but both were invalid.");

            token.ThrowIfCancellationRequested();

            if (recvWindow <= 0)
                recvWindow = _options?.RecvWindowDefault ?? 0;

            var totalParams = $"symbol={symbol.FormatSymbol()}";

            if (orderId >= 0)
                totalParams += $"&orderId={orderId}";

            if (!string.IsNullOrWhiteSpace(origClientOrderId))
                totalParams += $"&origClientOrderId={origClientOrderId}";

            if (recvWindow > 0)
                totalParams += $"&recvWindow={recvWindow}";

            var timestamp = await GetTimestampAsync(token)
                .ConfigureAwait(false);

            totalParams += $"&timestamp={timestamp}";

            var signature = user.Sign(totalParams);

            return await GetAsync($"/api/v3/order?{totalParams}&signature={signature}", user, user.RateLimiter, token)
                .ConfigureAwait(false);
        }

        public virtual async Task<string> CancelOrderAsync(IBinanceApiUser user, string symbol, long orderId = BinanceApi.NullId, string origClientOrderId = null, string newClientOrderId = null, long recvWindow = default, CancellationToken token = default)
        {
            Throw.IfNull(user, nameof(user));
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            if (orderId < 0 && string.IsNullOrWhiteSpace(origClientOrderId))
                throw new ArgumentException($"Either '{nameof(orderId)}' or '{nameof(origClientOrderId)}' must be provided, but both were invalid.");

            token.ThrowIfCancellationRequested();

            if (recvWindow <= 0)
                recvWindow = _options?.RecvWindowDefault ?? 0;

            var totalParams = $"symbol={symbol.FormatSymbol()}";

            if (orderId >= 0)
                totalParams += $"&orderId={orderId}";

            if (!string.IsNullOrWhiteSpace(origClientOrderId))
                totalParams += $"&origClientOrderId={origClientOrderId}";

            if (!string.IsNullOrWhiteSpace(newClientOrderId))
                totalParams += $"&newClientOrderId={newClientOrderId}";

            if (recvWindow > 0)
                totalParams += $"&recvWindow={recvWindow}";

            var timestamp = await GetTimestampAsync(token)
                .ConfigureAwait(false);

            totalParams += $"&timestamp={timestamp}";

            var signature = user.Sign(totalParams);

            return await DeleteAsync($"/api/v3/order?{totalParams}&signature={signature}", user, user.RateLimiter, token)
                .ConfigureAwait(false);
        }

        public virtual async Task<string> GetOpenOrdersAsync(IBinanceApiUser user, string symbol, long recvWindow = default, CancellationToken token = default)
        {
            Throw.IfNull(user, nameof(user));
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            token.ThrowIfCancellationRequested();

            if (recvWindow <= 0)
                recvWindow = _options?.RecvWindowDefault ?? 0;

            var totalParams = $"symbol={symbol.FormatSymbol()}";

            if (recvWindow > 0)
                totalParams += $"&recvWindow={recvWindow}";

            var timestamp = await GetTimestampAsync(token).ConfigureAwait(false);

            totalParams += $"&timestamp={timestamp}";

            var signature = user.Sign(totalParams);

            return await GetAsync($"/api/v3/openOrders?{totalParams}&signature={signature}", user, RateLimiter, token)
                .ConfigureAwait(false);
        }

        public virtual async Task<string> GetOrdersAsync(IBinanceApiUser user, string symbol, long orderId = BinanceApi.NullId, int limit = default, long recvWindow = default, CancellationToken token = default)
        {
            Throw.IfNull(user, nameof(user));
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            token.ThrowIfCancellationRequested();

            if (recvWindow <= 0)
                recvWindow = _options?.RecvWindowDefault ?? 0;

            var totalParams = $"symbol={symbol.FormatSymbol()}";

            if (orderId >= 0)
                totalParams += $"&orderId={orderId}";

            if (limit > 0)
                totalParams += $"&limit={limit}";

            if (recvWindow > 0)
                totalParams += $"&recvWindow={recvWindow}";

            var timestamp = await GetTimestampAsync(token).ConfigureAwait(false);

            totalParams += $"&timestamp={timestamp}";

            var signature = user.Sign(totalParams);

            return await GetAsync($"/api/v3/allOrders?{totalParams}&signature={signature}", user, RateLimiter, token)
                .ConfigureAwait(false);
        }

        public virtual async Task<string> GetAccountInfoAsync(IBinanceApiUser user, long recvWindow = default, CancellationToken token = default)
        {
            Throw.IfNull(user, nameof(user));

            token.ThrowIfCancellationRequested();

            if (recvWindow <= 0)
                recvWindow = _options?.RecvWindowDefault ?? 0;

            var timestamp = await GetTimestampAsync(token).ConfigureAwait(false);

            var totalParams = $"timestamp={timestamp}";

            if (recvWindow > 0)
                totalParams += $"&recvWindow={recvWindow}";

            var signature = user.Sign(totalParams);

            return await GetAsync($"/api/v3/account?{totalParams}&signature={signature}", user, RateLimiter, token);
        }

        public virtual async Task<string> GetTradesAsync(IBinanceApiUser user, string symbol, long fromId = BinanceApi.NullId, int limit = default, long recvWindow = default, CancellationToken token = default)
        {
            Throw.IfNull(user, nameof(user));
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            token.ThrowIfCancellationRequested();

            if (recvWindow <= 0)
                recvWindow = _options?.RecvWindowDefault ?? 0;

            var totalParams = $"symbol={symbol.FormatSymbol()}";

            if (fromId >= 0)
                totalParams += $"&fromId={fromId}";

            if (limit > 0)
                totalParams += $"&limit={limit}";

            if (recvWindow > 0)
                totalParams += $"&recvWindow={recvWindow}";

            var timestamp = await GetTimestampAsync(token).ConfigureAwait(false);

            totalParams += $"&timestamp={timestamp}";

            var signature = user.Sign(totalParams);

            return await GetAsync($"/api/v3/myTrades?{totalParams}&signature={signature}", user, RateLimiter, token)
                .ConfigureAwait(false);
        }

        public virtual async Task<string> WithdrawAsync(IBinanceApiUser user, string asset, string address, decimal amount, string name = null, long recvWindow = default, CancellationToken token = default)
        {
            Throw.IfNull(user, nameof(user));
            Throw.IfNullOrWhiteSpace(asset, nameof(asset));
            Throw.IfNullOrWhiteSpace(address, nameof(address));

            if (amount <= 0)
                throw new ArgumentException("Withdraw amount must be greater than 0.", nameof(amount));

            token.ThrowIfCancellationRequested();

            if (recvWindow <= 0)
                recvWindow = _options?.RecvWindowDefault ?? 0;

            var totalParams = $"asset={asset.FormatSymbol()}&address={address}&amount={amount}";

            if (!string.IsNullOrWhiteSpace(name))
                totalParams += $"&name={name}";

            if (recvWindow > 0)
                totalParams += $"&recvWindow={recvWindow}";

            var timestamp = await GetTimestampAsync(token)
                .ConfigureAwait(false);

            totalParams += $"&timestamp={timestamp}";

            var signature = user.Sign(totalParams);

            return await PostAsync($"/wapi/v1/withdraw.html?{totalParams}&signature={signature}", null, user, RateLimiter, token)
                .ConfigureAwait(false);
        }

        public virtual async Task<string> GetDepositsAsync(IBinanceApiUser user, string asset = null, DepositStatus? status = null, long startTime = default, long endTime = default, long recvWindow = default, CancellationToken token = default)
        {
            Throw.IfNull(user, nameof(user));

            token.ThrowIfCancellationRequested();

            if (recvWindow <= 0)
                recvWindow = _options?.RecvWindowDefault ?? 0;

            var timestamp = await GetTimestampAsync(token)
                .ConfigureAwait(false);

            var totalParams = $"timestamp={timestamp}";

            if (!string.IsNullOrWhiteSpace(asset))
            {
                asset = asset.FormatSymbol();
                totalParams += $"&asset={asset}";
            }

            if (status.HasValue)
            {
                totalParams += $"&status={(int)status}";
            }

            if (startTime > 0)
                totalParams += $"&startTime={startTime}";

            if (endTime > 0)
                totalParams += $"&endTime={endTime}";

            if (recvWindow > 0)
                totalParams += $"&recvWindow={recvWindow}";

            var signature = user.Sign(totalParams);

            return await PostAsync($"/wapi/v1/getDepositHistory.html?{totalParams}&signature={signature}", null, user, RateLimiter, token)
                .ConfigureAwait(false);
        }

        public virtual async Task<string> GetWithdrawalsAsync(IBinanceApiUser user, string asset = null, WithdrawalStatus? status = null, long startTime = default, long endTime = default, long recvWindow = default, CancellationToken token = default)
        {
            Throw.IfNull(user, nameof(user));

            token.ThrowIfCancellationRequested();

            if (recvWindow <= 0)
                recvWindow = _options?.RecvWindowDefault ?? 0;

            var timestamp = await GetTimestampAsync(token)
                .ConfigureAwait(false);

            var totalParams = $"timestamp={timestamp}";

            if (!string.IsNullOrWhiteSpace(asset))
            {
                asset = asset.FormatSymbol();
                totalParams += $"&asset={asset}";
            }

            if (status.HasValue)
            {
                totalParams += $"&status={(int)status}";
            }

            if (startTime > 0)
                totalParams += $"&startTime={startTime}";

            if (endTime > 0)
                totalParams += $"&endTime={endTime}";

            if (recvWindow > 0)
                totalParams += $"&recvWindow={recvWindow}";

            var signature = user.Sign(totalParams);

            return await PostAsync($"/wapi/v1/getWithdrawHistory.html?{totalParams}&signature={signature}", null, user, RateLimiter, token)
                .ConfigureAwait(false);
        }

        #endregion Account

        #region User Stream

        public virtual Task<string> UserStreamStartAsync(IBinanceApiUser user, CancellationToken token = default)
        {
            Throw.IfNull(user, nameof(user));

            token.ThrowIfCancellationRequested();

            return PostAsync("/api/v1/userDataStream", null, user, RateLimiter, token);
        }

        public virtual Task<string> UserStreamKeepAliveAsync(IBinanceApiUser user, string listenKey, CancellationToken token = default)
        {
            Throw.IfNull(user, nameof(user));
            Throw.IfNullOrWhiteSpace(listenKey, nameof(listenKey));

            token.ThrowIfCancellationRequested();

            return PutAsync($"/api/v1/userDataStream?listenKey={listenKey}", null, user, RateLimiter, token);
        }

        public virtual Task<string> UserStreamCloseAsync(IBinanceApiUser user, string listenKey, CancellationToken token = default)
        {
            Throw.IfNull(user, nameof(user));
            Throw.IfNullOrWhiteSpace(listenKey, nameof(listenKey));

            token.ThrowIfCancellationRequested();

            return DeleteAsync($"/api/v1/userDataStream?listenKey={listenKey}", user, RateLimiter, token);
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
                if (DateTime.UtcNow - _timestampOffsetUpdatedAt > 
                    TimeSpan.FromMinutes(_options?.TimestampOffsetRefreshPeriodMinutes ?? TimestampOffsetRefreshPeriodMinutesDefault))
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

        private Task<string> GetAsync(string requestPath, IBinanceApiUser user = null, IApiRateLimiter rateLimiter = null, CancellationToken token = default)
        {
            return RequestAsync(HttpMethod.Get, requestPath, null, user, rateLimiter, token);
        }

        private Task<string> PostAsync(string requestPath, string body, IBinanceApiUser user = null, IApiRateLimiter rateLimiter = null, CancellationToken token = default)
        {
            return RequestAsync(HttpMethod.Post, requestPath, body, user, rateLimiter, token);
        }

        private Task<string> PutAsync(string requestPath, string body, IBinanceApiUser user = null, IApiRateLimiter rateLimiter = null, CancellationToken token = default)
        {
            return RequestAsync(HttpMethod.Put, requestPath, body, user, rateLimiter, token);
        }

        private Task<string> DeleteAsync(string requestPath, IBinanceApiUser user = null, IApiRateLimiter rateLimiter = null, CancellationToken token = default)
        {
            return RequestAsync(HttpMethod.Delete, requestPath, null, user, rateLimiter, token);
        }

        private async Task<string> RequestAsync(HttpMethod method, string requestPath, string body = null, IBinanceApiUser user = null, IApiRateLimiter rateLimiter = null, CancellationToken token = default)
        {
            var request = new HttpRequestMessage(method, requestPath);

            if (body != null)
            {
                request.Content = new StringContent(body, Encoding.UTF8, "application/x-www-form-urlencoded");
            }

            if (user != null)
            {
                request.Headers.Add(RequestHeaderKeyName, user.ApiKey);
            }

            _logger?.LogDebug($"{nameof(BinanceJsonApi)}.{nameof(RequestAsync)}: [{method.Method}] \"{requestPath}\"");

            if (rateLimiter != null)
            {
                await rateLimiter.DelayAsync(token)
                    .ConfigureAwait(false);
            }

            using (var response = await _httpClient.SendAsync(request, token).ConfigureAwait(false))
            {
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync()
                        .ConfigureAwait(false);

                    _logger?.LogDebug($"{nameof(BinanceJsonApi)}: \"{json}\"");

                    return json;
                }

                if (response.StatusCode == HttpStatusCode.GatewayTimeout)
                {
                    throw new BinanceUnknownStatusException();
                }

                var error = await response.Content.ReadAsStringAsync()
                    .ConfigureAwait(false);

                var errorCode = 0;
                string errorMessage = null;

                // ReSharper disable once InvertIf
                if (!string.IsNullOrWhiteSpace(error) && error.IsJsonObject())
                {
                    try // to parse server error response.
                    {
                        var jObject = JObject.Parse(error);

                        errorCode = jObject["code"]?.Value<int>() ?? 0;
                        errorMessage = jObject["msg"]?.Value<string>();
                    }
                    catch (Exception e)
                    {
                        _logger?.LogError(e, $"{nameof(BinanceJsonApi)}.{nameof(RequestAsync)} failed to parse server error response: \"{error}\"");
                    }
                }

                throw new BinanceHttpException(response.StatusCode, response.ReasonPhrase, errorCode, errorMessage);
            }
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
                _httpClient?.Dispose();
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
