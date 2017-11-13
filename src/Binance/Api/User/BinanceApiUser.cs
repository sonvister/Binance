using System;
using System.Security.Cryptography;
using System.Text;
using Binance.Api.Json;
using Microsoft.Extensions.Options;

// ReSharper disable once CheckNamespace
namespace Binance.Api
{
    /// <summary>
    /// Binance API user <see cref="IBinanceApiUser"/> implementation.
    /// </summary>
    public sealed class BinanceApiUser : IBinanceApiUser
    {
        #region Public Properties

        public string ApiKey { get; }

        public IApiRateLimiter RateLimiter { get; set; }

        #endregion Public Properties

        #region Private Fields

        private readonly HMAC _hmac;

        #endregion Private Fields

        #region Constructors

        /// <summary>
        /// Construct an <see cref="IBinanceApiUser"/> instance providing an API
        /// key and optional API secret. The API secret is not required for 
        /// the user stream methods, but is required for other account methods.
        /// </summary>
        /// <param name="apiKey">The user's API key.</param>
        /// <param name="apiSecret">The user's API secret (optional).</param>
        /// <param name="rateLimiter">The rate limiter (auto-configured).</param>
        /// <param name="options">The JSON API options.</param>
        public BinanceApiUser(string apiKey, string apiSecret = null, IApiRateLimiter rateLimiter = null, IOptions<BinanceJsonApiOptions> options = null)
        {
            Throw.IfNullOrWhiteSpace(apiKey, nameof(apiKey));

            ApiKey = apiKey;

            if (!string.IsNullOrWhiteSpace(apiSecret))
            {
                _hmac = new HMACSHA256(Encoding.UTF8.GetBytes(apiSecret));
            }

            RateLimiter = rateLimiter;

            // Configure order rate limiter.
            RateLimiter?.Configure(
                TimeSpan.FromDays(options?.Value.OrderRateLimitDurationDays ?? BinanceJsonApi.OrderRateLimitDurationDaysDefault),
                options?.Value.OrderRateLimitCount ?? BinanceJsonApi.OrderRateLimitCountDefault);

            // Configure order burst rate limiter.
            RateLimiter?.Configure(
                TimeSpan.FromSeconds(options?.Value.OrderRateLimitBurstDurationSeconds ?? BinanceJsonApi.OrderRateLimitBurstDurationSecondsDefault),
                options?.Value.OrderRateLimitBurstCount ?? BinanceJsonApi.OrderRateLimitBurstCountDefault);
        }

        #endregion Constructors

        #region Public Methods

        public string Sign(string query)
        {
            if (_hmac == null)
                throw new InvalidOperationException("Signature requires the user's API secret.");

            var hash = _hmac.ComputeHash(Encoding.UTF8.GetBytes(query));

            return BitConverter.ToString(hash).Replace("-", string.Empty);
        }

        #endregion Public Methods

        #region IDisposable

        private bool _disposed;

        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _hmac?.Dispose();
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}
