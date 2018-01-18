using System;
using System.Collections.Generic;
using System.Threading;
using Binance.Api.WebSocket.Events;
using Binance.Market;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Binance.Api.WebSocket
{
    /// <summary>
    /// A <see cref="IAggregateTradeWebSocketClient"/> implementation.
    /// </summary>
    public class AggregateTradeWebSocketClient : BinanceWebSocketClient<AggregateTradeEventArgs>, IAggregateTradeWebSocketClient
    {
        #region Public Events

        public event EventHandler<AggregateTradeEventArgs> AggregateTrade;

        #endregion Public Events

        #region Public Properties

        public string Symbol { get; private set; }

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Default constructor provides default web socket stream, but no logging.
        /// </summary>
        public AggregateTradeWebSocketClient()
            : this(new BinanceWebSocketStream(), null)
        { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="logger"></param>
        public AggregateTradeWebSocketClient(IWebSocketStream stream, ILogger<AggregateTradeWebSocketClient> logger = null)
            : base(stream, logger)
        { }

        #endregion Construtors

        #region Public Methods

        public virtual void Subscribe(string symbol, Action<AggregateTradeEventArgs> callback)
        {
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            Symbol = symbol.FormatSymbol();

            SubscribeTo($"{Symbol.ToLowerInvariant()}@aggTrade", callback);
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Deserialize JSON and raise <see cref="AggregateTradeEventArgs"/> event.
        /// </summary>
        /// <param name="json"></param>
        /// <param name="token"></param>
        /// <param name="callback"></param>
        protected override void DeserializeJsonAndRaiseEvent(string json, CancellationToken token, IEnumerable<Action<AggregateTradeEventArgs>> callbacks)
        {
            Throw.IfNullOrWhiteSpace(json, nameof(json));

            Logger?.LogDebug($"{nameof(AggregateTradeWebSocketClient)}: \"{json}\"");

            try
            {
                var jObject = JObject.Parse(json);

                var eventType = jObject["e"].Value<string>();

                if (eventType == "aggTrade")
                {
                    var eventTime = jObject["E"].Value<long>();

                    var trade = new AggregateTrade(
                        jObject["s"].Value<string>(),  // symbol
                        jObject["a"].Value<long>(),    // aggregate trade ID
                        jObject["p"].Value<decimal>(), // price
                        jObject["q"].Value<decimal>(), // quantity
                        jObject["f"].Value<long>(),    // first trade ID
                        jObject["l"].Value<long>(),    // last trade ID
                        jObject["T"].Value<long>(),    // trade time (timestamp)
                        jObject["m"].Value<bool>(),    // is buyer the market maker?
                        jObject["M"].Value<bool>());   // is best price match?

                    var eventArgs = new AggregateTradeEventArgs(eventTime, token, trade);

                    try
                    {
                        if (callbacks != null)
                        {
                            foreach (var callback in callbacks)
                                callback(eventArgs);
                        }
                        AggregateTrade?.Invoke(this, eventArgs);
                    }
                    catch (OperationCanceledException) { }
                    catch (Exception e)
                    {
                        if (!token.IsCancellationRequested)
                        {
                            Logger?.LogError(e, $"{nameof(AggregateTradeWebSocketClient)}: Unhandled aggregate trade event handler exception.");
                        }
                    }
                }
                else
                {
                    Logger?.LogWarning($"{nameof(AggregateTradeWebSocketClient)}.{nameof(DeserializeJsonAndRaiseEvent)}: Unexpected event type ({eventType}).");
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception e)
            {
                if (!token.IsCancellationRequested)
                {
                    Logger?.LogError(e, $"{nameof(AggregateTradeWebSocketClient)}.{nameof(DeserializeJsonAndRaiseEvent)}");
                }
            }
        }

        #endregion Protected Methods
    }
}
