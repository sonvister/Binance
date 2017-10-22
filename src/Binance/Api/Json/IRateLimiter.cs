using System;
using System.Threading;
using System.Threading.Tasks;

namespace Binance
{
    public interface IRateLimiter
    {
        #region Public Properties

        /// <summary>
        /// Get or set the rate limiting enabled flag.
        /// </summary>
        bool IsEnabled { get; set; }

        /// <summary>
        /// Get the maximum count allowed for duration.
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Get the time duration used to measure rate.
        /// </summary>
        TimeSpan Duration { get; }

        #endregion Public Properties

        #region Public Methods

        /// <summary>
        /// Set the maximum count and duration.
        /// Rate is limited to count/duration.
        /// </summary>
        /// <param name="count"></param>
        /// <param name="duration"></param>
        void Configure(int count, TimeSpan duration);

        /// <summary>
        /// Perform rate limiting delay.
        /// </summary>
        /// <returns></returns>
        Task DelayAsync(CancellationToken token = default);

        #endregion Public Methods
    }
}
