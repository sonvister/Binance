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
    /// The default <see cref="IAggregateTradeClient"/> implementation.
    /// </summary>
    public class AggregateTradeClient : JsonClient<AggregateTradeEventArgs>, IAggregateTradeClient
    {
        #region Public Events

        public event EventHandler<AggregateTradeEventArgs> AggregateTrade;

        #endregion Public Events

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger"></param>
        public AggregateTradeClient(ILogger<AggregateTradeClient> logger = null)
            : base(logger)
        { }

        #endregion Construtors

        #region Public Methods

        public virtual void Subscribe(string symbol, Action<AggregateTradeEventArgs> callback)
        {
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            symbol = symbol.FormatSymbol();

            Logger?.LogDebug($"{nameof(AggregateTradeClient)}.{nameof(Subscribe)}: \"{symbol}\" (callback: {(callback == null ? "no" : "yes")}).  [thread: {Thread.CurrentThread.ManagedThreadId}]");

            SubscribeStream(GetStreamName(symbol), callback);
        }

        public virtual void Unsubscribe(string symbol, Action<AggregateTradeEventArgs> callback)
        {
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            symbol = symbol.FormatSymbol();

            Logger?.LogDebug($"{nameof(AggregateTradeClient)}.{nameof(Unsubscribe)}: \"{symbol}\" (callback: {(callback == null ? "no" : "yes")}).  [thread: {Thread.CurrentThread.ManagedThreadId}]");

            UnsubscribeStream(GetStreamName(symbol), callback);
        }

        #endregion Public Methods

        #region Protected Methods

        protected override Task HandleMessageAsync(IEnumerable<Action<AggregateTradeEventArgs>> callbacks, string stream, string json, CancellationToken token = default)
        {
            //Logger?.LogDebug($"{nameof(AggregateTradeClient)}: \"{json}\"  [thread: {Thread.CurrentThread.ManagedThreadId}]");

            try
            {
                var jObject = JObject.Parse(json);

                var eventType = jObject["e"].Value<string>();

                if (eventType == "aggTrade")
                {
                    var eventTime = jObject["E"].Value<long>().ToDateTime();

                    var trade = new AggregateTrade(
                        jObject["s"].Value<string>(),  // symbol
                        jObject["a"].Value<long>(),    // aggregate trade ID
                        jObject["p"].Value<decimal>(), // price
                        jObject["q"].Value<decimal>(), // quantity
                        jObject["f"].Value<long>(),    // first trade ID
                        jObject["l"].Value<long>(),    // last trade ID
                        jObject["T"].Value<long>()     // trade time
                            .ToDateTime(),
                        jObject["m"].Value<bool>(),    // is buyer the market maker?
                        jObject["M"].Value<bool>());   // is best price match?

                    var eventArgs = new AggregateTradeEventArgs(eventTime, token, trade);

                    try
                    {
                        if (callbacks != null)
                        {
                            foreach (var callback in callbacks)
                                callback(eventArgs);
                        }

                        AggregateTrade?.Invoke(this, eventArgs);
                    }
                    catch (Exception e)
                    {
                        Logger?.LogError(e, $"{nameof(AggregateTradeClient)}: Unhandled aggregate trade event handler exception.  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                    }
                }
                else
                {
                    Logger?.LogWarning($"{nameof(AggregateTradeClient)}.{nameof(HandleMessageAsync)}: Unexpected event type ({eventType}).  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                }
            }
            catch (Exception e)
            {
                Logger?.LogError(e, $"{nameof(AggregateTradeClient)}.{nameof(HandleMessageAsync)}: Failed.  [thread: {Thread.CurrentThread.ManagedThreadId}]");
            }

            return Task.CompletedTask;
        }

        #endregion Protected Methods

        #region Private Methods

        private static string GetStreamName(string symbol)
            => $"{symbol.ToLowerInvariant()}@aggTrade";

        #endregion Private Methods
    }
}
