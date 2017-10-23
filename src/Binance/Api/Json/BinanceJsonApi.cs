using Binance.Accounts;
using Binance.Orders;
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

        #endregion Public Constants

        #region Public Properties

        public IRateLimiter RateLimiter { get; private set; }

        #endregion Public Properties

        #region Private Constants

        private const string UserAgent = "Binance/0.0.1-alpha (.NET)";

        private const string RequestHeaderKeyName = "X-MBX-APIKEY";

        #endregion Private Constants

        #region Private Fields

        private HttpClient _httpClient;

        private long _timestampOffset;

        private DateTime _timestampOffsetUpdatedAt;

        #endregion Private Fields

        #region Constructors

        public BinanceJsonApi(IRateLimiter rateLimiter = null)
        {
            RateLimiter = rateLimiter ?? new RateLimiter();

            _httpClient = new HttpClient()
            {
                BaseAddress = new Uri(EndpointUrl)
            };

            _httpClient.DefaultRequestHeaders.Add("User-Agent", UserAgent);
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

        public virtual Task<string> GetOrderBookAsync(string symbol, int limit = BinanceApi.OrderBookLimitDefault, CancellationToken token = default)
        {
            symbol = ThrowIfSymbolIsInvalid(symbol);

            if (limit < 1 || limit > BinanceApi.OrderBookLimitMax)
                throw new ArgumentException($"{nameof(BinanceJsonApi)}.{nameof(GetOrderBookAsync)} limit must be in the range [1-{BinanceApi.OrderBookLimitMax}].", nameof(limit));

            return GetAsync($"/api/v1/depth?symbol={symbol}&limit={limit}", token);
        }

        public virtual Task<string> GetAggregateTradesAsync(string symbol, long fromId = BinanceApi.NullId, long startTime = 0, long endTime = 0, int limit = BinanceApi.TradesLimitDefault, CancellationToken token = default)
        {
            symbol = ThrowIfSymbolIsInvalid(symbol);

            if (limit < 1 || limit > BinanceApi.TradesLimitMax)
                throw new ArgumentException($"{nameof(BinanceJsonApi)}.{nameof(GetAggregateTradesAsync)} limit must be in the range [1-{BinanceApi.TradesLimitMax}].", nameof(limit));

            var totalParams = $"/api/v1/aggTrades?symbol={symbol}";

            if (fromId >= 0)
                totalParams += $"&fromId={fromId}";

            if (startTime > 0)
                totalParams += $"&startTime={startTime}";

            if (endTime > 0)
                totalParams += $"&endTime={endTime}";

            if (startTime <= 0 || endTime <= 0)
                totalParams += $"&limit={limit}";
            else
            {
                var start = DateTimeOffset.FromUnixTimeMilliseconds(startTime);
                var end = DateTimeOffset.FromUnixTimeMilliseconds(endTime);

                if ((end - start).Duration() >= TimeSpan.FromHours(24))
                    throw new ArgumentException("The interval between startTime and endTime must be less than 24 hours.", nameof(endTime));
            }

            return GetAsync(totalParams, token);
        }

        public virtual Task<string> GetCandlesticksAsync(string symbol, KlineInterval interval, int limit = BinanceApi.CandlesticksLimitDefault, long startTime = 0, long endTime = 0, CancellationToken token = default)
        {
            symbol = ThrowIfSymbolIsInvalid(symbol);

            if (limit < 1 || limit > BinanceApi.CandlesticksLimitMax)
                throw new ArgumentException($"{nameof(BinanceJsonApi)}.{nameof(GetCandlesticksAsync)} limit must be in the range [1-{BinanceApi.CandlesticksLimitMax}].", nameof(limit));

            var totalParams = $"/api/v1/klines?symbol={symbol}&interval={interval.AsString()}&limit={limit}";

            if (startTime > 0)
                totalParams += $"&startTime={startTime}";

            if (endTime > 0)
                totalParams += $"&endTime={endTime}";

            return GetAsync(totalParams, token);
        }

        public virtual Task<string> Get24hStatsAsync(string symbol, CancellationToken token = default)
        {
            symbol = ThrowIfSymbolIsInvalid(symbol);

            return GetAsync($"/api/v1/ticker/24hr?symbol={symbol}", token);
        }

        public virtual Task<string> GetPrices(CancellationToken token = default)
        {
            return GetAsync($"/api/v1/ticker/allPrices", token);
        }

        public virtual Task<string> GetOrderBookTopsAsync(CancellationToken token = default)
        {
            return GetAsync($"/api/v1/ticker/allBookTickers", token);
        }

        #endregion Market Data

        #region Account

        public virtual async Task<string> PlaceOrderAsync(IBinanceUser user, string symbol, OrderSide side, OrderType type, decimal quantity, decimal price, string newClientOrderId = null, TimeInForce? timeInForce = null, decimal stopPrice = 0, decimal icebergQty = 0, long recvWindow = BinanceApi.RecvWindowDefault, CancellationToken token = default)
        {
            var query = await GetTotalParamsWithSignatureAsync(user, symbol, side, type, quantity, price, newClientOrderId, timeInForce, stopPrice, icebergQty, recvWindow, token)
                .ConfigureAwait(false);

            return await PostAsync($"/api/v3/order?{query}", string.Empty, token, user)
                .ConfigureAwait(false);
        }

        public virtual async Task<string> TestOrderAsync(IBinanceUser user, string symbol, OrderSide side, OrderType type, decimal quantity, decimal price, string newClientOrderId = null, TimeInForce? timeInForce = null, decimal stopPrice = 0, decimal icebergQty = 0, long recvWindow = BinanceApi.RecvWindowDefault, CancellationToken token = default)
        {
            var query = await GetTotalParamsWithSignatureAsync(user, symbol, side, type, quantity, price, newClientOrderId, timeInForce, stopPrice, icebergQty, recvWindow, token)
                .ConfigureAwait(false);

            return await PostAsync($"/api/v3/order/test?{query}", string.Empty, token, user)
                .ConfigureAwait(false);
        }

        public virtual async Task<string> GetOrderAsync(IBinanceUser user, string symbol, long orderId = 0, string origClientOrderId = null, long recvWindow = BinanceApi.RecvWindowDefault, CancellationToken token = default)
        {
            Throw.IfNull(user, nameof(user));

            symbol = ThrowIfSymbolIsInvalid(symbol);

            ThrowIfRecvWindowIsInvalid(recvWindow);

            var totalParams = $"symbol={symbol}&recvWindow={recvWindow}";

            if (orderId >= 0)
                totalParams += $"&orderId={orderId}";

            if (!string.IsNullOrWhiteSpace(origClientOrderId))
                totalParams += $"&origClientOrderId={origClientOrderId}";

            var timestamp = await GetTimestampAsync(token).ConfigureAwait(false);

            totalParams += $"&timestamp={timestamp}";

            var signature = user.Sign(totalParams);

            return await GetAsync($"/api/v3/order?{totalParams}&signature={signature}", token, user)
                .ConfigureAwait(false);
        }

        public virtual async Task<string> CancelOrderAsync(IBinanceUser user, string symbol, long orderId = BinanceApi.NullId, string origClientOrderId = null, string newClientOrderId = null, long recvWindow = BinanceApi.RecvWindowDefault, CancellationToken token = default)
        {
            Throw.IfNull(user, nameof(user));

            symbol = ThrowIfSymbolIsInvalid(symbol);

            ThrowIfRecvWindowIsInvalid(recvWindow);

            var totalParams = $"symbol={symbol}&recvWindow={recvWindow}";

            if (orderId >= 0)
                totalParams += $"&orderId={orderId}";

            if (!string.IsNullOrWhiteSpace(origClientOrderId))
                totalParams += $"&origClientOrderId={origClientOrderId}";

            if (!string.IsNullOrWhiteSpace(newClientOrderId))
                totalParams += $"&newClientOrderId={newClientOrderId}";

            var timestamp = await GetTimestampAsync(token).ConfigureAwait(false);

            totalParams += $"&timestamp={timestamp}";

            var signature = user.Sign(totalParams);

            return await DeleteAsync($"/api/v3/order?{totalParams}&signature={signature}", token, user)
                .ConfigureAwait(false);
        }

        public virtual async Task<string> GetOpenOrdersAsync(IBinanceUser user, string symbol, long recvWindow = BinanceApi.RecvWindowDefault, CancellationToken token = default)
        {
            Throw.IfNull(user, nameof(user));

            symbol = ThrowIfSymbolIsInvalid(symbol);

            ThrowIfRecvWindowIsInvalid(recvWindow);

            var totalParams = $"symbol={symbol}&recvWindow={recvWindow}";

            var timestamp = await GetTimestampAsync(token).ConfigureAwait(false);

            totalParams += $"&timestamp={timestamp}";

            var signature = user.Sign(totalParams);

            return await GetAsync($"/api/v3/openOrders?{totalParams}&signature={signature}", token, user)
                .ConfigureAwait(false);
        }

        public virtual async Task<string> GetOrdersAsync(IBinanceUser user, string symbol, long orderId = BinanceApi.NullId, int limit = BinanceApi.OrdersLimitDefault, long recvWindow = BinanceApi.RecvWindowDefault, CancellationToken token = default)
        {
            Throw.IfNull(user, nameof(user));

            symbol = ThrowIfSymbolIsInvalid(symbol);

            ThrowIfRecvWindowIsInvalid(recvWindow);

            if (limit < 1 || limit > BinanceApi.OrdersLimitMax)
                throw new ArgumentException($"{nameof(BinanceJsonApi)}.{nameof(GetOrdersAsync)} limit must be in the range [1-{BinanceApi.OrdersLimitMax}].", nameof(limit));

            var totalParams = $"symbol={symbol}&limit={limit}&recvWindow={recvWindow}";

            if (orderId >= 0)
                totalParams += $"&orderId={orderId}";

            var timestamp = await GetTimestampAsync(token).ConfigureAwait(false);

            totalParams += $"&timestamp={timestamp}";

            var signature = user.Sign(totalParams);

            return await GetAsync($"/api/v3/allOrders?{totalParams}&signature={signature}", token, user)
                .ConfigureAwait(false);
        }

        public virtual async Task<string> GetAccountAsync(IBinanceUser user, long recvWindow = BinanceApi.RecvWindowDefault, CancellationToken token = default)
        {
            Throw.IfNull(user, nameof(user));

            ThrowIfRecvWindowIsInvalid(recvWindow);

            var timestamp = await GetTimestampAsync(token).ConfigureAwait(false);

            var totalParams = $"recvWindow={recvWindow}&timestamp={timestamp}";

            var signature = user.Sign(totalParams);

            return await GetAsync($"/api/v3/account?{totalParams}&signature={signature}", token, user);
        }

        public virtual async Task<string> GetTradesAsync(IBinanceUser user, string symbol, int limit = BinanceApi.OrdersLimitDefault, long fromId = BinanceApi.NullId, long recvWindow = BinanceApi.RecvWindowDefault, CancellationToken token = default)
        {
            Throw.IfNull(user, nameof(user));

            symbol = ThrowIfSymbolIsInvalid(symbol);

            ThrowIfRecvWindowIsInvalid(recvWindow);

            if (limit < 1 || limit > BinanceApi.TradesLimitMax)
                throw new ArgumentException($"{nameof(BinanceJsonApi)}.{nameof(GetOrdersAsync)} limit must be in the range [1-{BinanceApi.TradesLimitMax}].", nameof(limit));

            var totalParams = $"symbol={symbol}&limit={limit}&recvWindow={recvWindow}";

            if (fromId >= 0)
                totalParams += $"&fromId={fromId}";

            var timestamp = await GetTimestampAsync(token).ConfigureAwait(false);

            totalParams += $"&timestamp={timestamp}";

            var signature = user.Sign(totalParams);

            return await GetAsync($"/api/v3/myTrades?{totalParams}&signature={signature}", token, user)
                .ConfigureAwait(false);
        }

        public virtual async Task<string> WithdrawAsync(IBinanceUser user, string asset, string address, decimal amount, string name, long recvWindow = BinanceApi.RecvWindowDefault, CancellationToken token = default)
        {
            Throw.IfNull(user, nameof(user));

            asset = ThrowIfSymbolIsInvalid(asset);

            Throw.IfNullOrWhiteSpace(address, nameof(address));

            if (amount <= 0)
                throw new ArgumentException($"Withdraw amount must be greater than 0.", nameof(amount));

            ThrowIfRecvWindowIsInvalid(recvWindow);

            var totalParams = $"asset={asset}&address={address}&amount={amount}&recvWindow={recvWindow}";

            if (!string.IsNullOrWhiteSpace(name))
                totalParams += $"&name={name}";

            var timestamp = await GetTimestampAsync(token).ConfigureAwait(false);

            totalParams += $"&timestamp={timestamp}";

            var signature = user.Sign(totalParams);

            return await PostAsync($"/wapi/v1/withdraw.html?{totalParams}&signature={signature}", null, token, user)
                .ConfigureAwait(false);
        }

        public virtual async Task<string> GetDepositsAsync(IBinanceUser user, string asset, DepositStatus? status = null, long startTime = 0, long endTime = 0, long recvWindow = BinanceApi.RecvWindowDefault, CancellationToken token = default)
        {
            Throw.IfNull(user, nameof(user));

            ThrowIfRecvWindowIsInvalid(recvWindow);

            var totalParams = $"recvWindow={recvWindow}";

            if (!string.IsNullOrWhiteSpace(asset))
            {
                asset = ThrowIfSymbolIsInvalid(asset);
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

            var timestamp = await GetTimestampAsync(token).ConfigureAwait(false);

            totalParams += $"&timestamp={timestamp}";

            var signature = user.Sign(totalParams);

            return await PostAsync($"/wapi/v1/getDepositHistory.html?{totalParams}&signature={signature}", null, token, user)
                .ConfigureAwait(false);
        }

        public virtual async Task<string> GetWithdrawalsAsync(IBinanceUser user, string asset, WithdrawalStatus? status = null, long startTime = 0, long endTime = 0, long recvWindow = BinanceApi.RecvWindowDefault, CancellationToken token = default)
        {
            Throw.IfNull(user, nameof(user));

            ThrowIfRecvWindowIsInvalid(recvWindow);

            var totalParams = $"recvWindow={recvWindow}";

            if (!string.IsNullOrWhiteSpace(asset))
            {
                asset = ThrowIfSymbolIsInvalid(asset);
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

            var timestamp = await GetTimestampAsync(token).ConfigureAwait(false);

            totalParams += $"&timestamp={timestamp}";

            var signature = user.Sign(totalParams);

            return await PostAsync($"/wapi/v1/getWithdrawHistory.html?{totalParams}&signature={signature}", null, token, user)
                .ConfigureAwait(false);
        }

        #endregion Account

        #region User Stream

        public virtual Task<string> UserStreamStartAsync(IBinanceUser user, CancellationToken token = default)
        {
            Throw.IfNull(user, nameof(user));

            return PostAsync($"/api/v1/userDataStream", string.Empty, token, user);
        }

        public virtual Task<string> UserStreamKeepAliveAsync(IBinanceUser user, string listenKey, CancellationToken token = default)
        {
            Throw.IfNull(user, nameof(user));
            Throw.IfNullOrWhiteSpace(listenKey, nameof(listenKey));

            return PutAsync($"/api/v1/userDataStream?listenKey={listenKey}", string.Empty, token, user);
        }

        public virtual Task<string> UserStreamCloseAsync(IBinanceUser user, string listenKey, CancellationToken token = default)
        {
            Throw.IfNull(user, nameof(user));
            Throw.IfNullOrWhiteSpace(listenKey, nameof(listenKey));

            return DeleteAsync($"/api/v1/userDataStream?listenKey={listenKey}", token, user);
        }

        #endregion User Stream

        #region Private Methods

        /// <summary>
        /// Get new order query string.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="symbol"></param>
        /// <param name="side"></param>
        /// <param name="type"></param>
        /// <param name="quantity"></param>
        /// <param name="price"></param>
        /// <param name="newClientOrderId"></param>
        /// <param name="timeInForce"></param>
        /// <param name="stopPrice"></param>
        /// <param name="icebergQty"></param>
        /// <param name="recvWindow"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task<string> GetTotalParamsWithSignatureAsync(IBinanceUser user, string symbol, OrderSide side, OrderType type, decimal quantity, decimal price, string newClientOrderId = null, TimeInForce? timeInForce = null, decimal stopPrice = 0, decimal icebergQty = 0, long recvWindow = BinanceApi.RecvWindowDefault, CancellationToken token = default)
        {
            Throw.IfNull(user, nameof(user));

            symbol = ThrowIfSymbolIsInvalid(symbol);

            if (quantity <= 0)
                throw new ArgumentException($"Order quantity must be greater than 0.", nameof(quantity));

            ThrowIfRecvWindowIsInvalid(recvWindow);

            var totalParams = $"symbol={symbol}&side={side.ToString().ToUpper()}&type={type.ToString().ToUpper()}&quantity={quantity}";

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

            var timestamp = await GetTimestampAsync(token).ConfigureAwait(false);

            totalParams += $"&recvWindow={recvWindow}&timestamp={timestamp}";

            var signature = user.Sign(totalParams);

            return $"{totalParams}&signature={signature}";
        }

        /// <summary>
        /// Get local system timestamp synchronized with server time.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task<long> GetTimestampAsync(CancellationToken token = default)
        {
            if (DateTime.UtcNow - _timestampOffsetUpdatedAt > TimeSpan.FromHours(1))
            {
                var systemTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                var json = await GetServerTimeAsync(token).ConfigureAwait(false);

                var jObject = JObject.Parse(json);

                // Calculate timestamp offset to account for time differences and delays.
                _timestampOffset = jObject["serverTime"].Value<long>() - systemTime;

                // Record the current system time to determine when to refresh offset.
                _timestampOffsetUpdatedAt = DateTime.UtcNow;
            }

            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + _timestampOffset;
        }

        private string ThrowIfSymbolIsInvalid(string symbol)
        {
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            return symbol.FixSymbol();
        }

        private void ThrowIfRecvWindowIsInvalid(long recvWindow)
        {
            if (recvWindow < 1)
                throw new ArgumentException($"Receive window must be greater than 0.", nameof(recvWindow));
        }

        private Task<string> GetAsync(string requestPath, CancellationToken token, IBinanceUser user = null)
        {
            return RequestAsync(HttpMethod.Get, requestPath, null, user, false, token);
        }

        private Task<string> PostAsync(string requestPath, string body, CancellationToken token, IBinanceUser user = null, bool bypassDelay = false)
        {
            return RequestAsync(HttpMethod.Post, requestPath, body, user, bypassDelay, token);
        }

        private Task<string> PutAsync(string requestPath, string body, CancellationToken token, IBinanceUser user = null, bool bypassDelay = false)
        {
            return RequestAsync(HttpMethod.Put, requestPath, body, user, bypassDelay, token);
        }

        private Task<string> DeleteAsync(string requestPath, CancellationToken token, IBinanceUser user = null, bool bypassDelay = false)
        {
            return RequestAsync(HttpMethod.Delete, requestPath, null, user, bypassDelay, token);
        }

        private async Task<string> RequestAsync(HttpMethod method, string requestPath, string body = null, IBinanceUser user = null, bool bypassDelay = false, CancellationToken token = default)
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
                await RateLimiter.DelayAsync()
                    .ConfigureAwait(false);
            }

            using (var response = await _httpClient.SendAsync(request, token).ConfigureAwait(false))
            {
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadAsStringAsync()
                        .ConfigureAwait(false);
                }
                else if (response.StatusCode == HttpStatusCode.GatewayTimeout)
                {
                    throw new BinanceUnknownStatusException();
                }
                else
                {
                    var error = await response.Content.ReadAsStringAsync()
                        .ConfigureAwait(false);

                    int errorCode = 0;
                    string errorMessage = null;

                    if (!string.IsNullOrWhiteSpace(error))
                    {
                        try // to parse server error response.
                        {
                            var jObject = JObject.Parse(error);

                            errorCode = jObject["code"]?.Value<int>() ?? 0;
                            errorMessage = jObject["msg"]?.Value<string>() ?? null;
                        }
                        catch { }
                    }

                    throw new BinanceHttpException(response.StatusCode, response.ReasonPhrase, errorCode, errorMessage);
                }
            }
        }

        #endregion Private Methods

        #region IDisposable

        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
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
