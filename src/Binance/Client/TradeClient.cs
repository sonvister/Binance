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
    /// The default <see cref="ITradeClient"/> implementation.
    /// </summary>
    public class TradeClient : JsonClient<TradeEventArgs>, ITradeClient
    {
        #region Public Events

        public event EventHandler<TradeEventArgs> Trade;

        #endregion Public Events

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger"></param>
        public TradeClient(ILogger<TradeClient> logger = null)
            : base(logger)
        { }

        #endregion Construtors

        #region Public Methods

        public virtual ITradeClient Subscribe(string symbol, Action<TradeEventArgs> callback)
        {
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            symbol = symbol.FormatSymbol();

            Logger?.LogDebug($"{nameof(TradeClient)}.{nameof(Subscribe)}: \"{symbol}\" (callback: {(callback == null ? "no" : "yes")}).  [thread: {Thread.CurrentThread.ManagedThreadId}]");

            SubscribeStream(GetStreamName(symbol), callback);

            return this;
        }

        public virtual ITradeClient Unsubscribe(string symbol, Action<TradeEventArgs> callback)
        {
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            symbol = symbol.FormatSymbol();

            Logger?.LogDebug($"{nameof(TradeClient)}.{nameof(Unsubscribe)}: \"{symbol}\" (callback: {(callback == null ? "no" : "yes")}).  [thread: {Thread.CurrentThread.ManagedThreadId}]");

            UnsubscribeStream(GetStreamName(symbol), callback);

            return this;
        }

        public virtual new ITradeClient Unsubscribe() => (ITradeClient)base.Unsubscribe();

        #endregion Public Methods

        #region Protected Methods

        protected override Task HandleMessageAsync(IEnumerable<Action<TradeEventArgs>> callbacks, string stream, string json, CancellationToken token = default)
        {
            try
            {
                var jObject = JObject.Parse(json);

                var eventType = jObject["e"].Value<string>();

                if (eventType == "trade")
                {
                    var eventTime = jObject["E"].Value<long>().ToDateTime();

                    var trade = new Trade(
                        jObject["s"].Value<string>(),  // symbol
                        jObject["t"].Value<long>(),    // trade ID
                        jObject["p"].Value<decimal>(), // price
                        jObject["q"].Value<decimal>(), // quantity
                        jObject["b"].Value<long>(),    // buyer order ID
                        jObject["a"].Value<long>(),    // seller order ID
                        jObject["T"].Value<long>()
                            .ToDateTime(),             // trade time
                        jObject["m"].Value<bool>(),    // is buyer the market maker?
                        jObject["M"].Value<bool>());   // is best price match?

                    var eventArgs = new TradeEventArgs(eventTime, token, trade);

                    try
                    {
                        if (callbacks != null)
                        {
                            foreach (var callback in callbacks)
                                callback(eventArgs);
                        }
                        Trade?.Invoke(this, eventArgs);
                    }
                    catch (OperationCanceledException) { }
                    catch (Exception e)
                    {
                        if (!token.IsCancellationRequested)
                        {
                            Logger?.LogWarning(e, $"{nameof(TradeClient)}: Unhandled aggregate trade event handler exception.");
                        }
                    }
                }
                else
                {
                    Logger?.LogWarning($"{nameof(TradeClient)}.{nameof(HandleMessageAsync)}: Unexpected event type ({eventType}).");
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception e)
            {
                if (!token.IsCancellationRequested)
                {
                    Logger?.LogError(e, $"{nameof(TradeClient)}.{nameof(HandleMessageAsync)}");
                }
            }

            return Task.CompletedTask;
        }

        #endregion Protected Methods

        #region Private Methods

        private static string GetStreamName(string symbol)
            => $"{symbol.ToLowerInvariant()}@trade";

        #endregion Private Methods
    }
}
