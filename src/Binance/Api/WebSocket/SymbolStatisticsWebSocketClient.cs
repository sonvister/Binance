using System;
using System.Linq;
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

        public event EventHandler<SymbolStatisticsEventArgs> StatisticsUpdate;

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

        public virtual Task SubscribeAsync(Action<SymbolStatisticsEventArgs> callback, CancellationToken token)
        {
            if (!token.CanBeCanceled)
                throw new ArgumentException("Token must be capable of being in the canceled state.", nameof(token));

            token.ThrowIfCancellationRequested();

            if (IsSubscribed)
                throw new InvalidOperationException($"{nameof(SymbolStatisticsWebSocketClient)} is already subscribed to all symbols.");

            return SubscribeToAsync("!ticker@arr", callback, token);
        }

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
                SymbolStatisticsEventArgs eventArgs;

                if (json.IsJsonArray())
                {
                    // Simulate a single event time.
                    var eventTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                    var statistics = JArray.Parse(json).Select(jToken => DeserializeSymbolStatistics(jToken)).ToArray();

                    eventArgs = new SymbolStatisticsEventArgs(eventTime, token, statistics);
                }
                else
                {
                    var jObject = JObject.Parse(json);

                    var eventType = jObject["e"].Value<string>();

                    if (eventType == "24hrTicker")
                    {
                        var eventTime = jObject["E"].Value<long>();

                        var statistics = DeserializeSymbolStatistics(jObject);

                        eventArgs = new SymbolStatisticsEventArgs(eventTime, token, statistics);
                    }
                    else
                    {
                        Logger?.LogWarning($"{nameof(SymbolStatisticsWebSocketClient)}.{nameof(DeserializeJsonAndRaiseEvent)}: Unexpected event type ({eventType}).");
                        return;
                    }
                }

                try
                {
                    callback?.Invoke(eventArgs);
                    StatisticsUpdate?.Invoke(this, eventArgs);
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

        #region Private Methods

        private static SymbolStatistics DeserializeSymbolStatistics(JToken jToken)
        {
            return new SymbolStatistics(
                jToken["s"].Value<string>(),  // symbol
                TimeSpan.FromHours(24),       // period
                jToken["p"].Value<decimal>(), // price change
                jToken["P"].Value<decimal>(), // price change percent
                jToken["w"].Value<decimal>(), // weighted average price
                jToken["x"].Value<decimal>(), // previous day's close price
                jToken["c"].Value<decimal>(), // current day's close price (last price)
                jToken["Q"].Value<decimal>(), // close trade's quantity (last quantity)
                jToken["b"].Value<decimal>(), // bid price
                jToken["B"].Value<decimal>(), // bid quantity
                jToken["a"].Value<decimal>(), // ask price
                jToken["A"].Value<decimal>(), // ask quantity
                jToken["o"].Value<decimal>(), // open price
                jToken["h"].Value<decimal>(), // high price
                jToken["l"].Value<decimal>(), // low price
                jToken["v"].Value<decimal>(), // base asset volume
                jToken["q"].Value<decimal>(), // quote asset volume
                jToken["O"].Value<long>(),    // open time
                jToken["C"].Value<long>(),    // close time
                jToken["F"].Value<long>(),    // first trade ID
                jToken["L"].Value<long>(),    // last trade ID
                jToken["n"].Value<long>());   // trade count
        }

        #endregion Private Methods
    }
}
