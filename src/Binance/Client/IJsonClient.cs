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
        /// Unsubscribe all streams (and callbacks).
        /// </summary>
        /// <returns></returns>
        IJsonClient Unsubscribe();
    }
}
