using System;

namespace Binance.Utility
{
    /// <summary>
    /// The <see cref="IRetryTaskController"/> pausing event arguments.
    /// </summary>
    public class PausingEventArgs : EventArgs
    {
        /// <summary>
        /// Get the delay.
        /// </summary>
        public TimeSpan Delay { get; }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="delay"></param>
        public PausingEventArgs(TimeSpan delay)
        {
            Throw.IfNull(delay, nameof(delay));

            Delay = delay;
        }
    }
}
