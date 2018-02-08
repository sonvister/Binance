using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Binance.WebSocket
{
    public interface IJsonStreamObserver
    {
        /// <summary>
        /// Get the observed streams.
        /// </summary>
        IEnumerable<string> ObservedStreams { get; }

        /// <summary>
        /// Handle JSON message events.
        /// </summary>
        /// <param name="stream">The stream identifier.</param>
        /// <param name="json">The JSON data.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns></returns>
        Task OnMessageAsync(string stream, string json, CancellationToken token = default);
    }
}
