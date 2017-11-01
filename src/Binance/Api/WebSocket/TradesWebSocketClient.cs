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
    /// A <see cref="ITradesWebSocketClient"/> implementation.
    /// </summary>
    public class TradesWebSocketClient : BinanceWebSocketClient<AggregateTradeEventArgs>, ITradesWebSocketClient
    {
        #region Public Events

        public event EventHandler<AggregateTradeEventArgs> AggregateTrade;

        #endregion Public Events

        #region Public Properties

        public string Symbol { get; private set; }

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger"></param>
        public TradesWebSocketClient(ILogger<TradesWebSocketClient> logger = null)
            : base(logger)
        { }

        #endregion Construtors

        #region Public Methods

        public virtual Task SubscribeAsync(string symbol, CancellationToken token = default)
            => SubscribeAsync(symbol, null, token);

        public virtual Task SubscribeAsync(string symbol, Action<AggregateTradeEventArgs> callback, CancellationToken token = default)
        {
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            Symbol = symbol.FormatSymbol();

            if (IsSubscribed)
                throw new InvalidOperationException($"{nameof(TradesWebSocketClient)} is already subscribed to symbol: \"{Symbol}\"");

            return SubscribeToAsync($"{Symbol.ToLower()}@aggTrade", callback, token);
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Deserialize JSON and raise <see cref="AggregateTradeEventArgs"/> event.
        /// </summary>
        /// <param name="json"></param>
        /// <param name="callback"></param>
        protected override void DeserializeJsonAndRaiseEvent(string json, Action<AggregateTradeEventArgs> callback = null)
        {
            Throw.IfNullOrWhiteSpace(json, nameof(json));

            Logger?.LogDebug($"{nameof(TradesWebSocketClient)}: \"{json}\"");

            try
            {
                var jObject = JObject.Parse(json);

                var eventType = jObject["e"].Value<string>();

                if (eventType == "aggTrade")
                {
                    var eventTime = jObject["E"].Value<long>();

                    var trade = new AggregateTrade(
                        jObject["s"].Value<string>(),
                        jObject["a"].Value<long>(),    // ID
                        jObject["p"].Value<decimal>(), // price
                        jObject["q"].Value<decimal>(), // quantity
                        jObject["f"].Value<long>(),    // first trade ID
                        jObject["l"].Value<long>(),    // last trade ID
                        jObject["T"].Value<long>(),    // timestamp
                        jObject["m"].Value<bool>(),    // is buyer maker
                        jObject["M"].Value<bool>());   // is best price

                    var eventArgs = new AggregateTradeEventArgs(eventTime, trade);

                    callback?.Invoke(eventArgs);
                    RaiseUpdateEvent(eventArgs);
                }
                else
                {
                    Logger?.LogWarning($"{nameof(TradesWebSocketClient)}.{nameof(DeserializeJsonAndRaiseEvent)}: Unexpected event type ({eventType}).");
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception e)
            {
                LogException(e, $"{nameof(TradesWebSocketClient)}.{nameof(DeserializeJsonAndRaiseEvent)}");
                throw;
            }
        }

        /// <summary>
        /// Raise aggregate trade event.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void RaiseUpdateEvent(AggregateTradeEventArgs args)
        {
            Throw.IfNull(args, nameof(args));

            try { AggregateTrade?.Invoke(this, args); }
            catch (Exception e)
            {
                LogException(e, $"{nameof(TradesWebSocketClient)}.{nameof(RaiseUpdateEvent)}");
                throw;
            }
        }

        #endregion Protected Methods
    }
}
