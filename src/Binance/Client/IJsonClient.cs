using System.Collections.Generic;
using Binance.Stream;

namespace Binance.Client
{
    /// <summary>
    /// A JSON stream observer client.
    /// </summary>
    public interface IJsonClient : IJsonStreamObserver
    {
        /// <summary>
        /// Get the observed streams.
        /// </summary>
        IEnumerable<string> ObservedStreams { get; }

        /// <summary>
        /// Unsubscribe all callbacks.
        /// </summary>
        void Unsubscribe();
    }
}
