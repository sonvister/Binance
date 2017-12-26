using System;
using System.Threading;
using System.Threading.Tasks;
using Binance.Api.WebSocket.Events;
using Binance.Market;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Binance.Api.WebSocket
{
    /// <summary>
    /// A <see cref="ISymbolStatisticsWebSocketClient"/> implementation.
    /// </summary>
    public class SymbolStatisticsWebSocketClient : BinanceWebSocketClient<SymbolStatisticsEventArgs>, ISymbolStatisticsWebSocketClient
    {
        #region Public Events

        public event EventHandler<SymbolStatisticsEventArgs> Statistics;

        #endregion Public Events

        #region Public Properties

        public string Symbol { get; private set; }

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="logger"></param>
        public SymbolStatisticsWebSocketClient(IWebSocketClient client, ILogger<SymbolStatisticsWebSocketClient> logger = null)
            : base(client, logger)
        { }

        #endregion Construtors

        #region Public Methods

        public virtual Task SubscribeAsync(string symbol, Action<SymbolStatisticsEventArgs> callback, CancellationToken token)
        {
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            if (!token.CanBeCanceled)
                throw new ArgumentException("Token must be capable of being in the canceled state.", nameof(token));

            token.ThrowIfCancellationRequested();

            Symbol = symbol.FormatSymbol();

            if (IsSubscribed)
                throw new InvalidOperationException($"{nameof(SymbolStatisticsWebSocketClient)} is already subscribed to symbol: \"{Symbol}\"");

            return SubscribeToAsync($"{Symbol.ToLower()}@ticker", callback, token);
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Deserialize JSON and raise <see cref="SymbolStatisticsTradeEventArgs"/> event.
        /// </summary>
        /// <param name="json"></param>
        /// <param name="token"></param>
        /// <param name="callback"></param>
        protected override void DeserializeJsonAndRaiseEvent(string json, CancellationToken token, Action<SymbolStatisticsEventArgs> callback = null)
        {
            Throw.IfNullOrWhiteSpace(json, nameof(json));

            Logger?.LogDebug($"{nameof(SymbolStatisticsWebSocketClient)}: \"{json}\"");

            try
            {
                var jObject = JObject.Parse(json);

                var eventType = jObject["e"].Value<string>();

                if (eventType == "24hrTicker")
                {
                    var eventTime = jObject["E"].Value<long>();

                    var statistics = new SymbolStatistics(
                        jObject["s"].Value<string>(),  // symbol
                        TimeSpan.FromHours(24),        // period
                        jObject["p"].Value<decimal>(), // price change
                        jObject["P"].Value<decimal>(), // price change percent
                        jObject["w"].Value<decimal>(), // weighted average price
                        jObject["x"].Value<decimal>(), // previous day's close price
                        jObject["c"].Value<decimal>(), // current day's close price (last price)
                        jObject["Q"].Value<decimal>(), // close trade's quantity (last quantity)
                        jObject["b"].Value<decimal>(), // bid price
                        jObject["B"].Value<decimal>(), // bid quantity
                        jObject["a"].Value<decimal>(), // ask price
                        jObject["A"].Value<decimal>(), // ask quantity
                        jObject["o"].Value<decimal>(), // open price
                        jObject["h"].Value<decimal>(), // high price
                        jObject["l"].Value<decimal>(), // low price
                        jObject["v"].Value<decimal>(), // base asset volume
                        jObject["q"].Value<decimal>(), // quote asset volume
                        jObject["O"].Value<long>(),    // open time
                        jObject["C"].Value<long>(),    // close time
                        jObject["F"].Value<long>(),    // first trade ID
                        jObject["L"].Value<long>(),    // last trade ID
                        jObject["n"].Value<long>());   // trade count

                    var eventArgs = new SymbolStatisticsEventArgs(eventTime, token, statistics);

                    try
                    {
                        callback?.Invoke(eventArgs);
                        Statistics?.Invoke(this, eventArgs);
                    }
                    catch (OperationCanceledException) { }
                    catch (Exception e)
                    {
                        if (!token.IsCancellationRequested)
                        {
                            Logger?.LogError(e, $"{nameof(SymbolStatisticsWebSocketClient)}: Unhandled aggregate trade event handler exception.");
                        }
                    }
                }
                else
                {
                    Logger?.LogWarning($"{nameof(SymbolStatisticsWebSocketClient)}.{nameof(DeserializeJsonAndRaiseEvent)}: Unexpected event type ({eventType}).");
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception e)
            {
                if (!token.IsCancellationRequested)
                {
                    Logger?.LogError(e, $"{nameof(SymbolStatisticsWebSocketClient)}.{nameof(DeserializeJsonAndRaiseEvent)}");
                }
            }
        }

        #endregion Protected Methods
    }
}
