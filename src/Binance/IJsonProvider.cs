using System;

namespace Binance
{
    /// <summary>
    /// A JSON message provider.
    /// </summary>
    public interface IJsonProvider
    {
        /// <summary>
        /// The message event.
        /// </summary>
        event EventHandler<JsonMessageEventArgs> Message;
    }
}
