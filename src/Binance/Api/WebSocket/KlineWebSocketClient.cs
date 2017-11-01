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
    /// A <see cref="IKlineWebSocketClient"/> implementation.
    /// </summary>
    public class KlineWebSocketClient : BinanceWebSocketClient<KlineEventArgs>, IKlineWebSocketClient
    {
        #region Public Events

        public event EventHandler<KlineEventArgs> Kline;

        #endregion Public Events

        #region Public Properties

        public string Symbol { get; private set; }

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger"></param>
        public KlineWebSocketClient(ILogger<KlineWebSocketClient> logger = null)
            : base(logger)
        { }

        #endregion Construtors

        #region Public Methods

        public virtual Task SubscribeAsync(string symbol, KlineInterval interval, CancellationToken token = default)
            => SubscribeAsync(symbol, interval, null, token);

        public virtual Task SubscribeAsync(string symbol, KlineInterval interval, Action<KlineEventArgs> callback, CancellationToken token = default)
        {
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            Symbol = symbol.FormatSymbol();

            if (IsSubscribed)
                throw new InvalidOperationException($"{nameof(KlineWebSocketClient)} is already subscribed to symbol: \"{symbol}\"");

            return SubscribeToAsync($"{Symbol.ToLower()}@kline_{interval.AsString()}", callback, token);
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Deserialize JSON and raise <see cref="KlineEventArgs"/> event.
        /// </summary>
        /// <param name="json"></param>
        /// <param name="callback"></param>
        protected override void DeserializeJsonAndRaiseEvent(string json, Action<KlineEventArgs> callback = null)
        {
            Throw.IfNullOrWhiteSpace(json, nameof(json));

            Logger?.LogDebug($"{nameof(KlineWebSocketClient)}: \"{json}\"");

            try
            {
                var jObject = JObject.Parse(json);

                var eventType = jObject["e"].Value<string>();

                if (eventType == "kline")
                {
                    //var symbol = jObject["s"].Value<string>();
                    var eventTime = jObject["E"].Value<long>();

                    var firstTradeId = jObject["k"]["f"].Value<long>();
                    var lastTradeId = jObject["k"]["L"].Value<long>();

                    var isFinal = jObject["k"]["x"].Value<bool>();

                    var candlestick = new Candlestick(
                        jObject["k"]["s"].Value<string>(),  // symbol
                        jObject["k"]["i"].Value<string>()
                            .ToKlineInterval(),             // interval
                        jObject["k"]["t"].Value<long>(),    // open time
                        jObject["k"]["o"].Value<decimal>(), // open
                        jObject["k"]["h"].Value<decimal>(), // high
                        jObject["k"]["l"].Value<decimal>(), // low
                        jObject["k"]["c"].Value<decimal>(), // close
                        jObject["k"]["v"].Value<decimal>(), // volume
                        jObject["k"]["T"].Value<long>(),    // close time
                        jObject["k"]["q"].Value<decimal>(), // quote asset volume
                        jObject["k"]["n"].Value<long>(),    // number of trades
                        jObject["k"]["V"].Value<decimal>(), // taker buy base asset volume (volume of active buy)
                        jObject["k"]["Q"].Value<decimal>()  // taker buy quote asset volume (quote volume of active buy)
                    );

                    var eventArgs = new KlineEventArgs(eventTime, candlestick, firstTradeId, lastTradeId, isFinal);

                    callback?.Invoke(eventArgs);
                    RaiseUpdateEvent(eventArgs);
                }
                else
                {
                    Logger?.LogWarning($"{nameof(KlineWebSocketClient)}.{nameof(DeserializeJsonAndRaiseEvent)}: Unexpected event type ({eventType}).");
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception e)
            {
                LogException(e, $"{nameof(KlineWebSocketClient)}.{nameof(DeserializeJsonAndRaiseEvent)}");
                throw;
            }
        }

        /// <summary>
        /// Raise kline event.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void RaiseUpdateEvent(KlineEventArgs args)
        {
            Throw.IfNull(args, nameof(args));

            try { Kline?.Invoke(this, args); }
            catch (Exception e)
            {
                LogException(e, $"{nameof(KlineWebSocketClient)}.{nameof(RaiseUpdateEvent)}");
                throw;
            }
        }

        #endregion Protected Methods
    }
}
