using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace Binance.Api
{
    public sealed class BinanceHttpClient : IBinanceHttpClient
    {
        #region Public Constants

        /// <summary>
        /// Get the base endpoint URL.
        /// </summary>
        public static readonly string EndpointUrl = "https://www.binance.com";

        /// <summary>
        /// Get the successful test response string.
        /// </summary>
        public static readonly string SuccessfulTestResponse = "{}";

        #endregion Public Constants

        #region Public Properties

        public ITimestampProvider TimestampProvider { get; }

        public IApiRateLimiter RateLimiter { get; }

        public BinanceApiOptions Options { get; }

        #endregion Public Properties

        #region Internal

        /// <summary>
        /// Singleton.
        /// </summary>
        public static BinanceHttpClient Instance => Initializer.Value;

        /// <summary>
        /// Lazy initializer.
        /// </summary>
        internal static Lazy<BinanceHttpClient> Initializer
            = new Lazy<BinanceHttpClient>(() => new BinanceHttpClient(), true);

        #endregion Internal

        #region Private Fields

        private readonly HttpClient _httpClient;

        private readonly ILogger<BinanceHttpClient> _logger;

        #endregion Private Fields

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="timestampProvider">The timestamp provider.</param>
        /// <param name="rateLimiter">The rate limiter (auto configured).</param>
        /// <param name="options">The options.</param>
        /// <param name="logger">The logger.</param>
        internal BinanceHttpClient(ITimestampProvider timestampProvider = null, IApiRateLimiter rateLimiter = null, IOptions<BinanceApiOptions> options = null, ILogger<BinanceHttpClient> logger = null)
        {
            TimestampProvider = timestampProvider ?? new TimestampProvider();
            RateLimiter = rateLimiter ?? new ApiRateLimiter();
            Options = options?.Value ?? new BinanceApiOptions();
            _logger = logger;

            // Configure request rate limiter.
            RateLimiter.Configure(TimeSpan.FromMinutes(Options.RequestRateLimit.DurationMinutes), Options.RequestRateLimit.Count);
            // Configure request burst rate limiter.
            RateLimiter.Configure(TimeSpan.FromSeconds(Options.RequestRateLimit.BurstDurationSeconds), Options.RequestRateLimit.BurstCount);

            _httpClient = new HttpClient
            {
                BaseAddress = new Uri(EndpointUrl)
            };

            var version = GetType().Assembly.GetName().Version;

            var versionString = $"{version.Major}.{version.Minor}.{version.Build}{(version.Revision > 0 ? $".{version.Revision}" : string.Empty)}";

            _httpClient.DefaultRequestHeaders.Add("User-Agent", $"Binance/{versionString} (.NET; +https://github.com/sonvister/Binance)");
        }

        #endregion Constructors

        #region Public Methods

        public async Task SignAsync(BinanceHttpRequest request, IBinanceApiUser user, CancellationToken token = default)
        {
            Throw.IfNull(request, nameof(request));
            Throw.IfNull(user, nameof(user));

            var timestamp =
                await TimestampProvider.GetTimestampAsync(this, token)
                    .ConfigureAwait(false);

            request.AddParameter("timestamp", timestamp);

            var signature = user.Sign(request.TotalParams);

            request.AddParameter("signature", signature);
        }

        public Task<string> GetAsync(string path, CancellationToken token = default)
            => GetAsync(new BinanceHttpRequest(path), token);

        public Task<string> GetAsync(BinanceHttpRequest request, CancellationToken token = default, IApiRateLimiter rateLimiter = null)
        {
            return RequestAsync(HttpMethod.Get, request, token, rateLimiter);
        }

        public Task<string> PostAsync(BinanceHttpRequest request, CancellationToken token = default, IApiRateLimiter rateLimiter = null)
        {
            return RequestAsync(HttpMethod.Post, request, token, rateLimiter);
        }

        public Task<string> PutAsync(BinanceHttpRequest request, CancellationToken token = default, IApiRateLimiter rateLimiter = null)
        {
            return RequestAsync(HttpMethod.Put, request, token, rateLimiter);
        }

        public Task<string> DeleteAsync(BinanceHttpRequest request, CancellationToken token = default, IApiRateLimiter rateLimiter = null)
        {
            return RequestAsync(HttpMethod.Delete, request, token, rateLimiter);
        }

        #endregion Public Methods

        #region Private Methods

        private async Task<string> RequestAsync(HttpMethod method, BinanceHttpRequest request, CancellationToken token = default, IApiRateLimiter rateLimiter = null)
        {
            Throw.IfNull(request, nameof(request));

            token.ThrowIfCancellationRequested();

            var requestMessage = request.CreateMessage(method);

            _logger?.LogDebug($"{nameof(BinanceHttpClient)}.{nameof(RequestAsync)}: [{method.Method}] \"{requestMessage.RequestUri}\"");

            await (rateLimiter ?? RateLimiter).DelayAsync(request.RateLimitWeight, token)
                .ConfigureAwait(false);

            using (var response = await _httpClient.SendAsync(requestMessage, token).ConfigureAwait(false))
            {
                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync()
                        .ConfigureAwait(false);

                    _logger?.LogDebug($"{nameof(BinanceHttpClient)}: \"{json}\"");

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
                        _logger?.LogError(e, $"{nameof(BinanceHttpClient)}.{nameof(RequestAsync)} failed to parse server error response: \"{error}\"");
                    }
                }

                switch (response.StatusCode)
                {
                    case (HttpStatusCode)429:
                        throw new BinanceRequestRateLimitExceededException(response.ReasonPhrase, errorCode, errorMessage);
                    case (HttpStatusCode)418:
                        throw new BinanceRequestRateLimitIpBanException(response.ReasonPhrase, errorCode, errorMessage);
                    default:
                        throw new BinanceHttpException(response.StatusCode, response.ReasonPhrase, errorCode, errorMessage);
                }
            }
        }

        #endregion Private Methods

        #region IDisposable

        private bool _disposed;

        private void Dispose(bool disposing)
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
