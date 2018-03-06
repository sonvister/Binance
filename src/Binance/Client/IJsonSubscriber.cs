using System.Collections.Generic;

namespace Binance.Client
{
    /// <summary>
    /// An observer of an <see cref="IJsonPublisher"/>.
    /// </summary>
    public interface IJsonSubscriber : IJsonConsumer
    {
        /// <summary>
        /// Get the observed streams.
        /// </summary>
        IEnumerable<string> SubscribedStreams { get; }

        /// <summary>
        /// Unsubscribe all streams (and callbacks).
        /// </summary>
        /// <returns></returns>
        IJsonSubscriber Unsubscribe();

        /// <summary>
        /// Handle a JSON message event.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="json"></param>
        void HandleMessage(string stream, string json);
    }
}
