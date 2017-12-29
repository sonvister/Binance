// ReSharper disable once CheckNamespace
using System.Net;

// ReSharper disable once CheckNamespace
namespace Binance.Api
{
    /// <summary>
    /// Binance request rate limit exceeded exception.
    /// 
    /// HTTP 418 return code is used when an IP has been auto-banned for continuing to send requests after receiving 429 codes.
    /// Repeatedly violating rate limits and/or failing to back off after receiving 429s will result in an automated IP ban (http status 418).
    /// </summary>
    public sealed class BinanceRequestRateLimitIpBanException : BinanceHttpException
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public BinanceRequestRateLimitIpBanException(string reasonPhrase, int errorCode, string errorMessage)
            : base((HttpStatusCode)418, reasonPhrase, errorCode, errorMessage)
        { }

        #endregion Constructors
    }
}
