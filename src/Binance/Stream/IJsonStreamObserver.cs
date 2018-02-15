using System.Threading;
using System.Threading.Tasks;

namespace Binance.Stream
{
    /// <summary>
    /// An observer of an <see cref="IJsonStream"/>.
    /// </summary>
    public interface IJsonStreamObserver
    {
        /// <summary>
        /// Handle a JSON message event.
        /// </summary>
        /// <param name="stream">The stream (source) identifier.</param>
        /// <param name="json">The JSON message data.</param>
        /// <param name="token">The cancellation token.</param>
        /// <returns></returns>
        Task HandleMessageAsync(string stream, string json, CancellationToken token = default);
    }
}
