using System.Net;

// ReSharper disable once CheckNamespace
namespace Binance
{
    /// <summary>
    /// Binance unknown status exception.
    /// 
    /// Thrown when the API successfully sent a request but not get a response
    /// within the timeout period (HTTP 504 return code). It is important to
    /// NOT treat this as a failure; the execution status is UNKNOWN and
    /// could have been a success.
    /// </summary>
    public sealed class BinanceUnknownStatusException : BinanceHttpException
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        public BinanceUnknownStatusException(HttpStatusCode statusCode)
            : base(statusCode, null, 0, "It is important to NOT treat this as a failure operation; the execution status is UNKNOWN and could have been a success.")
        { }

        #endregion Constructors
    }
}
