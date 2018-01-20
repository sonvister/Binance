using System;
using System.Collections.Generic;
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
        /// Default constructor provides default web socket stream, but no logging.
        /// </summary>
        public CandlestickWebSocketClient()
            : this(new BinanceWebSocketStream(), null)
        { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="logger"></param>
        public CandlestickWebSocketClient(IWebSocketStream webSocket, ILogger<CandlestickWebSocketClient> logger = null)
            : base(webSocket, logger)
        { }

        #endregion Construtors

        #region Public Methods

        public virtual void Subscribe(string symbol, CandlestickInterval interval, Action<CandlestickEventArgs> callback)
        {
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            Symbol = symbol.FormatSymbol();

            SubscribeTo($"{Symbol.ToLowerInvariant()}@kline_{interval.AsString()}", callback);
        }

        #endregion Public Methods

        #region Protected Methods

        protected override void OnWebSocketEvent(WebSocketStreamEventArgs args, IEnumerable<Action<CandlestickEventArgs>> callbacks)
        {
            Logger?.LogDebug($"{nameof(CandlestickWebSocketClient)}: \"{args.Json}\"");

            try
            {
                var jObject = JObject.Parse(args.Json);

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
                            .ToCandlestickInterval(),       // interval
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

                    var eventArgs = new CandlestickEventArgs(eventTime, args.Token, candlestick, firstTradeId, lastTradeId, isFinal);

                    try
                    {
                        if (callbacks != null)
                        {
                            foreach (var callback in callbacks)
                                callback(eventArgs);
                        }
                        Candlestick?.Invoke(this, eventArgs);
                    }
                    catch (OperationCanceledException) { }
                    catch (Exception e)
                    {
                        if (!args.Token.IsCancellationRequested)
                        {
                            Logger?.LogError(e, $"{nameof(CandlestickWebSocketClient)}: Unhandled candlestick event handler exception.");
                        }
                    }
                }
                else
                {
                    Logger?.LogWarning($"{nameof(CandlestickWebSocketClient)}.{nameof(OnWebSocketEvent)}: Unexpected event type ({eventType}).");
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception e)
            {
                if (!args.Token.IsCancellationRequested)
                {
                    Logger?.LogError(e, $"{nameof(CandlestickWebSocketClient)}.{nameof(OnWebSocketEvent)}");
                }
            }
        }

        #endregion Protected Methods
    }
}
