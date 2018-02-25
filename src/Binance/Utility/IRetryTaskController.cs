using System;

namespace Binance.Utility
{
    public interface IRetryTaskController : ITaskController
    {
        /// <summary>
        /// The pausing event.
        /// </summary>
        event EventHandler<PausingEventArgs> Pausing;

        /// <summary>
        /// The resuming event.
        /// </summary>
        event EventHandler<EventArgs> Resuming;

        /// <summary>
        /// Get or set the retry delay (milliseconds).
        /// </summary>
        int RetryDelayMilliseconds { get; set; }
    }
}
