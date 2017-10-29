using Binance.Account;
using Binance.Account.Orders;
using Binance.Market;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Binance.Api.Json
{
    public class BinanceJsonApi : IBinanceJsonApi
    {
        #region Public Constants

        public static readonly string EndpointUrl = "https://www.binance.com";

        public static readonly string SuccessfulTestResponse = "{}";

        public const int TimestampOffsetRefreshPeriodMinutesDefault = 60;

        #endregion Public Constants

        #region Public Properties

        public IRateLimiter RateLimiter { get; }

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

        public BinanceJsonApi(IRateLimiter rateLimiter = null, IOptions<BinanceJsonApiOptions> options = null, ILogger<BinanceJsonApi> logger = null)
        {
            RateLimiter = rateLimiter ?? new RateLimiter();

            _options = options?.Value;
            _logger = logger;

            _timestampOffsetSync = new SemaphoreSlim(1, 1);

            RateLimiter.Configure(
                options?.Value.RateLimiterCountDefault ?? Json.RateLimiter.CountDefault,
                TimeSpan.FromSeconds(options?.Value.RateLimiterDurationSecondsDefault ?? Json.RateLimiter.DurationSecondsDefault));

            _httpClient = new HttpClient()
            {
                BaseAddress = new Uri(EndpointUrl)
            };

            var version = GetType().Assembly.GetName().Version;

            _httpClient.DefaultRequestHeaders.Add("User-Agent", $"Binance/{version.Major}.{version.Minor}.{version.Build} (.NET; +https://github.com/sonvister/Binance)");
        }

        #endregion Constructors

        #region Connectivity

        public virtual Task<string> PingAsync(CancellationToken token = default)
        {
            return GetAsync("/api/v1/ping", token);
        }

        public virtual Task<string> GetServerTimeAsync(CancellationToken token = default)
        {
            return GetAsync("/api/v1/time", token);
        }

        #endregion Connectivity

        #region Market Data

        public virtual Task<string> GetOrderBookAsync(string symbol, int limit = default, CancellationToken token = default)
        {
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            var totalParams = $"symbol={symbol.FormatSymbol()}";

            if (limit > 0)
            {
                totalParams += $"&limit={limit}";
            }

            return GetAsync($"/api/v1/depth?{totalParams}", token);
        }

        public virtual Task<string> GetAggregateTradesAsync(string symbol, long fromId = BinanceApi.NullId, int limit = default, long startTime = default, long endTime = default, CancellationToken token = default)
        {
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

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

            return GetAsync($"/api/v1/aggTrades?{totalParams}", token);
        }

        public virtual Task<string> GetCandlesticksAsync(string symbol, KlineInterval interval, int limit = default, long startTime = default, long endTime = default, CancellationToken token = default)
        {
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            var totalParams = $"symbol={symbol.FormatSymbol()}&interval={interval.AsString()}";

            if (limit > 0)
                totalParams += $"&limit={limit}";

            if (startTime > 0)
                totalParams += $"&startTime={startTime}";

            if (endTime > 0)
                totalParams += $"&endTime={endTime}";

            return GetAsync($"/api/v1/klines?{totalParams}", token);
        }

        public virtual Task<string> Get24HourStatisticsAsync(string symbol, CancellationToken token = default)
        {
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            return GetAsync($"/api/v1/ticker/24hr?symbol={symbol.FormatSymbol()}", token);
        }

        public virtual Task<string> GetPricesAsync(CancellationToken token = default)
        {
            return GetAsync("/api/v1/ticker/allPrices", token);
        }

        public virtual Task<string> GetOrderBookTopsAsync(CancellationToken token = default)
        {
            return GetAsync("/api/v1/ticker/allBookTickers", token);
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

            var timestamp = await GetTimestampAsync(token).ConfigureAwait(false);

            totalParams += $"&timestamp={timestamp}";

            var signature = user.Sign(totalParams);

            var query = $"{totalParams}&signature={signature}";

            return await PostAsync($"/api/v3/order{(isTestOnly ? "/test" : string.Empty)}?{query}", string.Empty, token, user)
                .ConfigureAwait(false);
        }

        public virtual async Task<string> GetOrderAsync(IBinanceApiUser user, string symbol, long orderId = BinanceApi.NullId, string origClientOrderId = null, long recvWindow = default, CancellationToken token = default)
        {
            Throw.IfNull(user, nameof(user));
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            if (orderId < 0 && string.IsNullOrWhiteSpace(origClientOrderId))
                throw new ArgumentException($"Either '{nameof(orderId)}' or '{nameof(origClientOrderId)}' must be provided, but both were invalid.");

            if (recvWindow <= 0)
                recvWindow = _options?.RecvWindowDefault ?? 0;

            var totalParams = $"symbol={symbol.FormatSymbol()}";

            if (orderId >= 0)
                totalParams += $"&orderId={orderId}";

            if (!string.IsNullOrWhiteSpace(origClientOrderId))
                totalParams += $"&origClientOrderId={origClientOrderId}";

            if (recvWindow > 0)
                totalParams += $"&recvWindow={recvWindow}";

            var timestamp = await GetTimestampAsync(token).ConfigureAwait(false);

            totalParams += $"&timestamp={timestamp}";

            var signature = user.Sign(totalParams);

            return await GetAsync($"/api/v3/order?{totalParams}&signature={signature}", token, user)
                .ConfigureAwait(false);
        }

        public virtual async Task<string> CancelOrderAsync(IBinanceApiUser user, string symbol, long orderId = BinanceApi.NullId, string origClientOrderId = null, string newClientOrderId = null, long recvWindow = default, CancellationToken token = default)
        {
            Throw.IfNull(user, nameof(user));
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            if (orderId < 0 && string.IsNullOrWhiteSpace(origClientOrderId))
                throw new ArgumentException($"Either '{nameof(orderId)}' or '{nameof(origClientOrderId)}' must be provided, but both were invalid.");

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

            var timestamp = await GetTimestampAsync(token).ConfigureAwait(false);

            totalParams += $"&timestamp={timestamp}";

            var signature = user.Sign(totalParams);

            return await DeleteAsync($"/api/v3/order?{totalParams}&signature={signature}", token, user)
                .ConfigureAwait(false);
        }

        public virtual async Task<string> GetOpenOrdersAsync(IBinanceApiUser user, string symbol, long recvWindow = default, CancellationToken token = default)
        {
            Throw.IfNull(user, nameof(user));
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            if (recvWindow <= 0)
                recvWindow = _options?.RecvWindowDefault ?? 0;

            var totalParams = $"symbol={symbol.FormatSymbol()}";

            if (recvWindow > 0)
                totalParams += $"&recvWindow={recvWindow}";

            var timestamp = await GetTimestampAsync(token).ConfigureAwait(false);

            totalParams += $"&timestamp={timestamp}";

            var signature = user.Sign(totalParams);

            return await GetAsync($"/api/v3/openOrders?{totalParams}&signature={signature}", token, user)
                .ConfigureAwait(false);
        }

        public virtual async Task<string> GetOrdersAsync(IBinanceApiUser user, string symbol, long orderId = BinanceApi.NullId, int limit = default, long recvWindow = default, CancellationToken token = default)
        {
            Throw.IfNull(user, nameof(user));
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

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

            return await GetAsync($"/api/v3/allOrders?{totalParams}&signature={signature}", token, user)
                .ConfigureAwait(false);
        }

        public virtual async Task<string> GetAccountInfoAsync(IBinanceApiUser user, long recvWindow = default, CancellationToken token = default)
        {
            Throw.IfNull(user, nameof(user));

            if (recvWindow <= 0)
                recvWindow = _options?.RecvWindowDefault ?? 0;

            var timestamp = await GetTimestampAsync(token).ConfigureAwait(false);

            var totalParams = $"timestamp={timestamp}";

            if (recvWindow > 0)
                totalParams += $"&recvWindow={recvWindow}";

            var signature = user.Sign(totalParams);

            return await GetAsync($"/api/v3/account?{totalParams}&signature={signature}", token, user);
        }

        public virtual async Task<string> GetTradesAsync(IBinanceApiUser user, string symbol, long fromId = BinanceApi.NullId, int limit = default, long recvWindow = default, CancellationToken token = default)
        {
            Throw.IfNull(user, nameof(user));
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

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

            return await GetAsync($"/api/v3/myTrades?{totalParams}&signature={signature}", token, user)
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
                recvWindow = _options?.RecvWindowDefault ?? 0;

            var totalParams = $"asset={asset.FormatSymbol()}&address={address}&amount={amount}";

            if (!string.IsNullOrWhiteSpace(name))
                totalParams += $"&name={name}";

            if (recvWindow > 0)
                totalParams += $"&recvWindow={recvWindow}";

            var timestamp = await GetTimestampAsync(token).ConfigureAwait(false);

            totalParams += $"&timestamp={timestamp}";

            var signature = user.Sign(totalParams);

            return await PostAsync($"/wapi/v1/withdraw.html?{totalParams}&signature={signature}", null, token, user)
                .ConfigureAwait(false);
        }

        public virtual async Task<string> GetDepositsAsync(IBinanceApiUser user, string asset = null, DepositStatus? status = null, long startTime = default, long endTime = default, long recvWindow = default, CancellationToken token = default)
        {
            Throw.IfNull(user, nameof(user));

            if (recvWindow <= 0)
                recvWindow = _options?.RecvWindowDefault ?? 0;

            var timestamp = await GetTimestampAsync(token).ConfigureAwait(false);

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

            return await PostAsync($"/wapi/v1/getDepositHistory.html?{totalParams}&signature={signature}", null, token, user)
                .ConfigureAwait(false);
        }

        public virtual async Task<string> GetWithdrawalsAsync(IBinanceApiUser user, string asset = null, WithdrawalStatus? status = null, long startTime = default, long endTime = default, long recvWindow = default, CancellationToken token = default)
        {
            Throw.IfNull(user, nameof(user));

            if (recvWindow <= 0)
                recvWindow = _options?.RecvWindowDefault ?? 0;

            var timestamp = await GetTimestampAsync(token).ConfigureAwait(false);

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

            return await PostAsync($"/wapi/v1/getWithdrawHistory.html?{totalParams}&signature={signature}", null, token, user)
                .ConfigureAwait(false);
        }

        #endregion Account

        #region User Stream

        public virtual Task<string> UserStreamStartAsync(IBinanceApiUser user, CancellationToken token = default)
        {
            Throw.IfNull(user, nameof(user));

            return PostAsync("/api/v1/userDataStream", string.Empty, token, user);
        }

        public virtual Task<string> UserStreamKeepAliveAsync(IBinanceApiUser user, string listenKey, CancellationToken token = default)
        {
            Throw.IfNull(user, nameof(user));
            Throw.IfNullOrWhiteSpace(listenKey, nameof(listenKey));

            return PutAsync($"/api/v1/userDataStream?listenKey={listenKey}", string.Empty, token, user);
        }

        public virtual Task<string> UserStreamCloseAsync(IBinanceApiUser user, string listenKey, CancellationToken token = default)
        {
            Throw.IfNull(user, nameof(user));
            Throw.IfNullOrWhiteSpace(listenKey, nameof(listenKey));

            return DeleteAsync($"/api/v1/userDataStream?listenKey={listenKey}", token, user);
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
            await _timestampOffsetSync.WaitAsync(token);

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
                _logger?.LogWarning(e, $"{nameof(GetTimestampAsync)} failed to update timestamp offset.");
            }
            finally
            {
                _timestampOffsetSync.Release();
            }

            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + _timestampOffset;
        }

        private Task<string> GetAsync(string requestPath, CancellationToken token, IBinanceApiUser user = null)
        {
            return RequestAsync(HttpMethod.Get, requestPath, null, user, false, token);
        }

        private Task<string> PostAsync(string requestPath, string body, CancellationToken token, IBinanceApiUser user = null, bool bypassDelay = false)
        {
            return RequestAsync(HttpMethod.Post, requestPath, body, user, bypassDelay, token);
        }

        private Task<string> PutAsync(string requestPath, string body, CancellationToken token, IBinanceApiUser user = null, bool bypassDelay = false)
        {
            return RequestAsync(HttpMethod.Put, requestPath, body, user, bypassDelay, token);
        }

        private Task<string> DeleteAsync(string requestPath, CancellationToken token, IBinanceApiUser user = null, bool bypassDelay = false)
        {
            return RequestAsync(HttpMethod.Delete, requestPath, null, user, bypassDelay, token);
        }

        private async Task<string> RequestAsync(HttpMethod method, string requestPath, string body = null, IBinanceApiUser user = null, bool bypassDelay = false, CancellationToken token = default)
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

            if (!bypassDelay)
            {
                await RateLimiter.DelayAsync(token)
                    .ConfigureAwait(false);
            }

            using (var response = await _httpClient.SendAsync(request, token).ConfigureAwait(false))
            {
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync()
                        .ConfigureAwait(false);
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
                        _logger?.LogError(e, $"Failed to parse server error response: \"{error}\"");
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
