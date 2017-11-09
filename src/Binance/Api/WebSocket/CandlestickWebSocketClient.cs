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
    /// A <see cref="ICandlestickWebSocketClient"/> implementation.
    /// </summary>
    public class CandlestickWebSocketClient : BinanceWebSocketClient<CandlestickEventArgs>, ICandlestickWebSocketClient
    {
        #region Public Events

        public event EventHandler<CandlestickEventArgs> Candlestick;

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
        public CandlestickWebSocketClient(IWebSocketClient client, ILogger<CandlestickWebSocketClient> logger = null)
            : base(client, logger)
        { }

        #endregion Construtors

        #region Public Methods

        public virtual Task SubscribeAsync(string symbol, CandlestickInterval interval, CancellationToken token)
            => SubscribeAsync(symbol, interval, null, token);

        public virtual Task SubscribeAsync(string symbol, CandlestickInterval interval, Action<CandlestickEventArgs> callback, CancellationToken token)
        {
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            Symbol = symbol.FormatSymbol();

            if (IsSubscribed)
                throw new InvalidOperationException($"{nameof(CandlestickWebSocketClient)} is already subscribed to symbol: \"{symbol}\"");

            return SubscribeToAsync($"{Symbol.ToLower()}@kline_{interval.AsString()}", callback, token);
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Deserialize JSON and raise <see cref="CandlestickEventArgs"/> event.
        /// </summary>
        /// <param name="json"></param>
        /// <param name="token"></param>
        /// <param name="callback"></param>
        protected override void DeserializeJsonAndRaiseEvent(string json, CancellationToken token, Action<CandlestickEventArgs> callback = null)
        {
            Throw.IfNullOrWhiteSpace(json, nameof(json));

            Logger?.LogDebug($"{nameof(CandlestickWebSocketClient)}: \"{json}\"");

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
                            .ToCandlestickInterval(),             // interval
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

                    var eventArgs = new CandlestickEventArgs(eventTime, token, candlestick, firstTradeId, lastTradeId, isFinal);

                    try
                    {
                        callback?.Invoke(eventArgs);
                        Candlestick?.Invoke(this, eventArgs);
                    }
                    catch (Exception e)
                    {
                        LogException(e, $"{nameof(CandlestickWebSocketClient)} event handler");
                    }
                }
                else
                {
                    Logger?.LogWarning($"{nameof(CandlestickWebSocketClient)}.{nameof(DeserializeJsonAndRaiseEvent)}: Unexpected event type ({eventType}).");
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception e)
            {
                LogException(e, $"{nameof(CandlestickWebSocketClient)}.{nameof(DeserializeJsonAndRaiseEvent)}");
                throw;
            }
        }

        #endregion Protected Methods
    }
}
