using Binance.Api.WebSocket.Events;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Binance.Api.WebSocket
{
    /// <summary>
    /// A <see cref="IKlineWebSocketClient"/> implementation.
    /// </summary>
    public class KlineWebSocketClient : BinanceWebSocketClient, IKlineWebSocketClient
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

        public Task SubscribeAsync(string symbol, KlineInterval interval, CancellationToken token = default)
        {
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            Symbol = symbol.FixSymbol();

            if (_isSubscribed)
                throw new InvalidOperationException($"{nameof(KlineWebSocketClient)} is already subscribed to symbol: \"{symbol}\"");

            return SubscribeAsync($"{Symbol.ToLower()}@kline_{interval.AsString()}", json =>
            {
                try
                {
                    var eventArgs = DeserializeJson(json);
                    if (eventArgs != null)
                    {
                        FireUpdateEvent(eventArgs);
                    }
                }
                catch (OperationCanceledException) { }
                catch (Exception e)
                {
                    LogException(e, $"{nameof(KlineWebSocketClient)}.{nameof(FireUpdateEvent)}");
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
        protected virtual KlineEventArgs DeserializeJson(string json)
        {
            Throw.IfNullOrWhiteSpace(json, nameof(json));

            try
            {
                _logger?.LogTrace($"{nameof(KlineWebSocketClient)}.{nameof(DeserializeJson)}: \"{json}\"");

                var jObject = JObject.Parse(json);

                var eventType = jObject["e"].Value<string>();

                if (eventType == "kline")
                {
                    var symbol = jObject["s"].Value<string>();
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

                    return new KlineEventArgs(eventTime, candlestick, firstTradeId, lastTradeId, isFinal);
                }
                else
                {
                    _logger?.LogWarning($"{nameof(KlineWebSocketClient)}.{nameof(DeserializeJson)}: Unexpected event type ({eventType}).");
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception e)
            {
                LogException(e, $"{nameof(KlineWebSocketClient)}.{nameof(DeserializeJson)}");
                throw;
            }

            return null;
        }

        /// <summary>
        /// Fire kline event.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void FireUpdateEvent(KlineEventArgs args)
        {
            Throw.IfNull(args, nameof(args));

            try { Kline?.Invoke(this, args); }
            catch (Exception e)
            {
                LogException(e, $"{nameof(KlineWebSocketClient)}.{nameof(FireUpdateEvent)}");
                throw;
            }
        }

        #endregion Protected Methods
    }
}
