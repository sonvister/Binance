using Binance.Api.WebSocket.Events;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Binance.Api.WebSocket
{
    /// <summary>
    /// A <see cref="IDepthWebSocketClient"/> implementation.
    /// </summary>
    public class DepthWebSocketClient : BinanceWebSocketClient, IDepthWebSocketClient
    {
        #region Public Events

        public event EventHandler<DepthUpdateEventArgs> DepthUpdate;

        #endregion Public Events

        #region Public Properties

        public string Symbol { get; private set; }

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger"></param>
        public DepthWebSocketClient(ILogger<DepthWebSocketClient> logger = null)
            : base(logger)
        { }

        #endregion Construtors

        #region Public Methods

        public virtual Task SubscribeAsync(string symbol, CancellationToken token = default)
            => SubscribeAsync(symbol, null, token);

        public virtual Task SubscribeAsync(string symbol, Action<DepthUpdateEventArgs> callback, CancellationToken token = default)
        {
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            Symbol = symbol.FormatSymbol();

            if (IsSubscribed)
                throw new InvalidOperationException($"{nameof(DepthWebSocketClient)} is already subscribed to symbol: \"{Symbol}\"");

            return SubscribeAsync($"{Symbol.ToLower()}@depth", json =>
            {
                try
                {
                    var eventArgs = DeserializeJson(json);
                    if (eventArgs == null)
                        return;

                    callback?.Invoke(eventArgs);
                    RaiseUpdateEvent(eventArgs);
                }
                catch (OperationCanceledException) { }
                catch (Exception e)
                {
                    LogException(e, $"{nameof(DepthWebSocketClient)}.{nameof(RaiseUpdateEvent)}");
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
        protected virtual DepthUpdateEventArgs DeserializeJson(string json)
        {
            Throw.IfNullOrWhiteSpace(json, nameof(json));

            try
            {
                Logger?.LogTrace($"{nameof(DepthWebSocketClient)}.{nameof(DeserializeJson)}: \"{json}\"");

                var jObject = JObject.Parse(json);

                var eventType = jObject["e"].Value<string>();

                if (eventType == "depthUpdate")
                {
                    var symbol = jObject["s"].Value<string>();
                    var eventTime = jObject["E"].Value<long>();

                    var firstUpdateId = jObject["U"].Value<long>();
                    var lastUpdateId = jObject["u"].Value<long>();

                    var bids = jObject["b"].Select(entry => (entry[0].Value<decimal>(), entry[1].Value<decimal>())).ToList();
                    var asks = jObject["a"].Select(entry => (entry[0].Value<decimal>(), entry[1].Value<decimal>())).ToList();

                    return new DepthUpdateEventArgs(eventTime, symbol, firstUpdateId, lastUpdateId, bids, asks);
                }

                Logger?.LogWarning($"{nameof(DepthWebSocketClient)}.{nameof(DeserializeJson)}: Unexpected event type ({eventType}).");
            }
            catch (OperationCanceledException) { }
            catch (Exception e)
            {
                LogException(e, $"{nameof(DepthWebSocketClient)}.{nameof(DeserializeJson)}");
                throw;
            }

            return null;
        }

        /// <summary>
        /// Raise depth of market update event.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void RaiseUpdateEvent(DepthUpdateEventArgs args)
        {
            Throw.IfNull(args, nameof(args));

            try { DepthUpdate?.Invoke(this, args); }
            catch (Exception e)
            {
                LogException(e, $"{nameof(DepthWebSocketClient)}.{nameof(RaiseUpdateEvent)}");
                throw;
            }
        }

        #endregion Protected Methods
    }
}
