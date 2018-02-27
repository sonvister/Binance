using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Binance.Client.Events;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Binance.Client
{
    /// <summary>
    /// The default <see cref="IDepthClient"/> implementation.
    /// </summary>
    public class DepthClient : JsonClient<DepthUpdateEventArgs>, IDepthClient
    {
        #region Public Events

        public event EventHandler<DepthUpdateEventArgs> DepthUpdate;

        #endregion Public Events

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger"></param>
        public DepthClient(ILogger<DepthClient> logger = null)
            : base(logger)
        { }

        #endregion Construtors

        #region Public Methods

        public virtual IDepthClient Subscribe(string symbol, int limit, Action<DepthUpdateEventArgs> callback)
        {
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            symbol = symbol.FormatSymbol();

            Logger?.LogDebug($"{nameof(DepthClient)}.{nameof(Subscribe)}: \"{symbol}\" \"{limit}\" (callback: {(callback == null ? "no" : "yes")}).  [thread: {Thread.CurrentThread.ManagedThreadId}]");

            SubscribeStream(GetStreamName(symbol, limit), callback);

            return this;
        }

        public virtual IDepthClient Unsubscribe(string symbol, int limit, Action<DepthUpdateEventArgs> callback)
        {
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            symbol = symbol.FormatSymbol();

            Logger?.LogDebug($"{nameof(DepthClient)}.{nameof(Unsubscribe)}: \"{symbol}\" \"{limit}\" (callback: {(callback == null ? "no" : "yes")}).  [thread: {Thread.CurrentThread.ManagedThreadId}]");

            UnsubscribeStream(GetStreamName(symbol, limit), callback);

            return this;
        }

        public virtual new IDepthClient Unsubscribe() => (IDepthClient)base.Unsubscribe();

        #endregion Public Methods

        #region Protected Methods

        protected override Task HandleMessageAsync(IEnumerable<Action<DepthUpdateEventArgs>> callbacks, string stream, string json, CancellationToken token = default)
        {
            try
            {
                var jObject = JObject.Parse(json);

                var eventType = jObject["e"]?.Value<string>();

                DepthUpdateEventArgs eventArgs;

                switch (eventType)
                {
                    case null: // partial depth stream.
                    {
                        var symbol = stream.Split('@')[0].ToUpperInvariant();

                        // Simulate event time.
                        var eventTime = DateTime.UtcNow.ToTimestamp().ToDateTime();

                        var lastUpdateId = jObject["lastUpdateId"].Value<long>();

                        var bids = jObject["bids"].Select(entry => (entry[0].Value<decimal>(), entry[1].Value<decimal>())).ToArray();
                        var asks = jObject["asks"].Select(entry => (entry[0].Value<decimal>(), entry[1].Value<decimal>())).ToArray();

                        eventArgs = new DepthUpdateEventArgs(eventTime, token, symbol, lastUpdateId, lastUpdateId, bids, asks);
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

                        eventArgs = new DepthUpdateEventArgs(eventTime, token, symbol, firstUpdateId, lastUpdateId, bids, asks);
                        break;
                    }
                    default:
                        Logger?.LogWarning($"{nameof(DepthClient)}.{nameof(HandleMessageAsync)}: Unexpected event type ({eventType}).");
                        return Task.CompletedTask;
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
                    if (!token.IsCancellationRequested)
                    {
                        Logger?.LogWarning(e, $"{nameof(DepthClient)}: Unhandled depth update event handler exception.");
                    }
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception e)
            {
                if (!token.IsCancellationRequested)
                {
                    Logger?.LogError(e, $"{nameof(DepthClient)}.{nameof(HandleMessageAsync)}");
                }
            }

            return Task.CompletedTask;
        }

        #endregion Protected Methods

        #region Private Methods

        private static string GetStreamName(string symbol, int limit)
            => limit > 0 ? $"{symbol.ToLowerInvariant()}@depth{limit}" : $"{symbol.ToLowerInvariant()}@depth";

        #endregion Private Methods
    }
}
