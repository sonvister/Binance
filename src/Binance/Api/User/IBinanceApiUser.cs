using System;

// ReSharper disable once CheckNamespace
namespace Binance
{
    public interface IBinanceApiUser : IDisposable
    {
        #region Public Properties

        /// <summary>
        /// Get the API key.
        /// </summary>
        string ApiKey { get; }

        /// <summary>
        /// Get or set the API request rate limiter.
        /// </summary>
        IApiRateLimiter RateLimiter { get; set; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Sign HTTP request parameters (query string concatenated with the request body).
        /// </summary>
        /// <param name="totalParams"></param>
        /// <returns></returns>
        string Sign(string totalParams);

        #endregion Public Methods
    }
}
