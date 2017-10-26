using System.Net;

namespace Binance.Api
{
    /// <summary>
    /// Binance HTTP exception.
    /// </summary>
    public class BinanceHttpException : BinanceApiException
    {
        #region Public Properties

        /// <summary>
        /// Get the HTTP response status code.
        /// </summary>
        public HttpStatusCode StatusCode { get; private set; }

        /// <summary>
        /// Get the HTTP respone reason phrase.
        /// </summary>
        public string ReasonPhrase { get; private set; }

        /// <summary>
        /// Get the endpoint error code.
        /// </summary>
        public int ErrorCode { get; private set; }

        /// <summary>
        /// Get the endpoint error message.
        /// </summary>
        public string ErrorMessage { get; private set; }

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="statusCode">The HTTP status code.</param>
        /// <param name="reasonPhrase">The HTTP response reason.</param>
        public BinanceHttpException(HttpStatusCode statusCode, string reasonPhrase)
            : this(statusCode, reasonPhrase, 0, null)
        { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="statusCode">The HTTP status code.</param>
        /// <param name="reasonPhrase">The HTTP response reason.</param>
        /// <param name="errorCode">The server ERROR code.</param>
        /// <param name="errorMessage">The server ERROR message.</param>
        public BinanceHttpException(HttpStatusCode statusCode, string reasonPhrase, int errorCode, string errorMessage)
            : base(FormatErrorMessage(statusCode, reasonPhrase, errorCode, errorMessage))
        {
            StatusCode = statusCode;
            ReasonPhrase = reasonPhrase;
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }

        #endregion Constructors

        #region Private Methods

        /// <summary>
        /// Format the error message.
        /// </summary>
        /// <param name="statusCode">The HTTP status code.</param>
        /// <param name="reasonPhrase">The HTTP response reason.</param>
        /// <param name="errorCode">The server ERROR code.</param>
        /// <param name="errorMessage">The server ERROR message.</param>
        /// <returns></returns>
        private static string FormatErrorMessage(HttpStatusCode statusCode, string reasonPhrase, int errorCode, string errorMessage)
        {
            reasonPhrase = !string.IsNullOrWhiteSpace(reasonPhrase) ? reasonPhrase : "[NO REASON]";

            errorMessage = !string.IsNullOrWhiteSpace(errorMessage) ? $" {errorMessage}" : " [NO MSG]";

            return $"[{statusCode}]: '{reasonPhrase}' -{errorMessage}{(errorCode != 0 ? $" ({errorCode})" : " [NO CODE]")}";
        }

        #endregion Private Methods
    }
}
