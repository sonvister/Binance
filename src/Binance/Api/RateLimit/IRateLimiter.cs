using System;
using System.Threading;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace Binance
{
    public interface IRateLimiter : IDisposable
    {
        #region Properties

        /// <summary>
        /// The count.
        /// </summary>
        int Count { get; set; }

        /// <summary>
        /// The duration.
        /// </summary>
        TimeSpan Duration { get; set; }

        #endregion Properties

        #region Methods

        /// <summary>
        /// Delay if count has been exceeded within time duration.
        /// </summary>
        /// <param name="count"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task DelayAsync(int count = 1, CancellationToken token = default);

        #endregion Methods
    }
}
