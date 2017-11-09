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
        /// <param name="client"></param>
        /// <param name="logger"></param>
        public TradesWebSocketClient(IWebSocketClient client, ILogger<TradesWebSocketClient> logger = null)
            : base(client, logger)
        { }

        #endregion Construtors

        #region Public Methods

        public virtual Task SubscribeAsync(string symbol, CancellationToken token)
            => SubscribeAsync(symbol, null, token);

        public virtual Task SubscribeAsync(string symbol, Action<AggregateTradeEventArgs> callback, CancellationToken token)
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
        /// <param name="token"></param>
        /// <param name="callback"></param>
        protected override void DeserializeJsonAndRaiseEvent(string json, CancellationToken token, Action<AggregateTradeEventArgs> callback = null)
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

                    var eventArgs = new AggregateTradeEventArgs(eventTime, token, trade);

                    try
                    {
                        callback?.Invoke(eventArgs);
                        AggregateTrade?.Invoke(this, eventArgs);
                    }
                    catch (Exception e)
                    {
                        LogException(e, $"{nameof(TradesWebSocketClient)} event handler");
                    }
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

        #endregion Protected Methods
    }
}
