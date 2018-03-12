using System.Collections.Generic;

namespace Binance.Client
{
    /// <summary>
    /// An subscriber of a JSON publisher.
    /// </summary>
    public interface IJsonSubscriber : IJsonConsumer
    {
        /// <summary>
        /// Get the subscribed streams.
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
        /// <param name="streamName"></param>
        /// <param name="json"></param>
        void HandleMessage(string streamName, string json);
    }
}
