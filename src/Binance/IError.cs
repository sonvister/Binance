using System;

namespace Binance
{
    public interface IError
    {
        /// <summary>
        /// The error event.
        /// </summary>
        event EventHandler<ErrorEventArgs> Error;
    }
}
