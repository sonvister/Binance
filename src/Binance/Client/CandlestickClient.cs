using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Binance.Client.Events;
using Binance.Market;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Binance.Client
{
    /// <summary>
    /// The default <see cref="ICandlestickClient"/> implementation.
    /// </summary>
    public class CandlestickClient : JsonClient<CandlestickEventArgs>, ICandlestickClient
    {
        #region Public Events

        public event EventHandler<CandlestickEventArgs> Candlestick;

        #endregion Public Events

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger"></param>
        public CandlestickClient(ILogger<CandlestickClient> logger = null)
            : base(logger)
        { }

        #endregion Construtors

        #region Public Methods

        public virtual ICandlestickClient Subscribe(string symbol, CandlestickInterval interval, Action<CandlestickEventArgs> callback)
        {
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            symbol = symbol.FormatSymbol();

            Logger?.LogDebug($"{nameof(CandlestickClient)}.{nameof(Subscribe)}: \"{symbol}\" \"{interval.AsString()}\" (callback: {(callback == null ? "no" : "yes")}).  [thread: {Thread.CurrentThread.ManagedThreadId}]");

            SubscribeStream(GetStreamName(symbol, interval), callback);

            return this;
        }

        public virtual ICandlestickClient Unsubscribe(string symbol, CandlestickInterval interval, Action<CandlestickEventArgs> callback)
        {
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            symbol = symbol.FormatSymbol();

            Logger?.LogDebug($"{nameof(CandlestickClient)}.{nameof(Unsubscribe)}: \"{symbol}\" \"{interval.AsString()}\" (callback: {(callback == null ? "no" : "yes")}).  [thread: {Thread.CurrentThread.ManagedThreadId}]");

            UnsubscribeStream(GetStreamName(symbol, interval), callback);

            return this;
        }

        public virtual new ICandlestickClient Unsubscribe() => (ICandlestickClient)base.Unsubscribe();

        #endregion Public Methods

        #region Protected Methods

        protected override Task HandleMessageAsync(IEnumerable<Action<CandlestickEventArgs>> callbacks, string stream, string json, CancellationToken token = default)
        {
            //Logger?.LogDebug($"{nameof(CandlestickWebSocketClient)}: \"{args.Json}\"");

            try
            {
                var jObject = JObject.Parse(json);

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

                    var eventArgs = new CandlestickEventArgs(eventTime, token, candlestick, firstTradeId, lastTradeId, isFinal);

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
                        if (!token.IsCancellationRequested)
                        {
                            Logger?.LogError(e, $"{nameof(CandlestickClient)}: Unhandled candlestick event handler exception.");
                        }
                    }
                }
                else
                {
                    Logger?.LogWarning($"{nameof(CandlestickClient)}.{nameof(HandleMessageAsync)}: Unexpected event type ({eventType}).");
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception e)
            {
                if (!token.IsCancellationRequested)
                {
                    Logger?.LogError(e, $"{nameof(CandlestickClient)}.{nameof(HandleMessageAsync)}");
                }
            }

            return Task.CompletedTask;
        }

        #endregion Protected Methods

        #region Private Methods

        private static string GetStreamName(string symbol, CandlestickInterval interval)
            => $"{symbol.ToLowerInvariant()}@kline_{interval.AsString()}";

        #endregion Private Methods
    }
}
