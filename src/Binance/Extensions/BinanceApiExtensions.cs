using Binance.Account.Orders;
using Binance.Market;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Binance.Account;

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
        /// Get older (non-compressed) trades.
        /// </summary>
        /// <param name="api"></param>
        /// <param name="user"></param>
        /// <param name="symbol"></param>
        /// <param name="fromId"></param>
        /// <param name="limit"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task<IEnumerable<Trade>> GetTradesAsync(this IBinanceApi api, IBinanceApiUser user, string symbol, long fromId = BinanceApi.NullId, int limit = default, CancellationToken token = default)
        {
            Throw.IfNull(api, nameof(api));
            Throw.IfNull(user, nameof(user));

            return api.GetTradesAsync(user.ApiKey, symbol, fromId, limit, token);
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
            Throw.IfNull(api, nameof(api));

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
            Throw.IfNull(api, nameof(api));
            Throw.IfNull(order, nameof(order));

            // Cancel order using order ID.
            return api.CancelOrderAsync(order.User, order.Symbol, order.Id, newClientOrderId, recvWindow, token);
        }

        /// <summary>
        /// Cancel all open orders for a symbol or all symbols.
        /// </summary>
        /// <param name="api"></param>
        /// <param name="user"></param>
        /// <param name="symbol"></param>
        /// <param name="recvWindow"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<string>> CancelAllOrdersAsync(this IBinanceApi api, IBinanceApiUser user, string symbol = null, long recvWindow = default, CancellationToken token = default)
        {
            Throw.IfNull(api, nameof(api));

            var symbols = string.IsNullOrWhiteSpace(symbol)
                ? await api.SymbolsAsync(token).ConfigureAwait(false)
                : new [] { symbol };

            var cancelOrderIds = new List<string>();

            foreach (var s in symbols)
            {
                var orders = await api.GetOpenOrdersAsync(user, s, recvWindow, token)
                    .ConfigureAwait(false);

                foreach (var order in orders)
                {
                    var cancelOrderId = await api.CancelAsync(order, recvWindow: recvWindow, token: token)
                        .ConfigureAwait(false);

                    cancelOrderIds.Add(cancelOrderId);
                }
            }

            return cancelOrderIds;
        }

        /// <summary>
        /// Get trades associated with an order.
        /// 
        /// NOTE: Without any trade reference (e.g. first and last trade ID),
        ///       this must query ALL account trades for a symbol.
        /// </summary>
        /// <param name="api"></param>
        /// <param name="order"></param>
        /// <param name="recvWindow"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<AccountTrade>> GetTradesAsync(this IBinanceApi api, Order order, long recvWindow = default, CancellationToken token = default)
        {
            Throw.IfNull(api, nameof(api));
            Throw.IfNull(order, nameof(order));

            var orderTrades = new List<AccountTrade>();

            if (order.Status == OrderStatus.Rejected)
                return orderTrades;

            // TODO: Can 'canceled' orders have trades?
            //if (order.Status == OrderStatus.Canceled)
            //    return orderTrades;
            
            // TODO: Can 'expired' orders have trades?
            //if (order.Status == OrderStatus.Expired)
            //    return orderTrades;

            long fromId = -1;
            const int limit = 500;
            while (!token.IsCancellationRequested)
            {
                // Get trades newer than trade ID (inclusive) returns oldest to newest (begin with trade ID: 0).
                var trades = await api.GetAccountTradesAsync(order.User, order.Symbol, fromId + 1, limit, recvWindow, token)
                    .ConfigureAwait(false);

                // No more trades to sift through.
                if (!trades.Any())
                    break;

                // Add trades with matching order ID.
                orderTrades.AddRange(trades.Where(t => t.OrderId == order.Id));

                // No more trades to query.
                if (trades.Count() < limit)
                    break;

                // Update trade ID for next query.
                fromId = trades.Last().Id;
            }

            return orderTrades;
        }
    }
}
