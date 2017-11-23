using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Binance.Api.Json
{
    public sealed class BinanceHttpClient : IBinanceHttpClient
    {
        #region Public Constants

        public static readonly string EndpointUrl = "https://www.binance.com";

        #endregion Public Constants

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
        /// <param name="logger"></param>
        public BinanceHttpClient(ILogger<BinanceHttpClient> logger = null)
        {
            _logger = logger;

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

        public Task<string> GetAsync(string requestPath, IBinanceApiUser user = null, IApiRateLimiter rateLimiter = null, CancellationToken token = default)
        {
            return RequestAsync(HttpMethod.Get, requestPath, null, user, rateLimiter, token);
        }

        public Task<string> PostAsync(string requestPath, string body, IBinanceApiUser user = null, IApiRateLimiter rateLimiter = null, CancellationToken token = default)
        {
            return RequestAsync(HttpMethod.Post, requestPath, body, user, rateLimiter, token);
        }

        public Task<string> PutAsync(string requestPath, string body, IBinanceApiUser user = null, IApiRateLimiter rateLimiter = null, CancellationToken token = default)
        {
            return RequestAsync(HttpMethod.Put, requestPath, body, user, rateLimiter, token);
        }

        public Task<string> DeleteAsync(string requestPath, IBinanceApiUser user = null, IApiRateLimiter rateLimiter = null, CancellationToken token = default)
        {
            return RequestAsync(HttpMethod.Delete, requestPath, null, user, rateLimiter, token);
        }

        #endregion Public Methods

        #region Private Methods

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
