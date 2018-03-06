using System.Collections.Generic;
using Binance.Client;

namespace Binance.Producer
{
    /// <summary>
    /// A JSON publisher.
    /// </summary>
    public interface IJsonPublisher : IJsonProducer
    {
        /// <summary>
        /// Get the subscribed stream names.
        /// </summary>
        IEnumerable<string> PublishedStreams { get; }

        /// <summary>
        /// Subscribe an <see cref="IJsonSubscriber"/> to a stream.
        /// </summary>
        /// <param name="subscriber"></param>
        /// <param name="streamNames"></param>
        IJsonPublisher Subscribe(IJsonSubscriber subscriber, params string[] streamNames);

        /// <summary>
        /// Unsubscribe an <see cref="IJsonSubscriber"/> from a stream.
        /// </summary>
        /// <param name="subscriber"></param>
        /// <param name="streamNames"></param>
        IJsonPublisher Unsubscribe(IJsonSubscriber subscriber, params string[] streamNames);
    }
}
