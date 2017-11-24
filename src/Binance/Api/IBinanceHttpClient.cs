using System;
using System.Threading;
using System.Threading.Tasks;

namespace Binance.Api
{
    public interface IBinanceHttpClient : IDisposable
    {
        /// <summary>
        /// Get the API request (default) rate limiter.
        /// </summary>
        IApiRateLimiter RateLimiter { get; }

        /// <summary>
        /// Get the options.
        /// </summary>
        BinanceApiOptions Options { get; }

        /// <summary>
        /// Get request.
        /// </summary>
        /// <param name="path"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<string> GetAsync(string path, CancellationToken token = default);

        /// <summary>
        /// Get request.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="token"></param>
        /// <param name="rateLimiter"></param>
        /// <returns></returns>
        Task<string> GetAsync(BinanceHttpRequest request, CancellationToken token = default, IApiRateLimiter rateLimiter = null);

        /// <summary>
        /// Post request.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="token"></param>
        /// <param name="rateLimiter"></param>
        /// <returns></returns>
        Task<string> PostAsync(BinanceHttpRequest request, CancellationToken token = default, IApiRateLimiter rateLimiter = null);

        /// <summary>
        /// Put request.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="token"></param>
        /// <param name="rateLimiter"></param>
        /// <returns></returns>
        Task<string> PutAsync(BinanceHttpRequest request, CancellationToken token = default, IApiRateLimiter rateLimiter = null);

        /// <summary>
        /// Delete request.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="token"></param>
        /// <param name="rateLimiter"></param>
        /// <returns></returns>
        Task<string> DeleteAsync(BinanceHttpRequest request, CancellationToken token = default, IApiRateLimiter rateLimiter = null);
    }
}
