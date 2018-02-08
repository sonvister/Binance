using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Binance.WebSocket
{
    public interface IJsonStream
    {
        /// <summary>
        /// Get the observers.
        /// </summary>
        IEnumerable<IJsonStreamObserver> Observers { get; }

        /// <summary>
        /// Attach an <see cref="IJsonStreamObserver"/>.
        /// </summary>
        /// <param name="observer"></param>
        void Attach(IJsonStreamObserver observer);

        /// <summary>
        /// Detach an <see cref="IJsonStreamObserver"/>.
        /// </summary>
        /// <param name="observer"></param>
        void Detach(IJsonStreamObserver observer);

        /// <summary>
        /// Detach all observers.
        /// </summary>
        void DetachAll();

        /// <summary>
        /// Get the flag indicating if the stream is active.
        /// </summary>
        bool IsStreaming { get; }

        /// <summary>
        /// Initiate data streaming and begin receiving messages by observers.
        /// Runtime exceptions are thrown by this method and must be handled
        /// by the caller, otherwise the <see cref="Task"/> will continue
        /// processing messages until the token is canceled.
        /// </summary>
        /// <param name="token">The cancellation token (required to abort streaming).</param>
        /// <returns></returns>
        Task StreamAsync(CancellationToken token);
    }
}
