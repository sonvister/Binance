using System;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace Binance.Api
{
    internal sealed class TimestampProvider : ITimestampProvider
    {
        #region Public Properties

        public TimeSpan TimestampOffsetRefreshPeriod { get; set; }

        public long TimestampOffset { get; private set; }

        #endregion Public Properties

        #region Private Fields

        private DateTime _offsetLastUpdate;

        private readonly SemaphoreSlim _syncLock = new SemaphoreSlim(1, 1);

        #endregion Private Fields

        #region Public Methods

        public async Task<long> GetTimestampAsync(IBinanceHttpClient client, CancellationToken token = default)
        {
            Throw.IfNull(client, nameof(client));

            // Acquire synchronization lock.
            await _syncLock.WaitAsync(token)
                .ConfigureAwait(false);

            try
            {
                if (DateTime.UtcNow - _offsetLastUpdate > TimestampOffsetRefreshPeriod)
                {
                    const long n = 3;

                    long sum = 0;
                    var count = n;
                    do
                    {
                        var systemTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                        var json = await client.GetServerTimeAsync(token)
                            .ConfigureAwait(false);

                        systemTime = (systemTime + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()) / 2;

                        // Calculate timestamp offset to account for time differences.
                        sum += JObject.Parse(json)["serverTime"].Value<long>() - systemTime;
                    } while (--count > 0);

                    // Calculate average offset.
                    TimestampOffset = sum / n;

                    // Record the current system time to determine when to refresh offset.
                    _offsetLastUpdate = DateTime.UtcNow;
                }
            }
            catch (Exception) { /* ignore */ }
            finally
            {
                // Release synchronization lock.
                _syncLock.Release();
            }

            return DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + TimestampOffset;
        }

        #endregion Public Methods
    }
}
