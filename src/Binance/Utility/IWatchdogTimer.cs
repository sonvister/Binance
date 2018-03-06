using System;

namespace Binance.Utility
{
    public interface IWatchdogTimer
    {
        /// <summary>
        /// Get or set the flag indicating if this timer is enabled.
        /// </summary>
        bool IsEnabled { get; set; }

        /// <summary>
        /// Get or set the timeout interval.
        /// </summary>
        TimeSpan Interval { get; set; }

        /// <summary>
        /// Reset the timer.
        /// </summary>
        void Kick();
    }
}
