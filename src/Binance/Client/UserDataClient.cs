using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Binance.Account;
using Binance.Account.Orders;
using Binance.Api;
using Binance.Client.Events;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

namespace Binance.Client
{
    /// <summary>
    /// The default <see cref="IUserDataClient"/> implementation.
    /// </summary>
    public class UserDataClient : JsonClient<UserDataEventArgs>, IUserDataClient
    {
        #region Public Events

        public event EventHandler<AccountUpdateEventArgs> AccountUpdate;

        public event EventHandler<OrderUpdateEventArgs> OrderUpdate;

        public event EventHandler<AccountTradeUpdateEventArgs> TradeUpdate;

        #endregion Public Events

        #region Public Properties

        public override IEnumerable<string> ObservedStreams
        {
            get
            {
                lock (_sync)
                {
                    return base.ObservedStreams
                        .Concat(_accountUpdateSubscribers.Keys)
                        .Concat(_orderUpdateSubscribers.Keys)
                        .Concat(_accountTradeUpdateSubscribers.Keys)
                        .Distinct()
                        .ToArray();
                }
            }
        }

        #endregion Public Properties

        #region Protected Fields

        protected readonly IDictionary<string, IBinanceApiUser> Users
            = new Dictionary<string, IBinanceApiUser>();

        #endregion Protected Fields

        #region Private Fields

        private readonly IDictionary<string, IList<Action<AccountUpdateEventArgs>>> _accountUpdateSubscribers;
        private readonly IDictionary<string, IList<Action<OrderUpdateEventArgs>>> _orderUpdateSubscribers;
        private readonly IDictionary<string, IList<Action<AccountTradeUpdateEventArgs>>> _accountTradeUpdateSubscribers;

        private readonly object _sync = new object();

        #endregion Private Fields

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger">The logger.</param>
        public UserDataClient(ILogger<UserDataClient> logger = null)
            : base(logger)
        {
            _accountUpdateSubscribers = new Dictionary<string, IList<Action<AccountUpdateEventArgs>>>();
            _orderUpdateSubscribers = new Dictionary<string, IList<Action<OrderUpdateEventArgs>>>();
            _accountTradeUpdateSubscribers = new Dictionary<string, IList<Action<AccountTradeUpdateEventArgs>>>();
        }

        #endregion Construtors

        #region Public Methods

        public virtual IUserDataClient Subscribe<TEventArgs>(string listenKey, IBinanceApiUser user, Action<TEventArgs> callback)
            where TEventArgs : UserDataEventArgs
        {
            Logger?.LogDebug($"{nameof(UserDataClient)}.{nameof(Subscribe)}: \"{listenKey}\" (callback: {(callback == null ? "no" : "yes")}).  [thread: {Thread.CurrentThread.ManagedThreadId}]");

            var type = typeof(TEventArgs);

            if (type == typeof(AccountUpdateEventArgs))
                // ReSharper disable once InconsistentlySynchronizedField
                Subscribe(listenKey, user, callback as Action<AccountUpdateEventArgs>, _accountUpdateSubscribers);
            else if (type == typeof(OrderUpdateEventArgs))
                // ReSharper disable once InconsistentlySynchronizedField
                Subscribe(listenKey, user, callback as Action<OrderUpdateEventArgs>, _orderUpdateSubscribers);
            else if (type == typeof(AccountTradeUpdateEventArgs))
                // ReSharper disable once InconsistentlySynchronizedField
                Subscribe(listenKey, user, callback as Action<AccountTradeUpdateEventArgs>, _accountTradeUpdateSubscribers);
            else
                Subscribe(listenKey, user, callback as Action<UserDataEventArgs>, null);

            return this;
        }

