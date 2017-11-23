using System;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace Binance.Api.Json
{
    public sealed class BinanceHttpClient : IBinanceHttpClient
    {
        #region Public Constants

        public static readonly string EndpointUrl = "https://www.binance.com";

        #endregion Public Constants

        #region Public Properties

        public IApiRateLimiter RateLimiter { get; }

        public BinanceJsonApiOptions Options { get; }

        #endregion Public Properties

        #region Private Constants

        private const string RequestHeaderKeyName = "X-MBX-APIKEY";

        #endregion Private Constants

        #region Private Fields

        private readonly HttpClient _httpClient;

        private readonly ILogger<BinanceHttpClient> _logger;

        #endregion Private Fields

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="rateLimiter">The rate limiter (auto configured).</param>
        /// <param name="options">The options.</param>
        /// <param name="logger">The logger.</param>
        public BinanceHttpClient(IApiRateLimiter rateLimiter = null, IOptions<BinanceJsonApiOptions> options = null, ILogger<BinanceHttpClient> logger = null)
        {
            RateLimiter = rateLimiter ?? new ApiRateLimiter();
            Options = options?.Value ?? new BinanceJsonApiOptions();
            _logger = logger;

            // Configure request rate limiter.
            RateLimiter.Configure(TimeSpan.FromMinutes(Options.RequestRateLimitDurationMinutes), Options.RequestRateLimitCount);
            // Configure request burst rate limiter.
            RateLimiter.Configure(TimeSpan.FromSeconds(Options.RequestRateLimitBurstDurationSeconds), Options.RequestRateLimitBurstCount);

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

            await (rateLimiter ?? RateLimiter).DelayAsync(token)
                .ConfigureAwait(false);

            using (var response = await _httpClient.SendAsync(requestMessage, token).ConfigureAwait(false))
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
