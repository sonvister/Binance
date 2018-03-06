using System;
using System.Threading;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace Binance
{
    public interface ITimestampProvider
    {
        /// <summary>
        /// Get or set the timestamp offset refresh period.
        /// </summary>
        TimeSpan TimestampOffsetRefreshPeriod { get; set; }

        /// <summary>
        /// Get the timestamp offset.
        /// </summary>
        long TimestampOffset { get; }

        /// <summary>
        /// Get a current timestamp (Unix time milliseconds).
        /// </summary>
        /// <param name="client"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<long> GetTimestampAsync(IBinanceHttpClient client, CancellationToken token = default);
    }
}
