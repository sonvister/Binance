using System;

namespace Binance
{
    /// <summary>
    /// A JSON message producer.
    /// </summary>
    public interface IJsonProducer
    {
        /// <summary>
        /// The message event.
        /// </summary>
        event EventHandler<JsonMessageEventArgs> Message;
    }
}
