using Binance.Account.Orders;
using Binance.Market;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
namespace Binance.Api
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
            Throw.IfNull(api, nameof(api));

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
            Throw.IfNull(api, nameof(api));

            return (await api.GetPricesAsync(token).ConfigureAwait(false))
                .Select(p => p.Symbol);
        }

        /// <summary>
        /// Get the most recent trades from trade (EXCLUSIVE).
        /// </summary>
        /// <param name="api"></param>
        /// <param name="trade"></param>
        /// <param name="limit"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task<IEnumerable<AggregateTrade>> GetAggregateTradesAsync(this IBinanceApi api, AggregateTrade trade, int limit = default, CancellationToken token = default)
        {
            Throw.IfNull(api, nameof(api));
            Throw.IfNull(trade, nameof(trade));

            return api.GetAggregateTradesFromAsync(trade.Symbol, trade.Id + 1, limit, token);
        }

        /// <summary>
        /// Get aggregate/compressed trades within a time range (INCLUSIVE).
        /// </summary>
        /// <param name="api"></param>
        /// <param name="symbol"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task<IEnumerable<AggregateTrade>> GetAggregateTradesAsync(this IBinanceApi api, string symbol, DateTime startTime, DateTime endTime, CancellationToken token = default)
        {
            Throw.IfNull(api, nameof(api));

            if (startTime.Kind != DateTimeKind.Utc)
                throw new ArgumentException("Date/Time must be UTC.", nameof(startTime));
            if (endTime.Kind != DateTimeKind.Utc)
                throw new ArgumentException("Date/Time must be UTC.", nameof(endTime));

            return api.GetAggregateTradesInAsync(symbol, new DateTimeOffset(startTime).ToUnixTimeMilliseconds(), new DateTimeOffset(endTime).ToUnixTimeMilliseconds(), token);
        }

        /// <summary>
        /// Get candlesticks for a symbol using string interval.
        /// </summary>
        /// <param name="api"></param>
        /// <param name="symbol"></param>
        /// <param name="interval"></param>
        /// <param name="limit"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task<IEnumerable<Candlestick>> GetCandlesticksAsync(this IBinanceApi api, string symbol, string interval, int limit = default, long startTime = default, long endTime = default, CancellationToken token = default)
        {
            Throw.IfNull(api, nameof(api));

            return api.GetCandlesticksAsync(symbol, interval.ToCandlestickInterval(), limit, startTime, endTime, token);
        }

        /// <summary>
        /// Get candlesticks for a symbol within a time range with optional limit.
        /// </summary>
        /// <param name="api"></param>
        /// <param name="symbol"></param>
        /// <param name="interval"></param>
        /// <param name="startTime"></param>
        /// <param name="endTime"></param>
        /// <param name="limit">The maximum number of candles to return beginning from start time (optional).</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task<IEnumerable<Candlestick>> GetCandlesticksAsync(this IBinanceApi api, string symbol, CandlestickInterval interval, DateTime startTime, DateTime endTime, int limit = default, CancellationToken token = default)
        {
            return api.GetCandlesticksAsync(symbol, interval, limit, new DateTimeOffset(startTime).ToUnixTimeMilliseconds(), new DateTimeOffset(endTime).ToUnixTimeMilliseconds(), token);
        }

        /// <summary>
        /// Cancel an order.
        /// </summary>
        /// <param name="api"></param>
        /// <param name="order"></param>
        /// <param name="newClientOrderId"></param>
        /// <param name="recvWindow"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task<string> CancelAsync(this IBinanceApi api, Order order, string newClientOrderId = null, long recvWindow = default, CancellationToken token = default)
        {
            Throw.IfNull(order, nameof(order));

            // Cancel order using order ID.
            return api.CancelOrderAsync(order.User, order.Symbol, order.Id, newClientOrderId, recvWindow, token);
        }
    }
}
