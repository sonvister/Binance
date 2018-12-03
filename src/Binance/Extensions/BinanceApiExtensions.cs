using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

// ReSharper disable once CheckNamespace
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
            Throw.IfNull(api, nameof(api));

            var timestamp = await api.GetTimestampAsync(token).ConfigureAwait(false);

            return timestamp.ToDateTime();
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
        public static Task<IEnumerable<Trade>> GetTradesFromAsync(this IBinanceApi api, IBinanceApiUser user, string symbol, long fromId, int limit = default, CancellationToken token = default)
        {
            Throw.IfNull(api, nameof(api));
            Throw.IfNull(user, nameof(user));

            return api.GetTradesFromAsync(user.ApiKey, symbol, fromId, limit, token);
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
        /// Get aggregate/compressed trades within a time interval (INCLUSIVE).
        /// </summary>
        /// <param name="api"></param>
        /// <param name="symbol"></param>
        /// <param name="timeInterval"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task<IEnumerable<AggregateTrade>> GetAggregateTradesAsync(this IBinanceApi api, string symbol, (DateTime, DateTime) timeInterval, CancellationToken token = default)
        {
            Throw.IfNull(api, nameof(api));

            return api.GetAggregateTradesAsync(symbol, timeInterval.Item1, timeInterval.Item2, token);
        }

        /// <summary>
        /// Get aggregate/compressed trades within a time interval (INCLUSIVE).
        /// </summary>
        /// <param name="api"></param>
        /// <param name="symbol"></param>
        /// <param name="timeInterval"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task<IEnumerable<AggregateTrade>> GetAggregateTradesAsync(this IBinanceApi api, string symbol, (long, long) timeInterval, CancellationToken token = default)
        {
            Throw.IfNull(api, nameof(api));

            return api.GetAggregateTradesAsync(symbol, timeInterval.Item1.ToDateTime(), timeInterval.Item2.ToDateTime(), token);
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
        public static Task<IEnumerable<Candlestick>> GetCandlesticksAsync(this IBinanceApi api, string symbol, string interval, int limit = default, DateTime startTime = default, DateTime endTime = default, CancellationToken token = default)
        {
            Throw.IfNull(api, nameof(api));

            return api.GetCandlesticksAsync(symbol, interval.ToCandlestickInterval(), limit, startTime, endTime, token);
        }

        /// <summary>
        /// Get candlesticks for a symbol within a time interval with optional limit.
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

            return api.GetCandlesticksAsync(symbol, interval, limit, startTime, endTime, token);
        }

        /// <summary>
        /// Get candlesticks for a symbol within a time interval with optional limit.
        /// </summary>
        /// <param name="api"></param>
        /// <param name="symbol"></param>
        /// <param name="interval"></param>
        /// <param name="timeInterval"></param>
        /// <param name="limit"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task<IEnumerable<Candlestick>> GetCandlesticksAsync(this IBinanceApi api, string symbol, CandlestickInterval interval, (long, long) timeInterval, int limit = default, CancellationToken token = default)
        {
            Throw.IfNull(api, nameof(api));

            return api.GetCandlesticksAsync(symbol, interval, limit, timeInterval.Item1.ToDateTime(), timeInterval.Item2.ToDateTime(), token);
        }

        /// <summary>
        /// Get orders within a time interval (INCLUSIVE).
        /// </summary>
        /// <param name="api"></param>
        /// <param name="user"></param>
        /// <param name="symbol"></param>
        /// <param name="timeInterval"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task<IEnumerable<Order>> GetOrdersAsync(this IBinanceApi api, IBinanceApiUser user, string symbol, (DateTime, DateTime) timeInterval, long recvWindow = default, CancellationToken token = default)
        {
            Throw.IfNull(api, nameof(api));

            return api.GetOrdersAsync(user, symbol, timeInterval.Item1, timeInterval.Item2, recvWindow, token);
        }

        /// <summary>
        /// Get orders within a time interval (INCLUSIVE).
        /// </summary>
        /// <param name="api"></param>
        /// <param name="user"></param>
        /// <param name="symbol"></param>
        /// <param name="timeInterval"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task<IEnumerable<Order>> GetOrdersAsync(this IBinanceApi api, IBinanceApiUser user, string symbol, (long, long) timeInterval, long recvWindow = default, CancellationToken token = default)
        {
            Throw.IfNull(api, nameof(api));

            return api.GetOrdersAsync(user, symbol, timeInterval.Item1.ToDateTime(), timeInterval.Item2.ToDateTime(), recvWindow, token);
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

             var cancelOrderIds = new List<string>();

            IEnumerable<Order> orders;
            do
            {
                orders = await api.GetOpenOrdersAsync(user, symbol, recvWindow, token)
                    .ConfigureAwait(false);

                foreach (var order in orders)
                {
                    var cancelOrderId = await api.CancelAsync(order, recvWindow: recvWindow, token: token)
                        .ConfigureAwait(false);

                    cancelOrderIds.Add(cancelOrderId);
                }
            } while (orders.Any());

            return cancelOrderIds;
        }

        /// <summary>
        /// Get account trades associated with an order.
        /// 
        /// NOTE: Without any trade reference (e.g. first and last trade ID),
        ///       this must query ALL account trades for a symbol.
        /// </summary>
        /// <param name="api"></param>
        /// <param name="order"></param>
        /// <param name="recvWindow"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<AccountTrade>> GetAccountTradesAsync(this IBinanceApi api, Order order, long recvWindow = default, CancellationToken token = default)
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
                // ReSharper disable once PossibleMultipleEnumeration
                if (!trades.Any())
                    break;

                // Add trades with matching order ID.
                // ReSharper disable once PossibleMultipleEnumeration
                orderTrades.AddRange(trades.Where(t => t.OrderId == order.Id));

                // No more trades to query.
                // ReSharper disable once PossibleMultipleEnumeration
                if (trades.Count() < limit)
                    break;

                // Update trade ID for next query.
                // ReSharper disable once PossibleMultipleEnumeration
                fromId = trades.Last().Id;
            }

            return orderTrades;
        }

        /// <summary>
        /// Get account trades within a time interval (INCLUSIVE).
        /// </summary>
        /// <param name="api"></param>
        /// <param name="user"></param>
        /// <param name="symbol"></param>
        /// <param name="timeInterval"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task<IEnumerable<AccountTrade>> GetAccountTradesAsync(this IBinanceApi api, IBinanceApiUser user, string symbol, (DateTime, DateTime) timeInterval, long recvWindow = default, CancellationToken token = default)
        {
            Throw.IfNull(api, nameof(api));

            return api.GetAccountTradesAsync(user, symbol, timeInterval.Item1, timeInterval.Item2, recvWindow, token);
        }

        /// <summary>
        /// Get account trades within a time interval (INCLUSIVE).
        /// </summary>
        /// <param name="api"></param>
        /// <param name="user"></param>
        /// <param name="symbol"></param>
        /// <param name="timeInterval"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task<IEnumerable<AccountTrade>> GetAccountTradesAsync(this IBinanceApi api, IBinanceApiUser user, string symbol, (long, long) timeInterval, long recvWindow = default, CancellationToken token = default)
        {
            Throw.IfNull(api, nameof(api));

            return api.GetAccountTradesAsync(user, symbol, timeInterval.Item1.ToDateTime(), timeInterval.Item2.ToDateTime(), recvWindow, token);
        }


        /// <summary>
        /// Get current price of a currency pair or determine the exchange rate
        /// (base price / quote price) of two assets using available markets.
        /// </summary>
        /// <param name="api"></param>
        /// <param name="baseAsset">The base asset.</param>
        /// <param name="quoteAsset">The quote asset.</param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static async Task<decimal> GetExchangeRateAsync(this IBinanceApi api, string baseAsset, string quoteAsset, CancellationToken token = default)
        {
            Throw.IfNull(api, nameof(api));
            Throw.IfNullOrWhiteSpace(baseAsset, nameof(baseAsset));
            Throw.IfNullOrWhiteSpace(quoteAsset, nameof(quoteAsset));

            baseAsset = baseAsset.FormatSymbol();
            quoteAsset = quoteAsset.FormatSymbol();

            if (!Asset.IsValid(baseAsset))
                throw new ArgumentException($"{nameof(IBinanceApi)}.{nameof(GetExchangeRateAsync)}: Invalid asset: \"{baseAsset}\"", nameof(baseAsset));
            if (!Asset.IsValid(quoteAsset))
                throw new ArgumentException($"{nameof(IBinanceApi)}.{nameof(GetExchangeRateAsync)}: Invalid asset: \"{quoteAsset}\"", nameof(quoteAsset));

            if (baseAsset == quoteAsset)
                return 1;

            var symbol = $"{baseAsset}{quoteAsset}";

            if (Symbol.IsValid(symbol))
            {
                var price = await api.GetPriceAsync(symbol, token)
                    .ConfigureAwait(false);

                return price.Value;
            }

            symbol = $"{quoteAsset}{baseAsset}";

            if (Symbol.IsValid(symbol))
            {
                var price = await api.GetPriceAsync(symbol, token)
                    .ConfigureAwait(false);

                return 1 / price.Value;
            }

            var markets = new [] { "BTC", "ETH", "BNB", "USDT" };

            foreach (var market in markets)
            {
                var s1 = $"{baseAsset}{market}";
                var s2 = $"{quoteAsset}{market}";

                if (!Symbol.IsValid(s1) || !Symbol.IsValid(s2))
                    continue;

                var t1 = api.GetPriceAsync(s1, token);
                var t2 = api.GetPriceAsync(s2, token);

                await Task.WhenAll(t1, t2)
                    .ConfigureAwait(false);

                return t1.Result.Value / t2.Result.Value;
            }

            throw new Exception($"{nameof(IBinanceApi)}.{nameof(GetExchangeRateAsync)}: No symbols/markets available to calculate exchange rate.");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="api"></param>
        /// <param name="user"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task<string> UserStreamStartAsync(this IBinanceApi api, IBinanceApiUser user, CancellationToken token = default)
        {
            Throw.IfNull(api, nameof(api));
            Throw.IfNull(user, nameof(user));

            return api.UserStreamStartAsync(user.ApiKey, token);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="api"></param>
        /// <param name="user"></param>
        /// <param name="listenKey"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task UserStreamKeepAliveAsync(this IBinanceApi api, IBinanceApiUser user, string listenKey, CancellationToken token = default)
        {
            Throw.IfNull(api, nameof(api));
            Throw.IfNull(user, nameof(user));

            return api.UserStreamKeepAliveAsync(user.ApiKey, listenKey, token);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="api"></param>
        /// <param name="user"></param>
        /// <param name="listenKey"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static Task UserStreamCloseAsync(this IBinanceApi api, IBinanceApiUser user, string listenKey, CancellationToken token = default)
        {
            Throw.IfNull(api, nameof(api));
            Throw.IfNull(user, nameof(user));

            return api.UserStreamCloseAsync(user.ApiKey, listenKey, token);
        }
    }
}
