using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Binance.WebSocket.Events;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Binance.WebSocket
{
    /// <summary>
    /// A <see cref="IDepthWebSocketClient"/> implementation.
    /// </summary>
    public class DepthWebSocketClient : BinanceWebSocketClient<DepthUpdateEventArgs>, IDepthWebSocketClient
    {
        #region Public Events

        public event EventHandler<DepthUpdateEventArgs> DepthUpdate;

        #endregion Public Events

        #region Constructors

        /// <summary>
        /// Default constructor provides default web socket stream, but no logging.
        /// </summary>
        public DepthWebSocketClient()
            : this(new BinanceWebSocketStream())
        { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="logger"></param>
        public DepthWebSocketClient(IWebSocketStream stream, ILogger<DepthWebSocketClient> logger = null)
            : base(stream, logger)
        { }

        #endregion Construtors

        #region Public Methods

        public virtual void Subscribe(string symbol, int limit, Action<DepthUpdateEventArgs> callback)
        {
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            symbol = symbol.FormatSymbol();

            Logger?.LogDebug($"{nameof(DepthWebSocketClient)}.{nameof(Subscribe)}: \"{symbol}\" \"{limit}\" (callback: {(callback == null ? "no" : "yes")}).  [thread: {Thread.CurrentThread.ManagedThreadId}]");

            SubscribeStream(GetStreamName(symbol, limit), callback);
        }

        public virtual void Unsubscribe(string symbol, int limit, Action<DepthUpdateEventArgs> callback)
        {
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            symbol = symbol.FormatSymbol();

            Logger?.LogDebug($"{nameof(DepthWebSocketClient)}.{nameof(Unsubscribe)}: \"{symbol}\" \"{limit}\" (callback: {(callback == null ? "no" : "yes")}).  [thread: {Thread.CurrentThread.ManagedThreadId}]");

            UnsubscribeStream(GetStreamName(symbol, limit), callback);
        }

        #endregion Public Methods

        #region Protected Methods

        protected override void OnWebSocketEvent(WebSocketStreamEventArgs args, IEnumerable<Action<DepthUpdateEventArgs>> callbacks)
        {
            Logger?.LogDebug($"{nameof(DepthWebSocketClient)}: \"{args.Json}\"");

            try
            {
                var jObject = JObject.Parse(args.Json);

                var eventType = jObject["e"]?.Value<string>();

                DepthUpdateEventArgs eventArgs;

                switch (eventType)
                {
                    case null: // partial depth stream.
                    {
                        var symbol = args.StreamName.Split('@')[0].ToUpperInvariant();
                        
                        // Simulate event time.
                        var eventTime = DateTime.UtcNow.ToTimestamp().ToDateTime();

                        var lastUpdateId = jObject["lastUpdateId"].Value<long>();

                        var bids = jObject["bids"].Select(entry => (entry[0].Value<decimal>(), entry[1].Value<decimal>())).ToArray();
                        var asks = jObject["asks"].Select(entry => (entry[0].Value<decimal>(), entry[1].Value<decimal>())).ToArray();

                        eventArgs = new DepthUpdateEventArgs(eventTime, args.Token, symbol, lastUpdateId, lastUpdateId, bids, asks);
                        break;
                    }
                    case "depthUpdate":
                    {
                        var symbol = jObject["s"].Value<string>();
                        var eventTime = jObject["E"].Value<long>().ToDateTime();

                        var firstUpdateId = jObject["U"].Value<long>();
                        var lastUpdateId = jObject["u"].Value<long>();

                        var bids = jObject["b"].Select(entry => (entry[0].Value<decimal>(), entry[1].Value<decimal>())).ToArray();
                        var asks = jObject["a"].Select(entry => (entry[0].Value<decimal>(), entry[1].Value<decimal>())).ToArray();

                        eventArgs = new DepthUpdateEventArgs(eventTime, args.Token, symbol, firstUpdateId, lastUpdateId, bids, asks);
                        break;
                    }
                    default:
                        Logger?.LogWarning($"{nameof(DepthWebSocketClient)}.{nameof(OnWebSocketEvent)}: Unexpected event type ({eventType}).");
                        return;
                }

                try
                {
                    if (callbacks != null)
                    {
                        foreach (var callback in callbacks)
                            callback(eventArgs);
                    }
                    DepthUpdate?.Invoke(this, eventArgs);
                }
                catch (OperationCanceledException) { }
                catch (Exception e)
                {
                    if (!args.Token.IsCancellationRequested)
                    {
                        Logger?.LogError(e, $"{nameof(DepthWebSocketClient)}: Unhandled depth update event handler exception.");
                    }
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception e)
            {
                if (!args.Token.IsCancellationRequested)
                {
                    Logger?.LogError(e, $"{nameof(DepthWebSocketClient)}.{nameof(OnWebSocketEvent)}");
                }
            }
        }

        #endregion Protected Methods

        #region Private Methods

        private static string GetStreamName(string symbol, int limit)
            => limit > 0 ? $"{symbol.ToLowerInvariant()}@depth{limit}" : $"{symbol.ToLowerInvariant()}@depth";

        #endregion Private Methods
    }
}
