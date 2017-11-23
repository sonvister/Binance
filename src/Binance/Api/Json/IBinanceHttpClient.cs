using System;
using System.Threading;
using System.Threading.Tasks;

namespace Binance.Api.Json
{
    public interface IBinanceHttpClient : IDisposable
    {
        /// <summary>
        /// Get request.
        /// </summary>
        /// <param name="requestPath"></param>
        /// <param name="user"></param>
        /// <param name="rateLimiter"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<string> GetAsync(string requestPath, IBinanceApiUser user = null, IApiRateLimiter rateLimiter = null, CancellationToken token = default);

        /// <summary>
        /// Post request.
        /// </summary>
        /// <param name="requestPath"></param>
        /// <param name="body"></param>
        /// <param name="user"></param>
        /// <param name="rateLimiter"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<string> PostAsync(string requestPath, string body, IBinanceApiUser user = null, IApiRateLimiter rateLimiter = null, CancellationToken token = default);

        /// <summary>
        /// Put request.
        /// </summary>
        /// <param name="requestPath"></param>
        /// <param name="body"></param>
        /// <param name="user"></param>
        /// <param name="rateLimiter"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<string> PutAsync(string requestPath, string body, IBinanceApiUser user = null, IApiRateLimiter rateLimiter = null, CancellationToken token = default);

        /// <summary>
        /// Delete request.
        /// </summary>
        /// <param name="requestPath"></param>
        /// <param name="user"></param>
        /// <param name="rateLimiter"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<string> DeleteAsync(string requestPath, IBinanceApiUser user = null, IApiRateLimiter rateLimiter = null, CancellationToken token = default);
    }
}