        public virtual IUserDataClient Unsubscribe<TEventArgs>(string listenKey, Action<TEventArgs> callback)
            where TEventArgs : UserDataEventArgs
        {
            Logger?.LogDebug($"{nameof(UserDataClient)}.{nameof(Unsubscribe)}: \"{listenKey}\" (callback: {(callback == null ? "no" : "yes")}).  [thread: {Thread.CurrentThread.ManagedThreadId}]");

            var type = typeof(TEventArgs);

            if (callback != null)
            {
                if (type == typeof(AccountUpdateEventArgs))
                    // ReSharper disable once InconsistentlySynchronizedField
                    Unsubscribe(listenKey, callback as Action<AccountUpdateEventArgs>, _accountUpdateSubscribers);
                else if (type == typeof(OrderUpdateEventArgs))
                    // ReSharper disable once InconsistentlySynchronizedField
                    Unsubscribe(listenKey, callback as Action<OrderUpdateEventArgs>, _orderUpdateSubscribers);
                else if (type == typeof(AccountTradeUpdateEventArgs))
                    // ReSharper disable once InconsistentlySynchronizedField
                    Unsubscribe(listenKey, callback as Action<AccountTradeUpdateEventArgs>, _accountTradeUpdateSubscribers);
                else
                    Unsubscribe(listenKey, callback as Action<UserDataEventArgs>, null);
            }
            else
            {
                // ReSharper disable once InconsistentlySynchronizedField
                Unsubscribe(listenKey, null, _accountUpdateSubscribers);
                // ReSharper disable once InconsistentlySynchronizedField
                Unsubscribe(listenKey, null, _orderUpdateSubscribers);
                // ReSharper disable once InconsistentlySynchronizedField
                Unsubscribe(listenKey, null, _accountTradeUpdateSubscribers);
                Unsubscribe<UserDataEventArgs>(listenKey, null, null);
            }

            return this;
        }

        public virtual new IUserDataClient Unsubscribe()
        {
            lock (_sync)
            {
                _accountUpdateSubscribers.Clear();
                _orderUpdateSubscribers.Clear();
                _accountTradeUpdateSubscribers.Clear();
            }

            return (IUserDataClient)base.Unsubscribe();
        }

        public void HandleListenKeyChange(string oldStreamName, string newStreamName)
        {
            lock (_sync)
            {
                if (Users.TryGetValue(oldStreamName, out var user))
                {
                    Users[newStreamName] = user;
                    Users.Remove(oldStreamName);
                }

                if (_accountUpdateSubscribers.TryGetValue(oldStreamName, out var accountUpdateCallbacks))
                {
                    _accountUpdateSubscribers[newStreamName] = accountUpdateCallbacks;
                    _accountUpdateSubscribers.Remove(oldStreamName);
                }

                if (_orderUpdateSubscribers.TryGetValue(oldStreamName, out var orderUpdateCallbacks))
                {
                    _orderUpdateSubscribers[newStreamName] = orderUpdateCallbacks;
                    _orderUpdateSubscribers.Remove(oldStreamName);
                }

                if (_accountTradeUpdateSubscribers.TryGetValue(oldStreamName, out var accountTradeCallbacks))
                {
                    _accountTradeUpdateSubscribers[newStreamName] = accountTradeCallbacks;
                    _accountTradeUpdateSubscribers.Remove(oldStreamName);
                }
            }

            ReplaceStreamName(oldStreamName, newStreamName);
        }

        #endregion Public Methods

        #region Protected Methods

