using System.Threading;
using System.Threading.Tasks;

namespace Binance.Api
{
    public interface ITimestampProvider
    {
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
