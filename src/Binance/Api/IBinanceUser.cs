using System;

namespace Binance.Api
{
    public interface IBinanceUser : IDisposable
    {
        #region Public Properties

        /// <summary>
        /// API key.
        /// </summary>
        string ApiKey { get; }

        /// <summary>
        /// Sign HTTP request parameters (query string concatenated with the request body).
        /// </summary>
        /// <param name="totalParams"></param>
        /// <returns></returns>
        string Sign(string totalParams);

        #endregion Public Properties
    }
}
