using System.Threading;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace Binance.Api.Json
{
    public static class BinanceJsonApiExtensions
    {
        /// <summary>
        /// Extension allowing candlestick interval as a string.
        /// </summary>
        /// <param name="jsonApi"></param>
        /// <param name="symbol"></param>
        /// <param name="interval"></param>
        /// <param name="limit"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task<string> GetCandlesticksAsync(this IBinanceJsonApi jsonApi, string symbol, string interval, int limit = default, long startTime = default, long endTime = default, CancellationToken token = default)
        {
            Throw.IfNull(jsonApi, nameof(jsonApi));

            return jsonApi.GetCandlesticksAsync(symbol, interval.ToCandlestickInterval(), limit, startTime, endTime, token);
        }
    }
}
