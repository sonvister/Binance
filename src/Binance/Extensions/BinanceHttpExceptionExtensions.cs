using Binance.Api;

// ReSharper disable once CheckNamespace
namespace Binance
{
    /// <summary>
    /// HTTP exception extensions to help interpret status codes.
    /// </summary>
    public static class BinanceHttpExceptionExtensions
    {
        /// <summary>
        /// HTTP 1xx return codes indicate that the request was received and understood.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static bool IsInformational(this BinanceHttpException e)
        {
            return (int)e.StatusCode >= 100 && (int)e.StatusCode < 200;
        }

        /// <summary>
        /// HTTP 2xx return codes indicate the action requested by the client
        /// was received, understood, accepted, and processed successfully.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static bool IsSuccessful(this BinanceHttpException e)
        {
            return (int)e.StatusCode >= 200 && (int)e.StatusCode < 300;
        }

        /// <summary>
        /// HTTP 3xx return codes indicate the client must take additional
        /// action to complete the request.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static bool IsRedirection(this BinanceHttpException e)
        {
            return (int)e.StatusCode >= 300 && (int)e.StatusCode < 400;
        }

        /// <summary>
        /// HTTP 4xx return codes are used for for malformed requests; the issue is on the sender's side.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static bool IsClientError(this BinanceHttpException e)
        {
            return (int)e.StatusCode >= 400 && (int)e.StatusCode < 500;
        }

        /// <summary>
        /// HTTP 5xx return codes are used for internal errors; the issue is on Binance's side.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        public static bool IsServerError(this BinanceHttpException e)
        {
            return (int)e.StatusCode >= 500 && (int)e.StatusCode < 600;
        }
    }
}
