using System;
using System.Collections.Generic;
using System.Linq;
using Binance.WebSocket.Events;
using Binance.Market;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System.Threading;

namespace Binance.WebSocket
{
    /// <summary>
    /// A <see cref="ISymbolStatisticsWebSocketClient"/> implementation.
    /// </summary>
    public class SymbolStatisticsWebSocketClient : BinanceWebSocketClient<SymbolStatisticsEventArgs>, ISymbolStatisticsWebSocketClient
    {
        #region Public Events

        public event EventHandler<SymbolStatisticsEventArgs> StatisticsUpdate;

        #endregion Public Events

        #region Constructors

        /// <summary>
        /// Default constructor provides default web socket stream, but no logging.
        /// </summary>
        public SymbolStatisticsWebSocketClient()
            : this(new BinanceWebSocketStream())
        { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="webSocket"></param>
        /// <param name="logger"></param>
        public SymbolStatisticsWebSocketClient(IWebSocketStream webSocket, ILogger<SymbolStatisticsWebSocketClient> logger = null)
            : base(webSocket, logger)
        { }

        #endregion Construtors

        #region Public Methods

        public virtual void Subscribe(Action<SymbolStatisticsEventArgs> callback)
        {
            Logger?.LogInformation($"{nameof(SymbolStatisticsWebSocketClient)}.{nameof(Subscribe)}: \"[All Symbols]\" (callback: {(callback == null ? "no" : "yes")}).  [thread: {Thread.CurrentThread.ManagedThreadId}]");

            SubscribeStream(GetStreamName(null), callback);
        }

        public virtual void Subscribe(string symbol, Action<SymbolStatisticsEventArgs> callback)
        {
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            symbol = symbol.FormatSymbol();

            Logger?.LogInformation($"{nameof(SymbolStatisticsWebSocketClient)}.{nameof(Subscribe)}: \"{symbol}\" (callback: {(callback == null ? "no" : "yes")}).  [thread: {Thread.CurrentThread.ManagedThreadId}]");

            SubscribeStream(GetStreamName(symbol), callback);
        }

        public virtual void Unsubscribe(Action<SymbolStatisticsEventArgs> callback)
        {
            Logger?.LogInformation($"{nameof(SymbolStatisticsWebSocketClient)}.{nameof(Unsubscribe)}: \"[All Symbols]\" (callback: {(callback == null ? "no" : "yes")}).  [thread: {Thread.CurrentThread.ManagedThreadId}]");

            UnsubscribeStream(GetStreamName(null), callback);
        }

        public virtual void Unsubscribe(string symbol, Action<SymbolStatisticsEventArgs> callback)
        {
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            symbol = symbol.FormatSymbol();

            Logger?.LogInformation($"{nameof(SymbolStatisticsWebSocketClient)}.{nameof(Unsubscribe)}: \"{symbol}\" (callback: {(callback == null ? "no" : "yes")}).  [thread: {Thread.CurrentThread.ManagedThreadId}]");

            UnsubscribeStream(GetStreamName(symbol), callback);
        }

        #endregion Public Methods

        #region Protected Methods

        protected override void OnWebSocketEvent(WebSocketStreamEventArgs args, IEnumerable<Action<SymbolStatisticsEventArgs>> callbacks)
        {
            Logger?.LogDebug($"{nameof(SymbolStatisticsWebSocketClient)}: \"{args.Json}\"");

            try
            {
                SymbolStatisticsEventArgs eventArgs;

                if (args.Json.IsJsonArray())
                {
                    // Simulate a single event time.
                    var eventTime = DateTime.UtcNow.ToTimestamp().ToDateTime();

                    var statistics = JArray.Parse(args.Json).Select(DeserializeSymbolStatistics).ToArray();

                    eventArgs = new SymbolStatisticsEventArgs(eventTime, args.Token, statistics);
                }
                else
                {
                    var jObject = JObject.Parse(args.Json);

                    var eventType = jObject["e"].Value<string>();

                    if (eventType == "24hrTicker")
                    {
                        var eventTime = jObject["E"].Value<long>().ToDateTime();

                        var statistics = DeserializeSymbolStatistics(jObject);

                        eventArgs = new SymbolStatisticsEventArgs(eventTime, args.Token, statistics);
                    }
                    else
                    {
                        Logger?.LogWarning($"{nameof(SymbolStatisticsWebSocketClient)}.{nameof(OnWebSocketEvent)}: Unexpected event type ({eventType}).");
                        return;
                    }
                }

                try
                {
                    if (callbacks != null)
                    {
                        foreach (var callback in callbacks)
                            callback(eventArgs);
                    }
                    StatisticsUpdate?.Invoke(this, eventArgs);
                }
                catch (OperationCanceledException) { }
                catch (Exception e)
                {
                    if (!args.Token.IsCancellationRequested)
                    {
                        Logger?.LogError(e, $"{nameof(SymbolStatisticsWebSocketClient)}: Unhandled aggregate trade event handler exception.");
                    }
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception e)
            {
                if (!args.Token.IsCancellationRequested)
                {
                    Logger?.LogError(e, $"{nameof(SymbolStatisticsWebSocketClient)}.{nameof(OnWebSocketEvent)}");
                }
            }
        }

        #endregion Protected Methods

        #region Private Methods

        private static string GetStreamName(string symbol)
            => symbol == null ? "!ticker@arr" : $"{symbol.ToLowerInvariant()}@ticker";

        private static SymbolStatistics DeserializeSymbolStatistics(JToken jToken)
        {
            return new SymbolStatistics(
                jToken["s"].Value<string>(),  // symbol
                TimeSpan.FromHours(24),       // period
                jToken["p"].Value<decimal>(), // price change
                jToken["P"].Value<decimal>(), // price change percent
                jToken["w"].Value<decimal>(), // weighted average price
                jToken["x"].Value<decimal>(), // previous day's close price
                jToken["c"].Value<decimal>(), // current day's close price (last price)
                jToken["Q"].Value<decimal>(), // close trade's quantity (last quantity)
                jToken["b"].Value<decimal>(), // bid price
                jToken["B"].Value<decimal>(), // bid quantity
                jToken["a"].Value<decimal>(), // ask price
                jToken["A"].Value<decimal>(), // ask quantity
                jToken["o"].Value<decimal>(), // open price
                jToken["h"].Value<decimal>(), // high price
                jToken["l"].Value<decimal>(), // low price
                jToken["v"].Value<decimal>(), // base asset volume
                jToken["q"].Value<decimal>(), // quote asset volume
                jToken["O"].Value<long>()
                    .ToDateTime(),            // open time
                jToken["C"].Value<long>()
                    .ToDateTime(),            // close time
                jToken["F"].Value<long>(),    // first trade ID
                jToken["L"].Value<long>(),    // last trade ID
                jToken["n"].Value<long>());   // trade count
        }

        #endregion Private Methods
    }
}
