using System;
using System.Collections.Generic;
using System.Threading;
using Binance.Market;
using Binance.WebSocket.Events;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Binance.WebSocket
{
    /// <summary>
    /// A <see cref="ICandlestickWebSocketClient"/> implementation.
    /// </summary>
    public class CandlestickWebSocketClient : BinanceWebSocketClient<CandlestickEventArgs>, ICandlestickWebSocketClient
    {
        #region Public Events

        public event EventHandler<CandlestickEventArgs> Candlestick;

        #endregion Public Events

        #region Constructors

        /// <summary>
        /// Default constructor provides default web socket stream, but no logging.
        /// </summary>
        public CandlestickWebSocketClient()
            : this(new BinanceWebSocketStream())
        { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="webSocket"></param>
        /// <param name="logger"></param>
        public CandlestickWebSocketClient(IWebSocketStream webSocket, ILogger<CandlestickWebSocketClient> logger = null)
            : base(webSocket, logger)
        { }

        #endregion Construtors

        #region Public Methods

        public virtual void Subscribe(string symbol, CandlestickInterval interval, Action<CandlestickEventArgs> callback)
        {
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            symbol = symbol.FormatSymbol();

            Logger?.LogInformation($"{nameof(CandlestickWebSocketClient)}.{nameof(Subscribe)}: \"{symbol}\" \"{interval.AsString()}\" (callback: {(callback == null ? "no" : "yes")}).  [thread: {Thread.CurrentThread.ManagedThreadId}]");

            SubscribeStream(GetStreamName(symbol, interval), callback);
        }

        public virtual void Unsubscribe(string symbol, CandlestickInterval interval, Action<CandlestickEventArgs> callback)
        {
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            symbol = symbol.FormatSymbol();

            Logger?.LogInformation($"{nameof(CandlestickWebSocketClient)}.{nameof(Unsubscribe)}: \"{symbol}\" \"{interval.AsString()}\" (callback: {(callback == null ? "no" : "yes")}).  [thread: {Thread.CurrentThread.ManagedThreadId}]");

            UnsubscribeStream(GetStreamName(symbol, interval), callback);
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
                    var eventTime = jObject["E"].Value<long>().ToDateTime();

                    var kLine = jObject["k"];

                    var firstTradeId = kLine["f"].Value<long>();
                    var lastTradeId = kLine["L"].Value<long>();

                    var isFinal = kLine["x"].Value<bool>();

                    var candlestick = new Candlestick(
                        kLine["s"].Value<string>(),  // symbol
                        kLine["i"].Value<string>()   // interval
                            .ToCandlestickInterval(), 
                        kLine["t"].Value<long>()     // open time
                            .ToDateTime(),           
                        kLine["o"].Value<decimal>(), // open
                        kLine["h"].Value<decimal>(), // high
                        kLine["l"].Value<decimal>(), // low
                        kLine["c"].Value<decimal>(), // close
                        kLine["v"].Value<decimal>(), // volume
                        kLine["T"].Value<long>()     // close time
                            .ToDateTime(),           
                        kLine["q"].Value<decimal>(), // quote asset volume
                        kLine["n"].Value<long>(),    // number of trades
                        kLine["V"].Value<decimal>(), // taker buy base asset volume (volume of active buy)
                        kLine["Q"].Value<decimal>()  // taker buy quote asset volume (quote volume of active buy)
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

        #region Private Methods

        private static string GetStreamName(string symbol, CandlestickInterval interval)
            => $"{symbol.ToLowerInvariant()}@kline_{interval.AsString()}";

        #endregion Private Methods
    }
}
