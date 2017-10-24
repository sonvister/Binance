using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Binance
{
    public static class BinanceApiExtensions
    {
        /// <summary>
        /// Get current server time.
        /// </summary>
        /// <param name="api"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<DateTime> GetTimeAsync(this IBinanceApi api, CancellationToken token = default)
        {
            var timestamp = await api.GetTimestampAsync(token).ConfigureAwait(false);

            return DateTimeOffset.FromUnixTimeMilliseconds(timestamp).UtcDateTime;
        }

        /// <summary>
        /// Get all symbols.
        /// </summary>
        /// <param name="api"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<string>> SymbolsAsync(this IBinanceApi api, CancellationToken token = default)
        {
            return (await api.GetPricesAsync(token).ConfigureAwait(false))
                .Select(p => p.Symbol);
        }
    }
}
