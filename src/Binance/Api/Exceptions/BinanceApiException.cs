using System;

namespace Binance.Api
{
    /// <summary>
    /// Binance API exception.
    /// </summary>
    public class BinanceApiException : Exception
    {
        #region Constructors

        public BinanceApiException()
            : base()
        { }

        public BinanceApiException(string message)
            : base(message)
        { }

        public BinanceApiException(string message, Exception innerException)
            : base(message, innerException)
        { }

        #endregion Constructors
    }
}