        protected override Task HandleMessageAsync(IEnumerable<Action<UserDataEventArgs>> callbacks, string stream, string json, CancellationToken token = default)
        {
            if (!Users.ContainsKey(stream))
            {
                Logger?.LogError($"{nameof(UserDataClient)}.{nameof(HandleMessageAsync)}: Unknown listen key (\"{stream}\").  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                return Task.CompletedTask; // ignore.
            }

            var user = Users[stream];

            try
            {
                var jObject = JObject.Parse(json);

                var eventType = jObject["e"].Value<string>();
                var eventTime = jObject["E"].Value<long>().ToDateTime();

                // ReSharper disable once ConvertIfStatementToSwitchStatement
                if (eventType == "outboundAccountInfo")
                {
                    var commissions = new AccountCommissions(
                        jObject["m"].Value<int>(),  // maker
                        jObject["t"].Value<int>(),  // taker
                        jObject["b"].Value<int>(),  // buyer
                        jObject["s"].Value<int>()); // seller

                    var status = new AccountStatus(
                        jObject["T"].Value<bool>(),  // can trade
                        jObject["W"].Value<bool>(),  // can withdraw
                        jObject["D"].Value<bool>()); // can deposit

                    var balances = jObject["B"]
                        .Select(entry => new AccountBalance(
                            entry["a"].Value<string>(),   // asset
                            entry["f"].Value<decimal>(),  // free amount
                            entry["l"].Value<decimal>())) // locked amount
                        .ToList();

                    var eventArgs = new AccountUpdateEventArgs(eventTime, token, new AccountInfo(user, commissions, status, jObject["u"].Value<long>().ToDateTime(), balances));

                    try
                    {
                        // ReSharper disable once InconsistentlySynchronizedField
                        if (_accountUpdateSubscribers.TryGetValue(stream, out var subscribers))
                        {
                            foreach (var subcriber in subscribers)
                                subcriber(eventArgs);
                        }

                        if (callbacks != null)
                        {
                            foreach (var callback in callbacks)
                                callback(eventArgs);
                        }

                        AccountUpdate?.Invoke(this, eventArgs);
                    }
                    catch (OperationCanceledException) { }
                    catch (Exception e)
                    {
                        if (!token.IsCancellationRequested)
                        {
                            Logger?.LogError(e, $"{nameof(UserDataClient)}: Unhandled account update event handler exception.");
                        }
                    }
                }
                else if (eventType == "executionReport")
                {
                    var order = new Order(user);

                    FillOrder(order, jObject);

                    var executionType = ConvertOrderExecutionType(jObject["x"].Value<string>());
                    var rejectedReason = jObject["r"].Value<string>();
                    var newClientOrderId = jObject["c"].Value<string>();

                    if (executionType == OrderExecutionType.Trade) // trade update event.
                    {
                        var trade = new AccountTrade(
                            jObject["s"].Value<string>(),  // symbol
                            jObject["t"].Value<long>(),    // ID
                            jObject["i"].Value<long>(),    // order ID
                            jObject["L"].Value<decimal>(), // price (price of last filled trade)
                            jObject["z"].Value<decimal>(), // quantity (accumulated quantity of filled trades)
                            jObject["n"].Value<decimal>(), // commission
                            jObject["N"].Value<string>(),  // commission asset
                            jObject["T"].Value<long>()
                                .ToDateTime(),             // time
                            order.Side == OrderSide.Buy,   // is buyer
                            jObject["m"].Value<bool>(),    // is buyer maker
                            jObject["M"].Value<bool>());   // is best price

                        var quantityOfLastFilledTrade = jObject["l"].Value<decimal>();

                        var eventArgs = new AccountTradeUpdateEventArgs(eventTime, token, order, rejectedReason, newClientOrderId, trade, quantityOfLastFilledTrade);

                        try
                        {
                            // ReSharper disable once InconsistentlySynchronizedField
                            if (_accountTradeUpdateSubscribers.TryGetValue(stream, out var subscribers))
                            {
                                foreach (var subcriber in subscribers)
                                    subcriber(eventArgs);
                            }

                            if (callbacks != null)
                            {
                                foreach (var callback in callbacks)
                                    callback(eventArgs);
                            }

                            TradeUpdate?.Invoke(this, eventArgs);
                        }
                        catch (OperationCanceledException) { }
                        catch (Exception e)
                        {
                            if (!token.IsCancellationRequested)
                            {
                                Logger?.LogError(e, $"{nameof(UserDataClient)}: Unhandled trade update event handler exception.");
                            }
                        }
                    }
                    else // order update event.
                    {
                        var eventArgs = new OrderUpdateEventArgs(eventTime, token, order, executionType, rejectedReason, newClientOrderId);

                        try
                        {
                            // ReSharper disable once InconsistentlySynchronizedField
                            if (_orderUpdateSubscribers.TryGetValue(stream, out var subscribers))
                            {
                                foreach (var subcriber in subscribers)
                                    subcriber(eventArgs);
                            }

                            if (callbacks != null)
                            {
                                foreach (var callback in callbacks)
                                    callback(eventArgs);
                            }

                            OrderUpdate?.Invoke(this, eventArgs);
                        }
                        catch (OperationCanceledException) { }
                        catch (Exception e)
                        {
                            if (!token.IsCancellationRequested)
                            {
                                Logger?.LogError(e, $"{nameof(UserDataClient)}: Unhandled order update event handler exception.");
                            }
                        }
                    }
                }
                else
                {
                    Logger?.LogWarning($"{nameof(UserDataClient)}.{nameof(HandleMessageAsync)}: Unexpected event type ({eventType}).");
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception e)
            {
                if (!token.IsCancellationRequested)
                {
                    Logger?.LogError(e, $"{nameof(UserDataClient)}.{nameof(HandleMessageAsync)}");
                }
            }

            return Task.CompletedTask;
        }

        #endregion Protected Methods

        #region Private Methods

        private void Subscribe<TEventArgs>(string listenKey, IBinanceApiUser user, Action<TEventArgs> callback, IDictionary<string, IList<Action<TEventArgs>>> subscribers)
            where TEventArgs : UserDataEventArgs
        {
            Throw.IfNullOrWhiteSpace(listenKey, nameof(listenKey));
            Throw.IfNull(user, nameof(user));

            // Subscribe callback (if provided) to listen key.
            if (subscribers == null)
            {
                SubscribeStream(listenKey, callback as Action<UserDataEventArgs>);
            }
            else
            {
                lock (_sync)
                {
                    SubscribeStream(listenKey, callback, subscribers);
                }
            }

            // If listen key is new.
            // ReSharper disable once InvertIf
            if (!Users.ContainsKey(listenKey))
            {
                // If a listen key exists with user.
                if (Users.Any(_ => _.Value.Equals(user)))
                    throw new InvalidOperationException($"{nameof(UserDataClient)}.{nameof(Subscribe)}: A listen key is already subscribed for this user.");

                // Add listen key and user (for stream event handling).
                Users[listenKey] = user;
            }
        }

        private void Unsubscribe<TEventArgs>(string listenKey, Action<TEventArgs> callback, IDictionary<string, IList<Action<TEventArgs>>> subscribers)
        {
            Throw.IfNullOrWhiteSpace(listenKey, nameof(listenKey));

            if (subscribers == null)
            {
                // Unsubscribe callback (if provided) from listen key.
                UnsubscribeStream(listenKey, callback as Action<UserDataEventArgs>);
            }
            else
            {
                lock (_sync)
                {
                    UnsubscribeStream(listenKey, callback, subscribers);
                }
            }

            // If listen key was removed from subscribers (no callbacks).
            if (!ObservedStreams.Contains(listenKey))
            {
                // Remove listen key (and user).
                Users.Remove(listenKey);
            }
        }

        /// <summary>
        /// Deserialize and fill order instance.
        /// </summary>
        /// <param name="order"></param>
        /// <param name="jToken"></param>
        private static void FillOrder(Order order, JToken jToken)
        {
            order.Symbol = jToken["s"].Value<string>();
            order.Id = jToken["i"].Value<long>();
            order.Time = jToken["T"].Value<long>().ToDateTime();
            order.Price = jToken["p"].Value<decimal>();
            order.OriginalQuantity = jToken["q"].Value<decimal>();
            order.ExecutedQuantity = jToken["z"].Value<decimal>();
            order.Status = jToken["X"].Value<string>().ConvertOrderStatus();
            order.TimeInForce = jToken["f"].Value<string>().ConvertTimeInForce();
            order.Type = jToken["o"].Value<string>().ConvertOrderType();
            order.Side = jToken["S"].Value<string>().ConvertOrderSide();
            order.StopPrice = jToken["P"].Value<decimal>();
            order.IcebergQuantity = jToken["F"].Value<decimal>();
            order.ClientOrderId = jToken["C"].Value<string>();
        }

        /// <summary>
        /// Deserialize order execution type.
        /// </summary>
        /// <param name="executionType"></param>
        /// <returns></returns>
        private static OrderExecutionType ConvertOrderExecutionType(string executionType)
        {
            switch (executionType)
            {
                case "NEW": return OrderExecutionType.New;
                case "CANCELED": return OrderExecutionType.Cancelled;
                case "REPLACED": return OrderExecutionType.Replaced;
                case "REJECTED": return OrderExecutionType.Rejected;
                case "TRADE": return OrderExecutionType.Trade;
                case "EXPIRED": return OrderExecutionType.Expired;
                default:
                    throw new Exception($"Failed to convert order execution type: \"{executionType}\"");
            }
        }

        #endregion Private Methods
    }
}
