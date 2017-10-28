using Binance.Api.WebSocket.Events;
using Binance.Market;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Binance.Api.WebSocket
{
    /// <summary>
    /// A <see cref="ITradesWebSocketClient"/> implementation.
    /// </summary>
    public class TradesWebSocketClient : BinanceWebSocketClient, ITradesWebSocketClient
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

            if (_isSubscribed)
                throw new InvalidOperationException($"{nameof(TradesWebSocketClient)} is already subscribed to symbol: \"{Symbol}\"");

            return SubscribeAsync($"{Symbol.ToLower()}@aggTrade", json =>
            {
                try
                {
                    var eventArgs = DeserializeJson(json);
                    if (eventArgs != null)
                    {
                        callback?.Invoke(eventArgs);
                        RaiseUpdateEvent(eventArgs);
                    }
                }
                catch (OperationCanceledException) { }
                catch (Exception e)
                {
                    LogException(e, $"{nameof(TradesWebSocketClient)}.{nameof(RaiseUpdateEvent)}");
                }
            }, token);
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Deserialize event JSON.
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        protected virtual AggregateTradeEventArgs DeserializeJson(string json)
        {
            Throw.IfNullOrWhiteSpace(json, nameof(json));

            try
            {
                _logger?.LogTrace($"{nameof(TradesWebSocketClient)}.{nameof(DeserializeJson)}: \"{json}\"");

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

                    return new AggregateTradeEventArgs(eventTime, trade);
                }
                else
                {
                    _logger?.LogWarning($"{nameof(TradesWebSocketClient)}.{nameof(DeserializeJson)}: Unexpected event type ({eventType}).");
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception e)
            {
                LogException(e, $"{nameof(TradesWebSocketClient)}.{nameof(DeserializeJson)}");
                throw;
            }

            return null;
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
