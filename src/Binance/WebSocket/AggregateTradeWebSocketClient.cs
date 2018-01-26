using System;
using System.Collections.Generic;
using Binance.Market;
using Binance.WebSocket.Events;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Binance.WebSocket
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
            : this(new BinanceWebSocketStream())
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

        protected override void OnWebSocketEvent(WebSocketStreamEventArgs args, IEnumerable<Action<AggregateTradeEventArgs>> callbacks)
        {
            Logger?.LogDebug($"{nameof(AggregateTradeWebSocketClient)}: \"{args.Json}\"");

            try
            {
                var jObject = JObject.Parse(args.Json);

                var eventType = jObject["e"].Value<string>();

                if (eventType == "aggTrade")
                {
                    var eventTime = jObject["E"].Value<long>().ToDateTime();

                    var trade = new AggregateTrade(
                        jObject["s"].Value<string>(),  // symbol
                        jObject["a"].Value<long>(),    // aggregate trade ID
                        jObject["p"].Value<decimal>(), // price
                        jObject["q"].Value<decimal>(), // quantity
                        jObject["f"].Value<long>(),    // first trade ID
                        jObject["l"].Value<long>(),    // last trade ID
                        jObject["T"].Value<long>()
                            .ToDateTime(),             // trade time
                        jObject["m"].Value<bool>(),    // is buyer the market maker?
                        jObject["M"].Value<bool>());   // is best price match?

                    var eventArgs = new AggregateTradeEventArgs(eventTime, args.Token, trade);

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
                        if (!args.Token.IsCancellationRequested)
                        {
                            Logger?.LogError(e, $"{nameof(AggregateTradeWebSocketClient)}: Unhandled aggregate trade event handler exception.");
                        }
                    }
                }
                else
                {
                    Logger?.LogWarning($"{nameof(AggregateTradeWebSocketClient)}.{nameof(OnWebSocketEvent)}: Unexpected event type ({eventType}).");
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception e)
            {
                if (!args.Token.IsCancellationRequested)
                {
                    Logger?.LogError(e, $"{nameof(AggregateTradeWebSocketClient)}.{nameof(OnWebSocketEvent)}");
                }
            }
        }

        #endregion Protected Methods
    }
}
