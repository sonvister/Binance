using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Binance.Stream
{
    /// <summary>
    /// A streaming JSON provider.
    /// </summary>
    public interface IJsonStream : IJsonProvider
    {
        /// <summary>
        /// Get the flag indicating if the stream is active.
        /// </summary>
        bool IsStreaming { get; }

        /// <summary>
        /// Get the subscribed stream names.
        /// </summary>
        IEnumerable<string> ProvidedStreams { get; }

        /// <summary>
        /// Subscribe an <see cref="IJsonStreamObserver"/> to a stream.
        /// </summary>
        /// <param name="observer"></param>
        /// <param name="streamNames"></param>
        void Subscribe(IJsonStreamObserver observer, params string[] streamNames);

        /// <summary>
        /// Unsubscribe an <see cref="IJsonStreamObserver"/> from a stream.
        /// </summary>
        /// <param name="observer"></param>
        /// <param name="streamNames"></param>
        void Unsubscribe(IJsonStreamObserver observer, params string[] streamNames);

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
