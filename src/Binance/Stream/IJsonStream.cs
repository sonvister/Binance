using System.Threading;
using System.Threading.Tasks;

namespace Binance.Stream
{
    /// <summary>
    /// A streaming JSON producer.
    /// </summary>
    public interface IJsonStream : IJsonProducer
    {
        /// <summary>
        /// Get the flag indicating if the stream is active.
        /// </summary>
        bool IsStreaming { get; }

        /// <summary>
        /// Initiate data streaming and begin sending messages to observers.
        /// Runtime exceptions are thrown by this method and must be handled
        /// by the caller, otherwise the <see cref="Task"/> will continue
        /// processing messages until the token is canceled.
        /// </summary>
        /// <param name="token">The cancellation token (used to abort streaming).</param>
        /// <returns></returns>
        Task StreamAsync(CancellationToken token);
    }
}
