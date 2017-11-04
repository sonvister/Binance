using System;

namespace Binance.Api
{
    public interface IBinanceApiUser : IDisposable
    {
        #region Public Properties

        /// <summary>
        /// API key.
        /// </summary>
        string ApiKey { get; }

        /// <summary>
        /// The API request rate limiter.
        /// </summary>
        IRateLimiter RateLimiter { get; set; }

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
