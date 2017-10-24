using Binance.Accounts;
using Binance.Api.WebSocket.Events;
using Binance.Orders;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Binance.Api.WebSocket
{
    /// <summary>
    /// A <see cref="IUserDataWebSocketClient"/> implementation.
    /// </summary>
    public class UserDataWebSocketClient : BinanceWebSocketClient, IUserDataWebSocketClient
    {
        #region Public Events

        public event EventHandler<AccountUpdateEventArgs> AccountUpdate;

        public event EventHandler<OrderUpdateEventArgs> OrderUpdate;

        public event EventHandler<TradeUpdateEventArgs> TradeUpdate;

        #endregion Public Events

        #region Public Properties

        public IBinanceUser User { get; private set; }

        #endregion Public Properties

        #region Private Fields

        private IBinanceApi _api;

        private string _listenKey;

        private Timer _keepAliveTimer;

        #endregion Private Fields

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="api">The Binance API.</param>
        /// <param name="logger">The logger.</param>
        public UserDataWebSocketClient(IBinanceApi api, ILogger<TradesWebSocketClient> logger = null)
            : base(logger)
        {
            _api = api;
        }

        #endregion Construtors

        #region Public Methods

        public virtual async Task SubscribeAsync(IBinanceUser user, CancellationToken token = default)
        {
            Throw.IfNull(user, nameof(user));

            User = user;

            if (_isSubscribed)
                throw new InvalidOperationException($"{nameof(UserDataWebSocketClient)} is already subscribed to a user.");

            _listenKey = await _api.UserStreamStartAsync(user, token)
                .ConfigureAwait(false);

            _keepAliveTimer = new Timer(OnKeepAliveTimer, token, 30000, 30000);

            await SubscribeAsync(_listenKey, json =>
            {
                try { DeserializeJsonAndRaiseEvent(json); }
                catch (OperationCanceledException) { }
                catch (Exception e)
                {
                    LogException(e, $"{nameof(UserDataWebSocketClient)}");
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
        protected virtual void DeserializeJsonAndRaiseEvent(string json)
        {
            Throw.IfNullOrWhiteSpace(json, nameof(json));

            try
            {
                _logger?.LogTrace($"{nameof(UserDataWebSocketClient)}: \"{json}\"");

                var jObject = JObject.Parse(json);

                var eventType = jObject["e"].Value<string>();
                var eventTime = jObject["E"].Value<long>();

                if (eventType == "outboundAccountInfo")
                {
                    var commissions = new AccountCommissions(
                        jObject["m"].Value<int>(),
                        jObject["t"].Value<int>(),
                        jObject["b"].Value<int>(),
                        jObject["s"].Value<int>());

                    var status = new AccountStatus(
                        jObject["T"].Value<bool>(),
                        jObject["W"].Value<bool>(),
                        jObject["D"].Value<bool>());

                    var balances = new List<AccountBalance>();
                    foreach (var entry in jObject["B"])
                    {
                        balances.Add(new AccountBalance(
                            entry["a"].Value<string>(),
                            entry["f"].Value<decimal>(),
                            entry["l"].Value<decimal>()));
                    }

                    RaiseAccountUpdateEvent(new AccountUpdateEventArgs(eventTime, new Account(commissions, status, balances)));
                }
                else if (eventType == "executionReport")
                {
                    var order = new Order();

                    FillOrder(order, jObject);

                    var executionType = ConvertOrderExecutionType(jObject["x"].Value<string>());
                    var rejectedReason = ConvertOrderRejectedReason(jObject["r"].Value<string>());

                    // Should this or jObject["c"] be on the order?
                    var origClientOrderId = jObject["C"].Value<string>();

                    if (executionType == OrderExecutionType.Trade) // trade update event.
                    {
                        var trade = new AccountTrade(
                            jObject["s"].Value<string>(),  // symbol
                            jObject["t"].Value<long>(),    // ID
                            jObject["L"].Value<decimal>(), // price (price of last filled trade) // TODO
                            jObject["z"].Value<decimal>(), // quantity (accumulated quantity of filled trades)
                            jObject["n"].Value<decimal>(), // commission
                            jObject["N"].Value<string>(),  // commission asset
                            jObject["T"].Value<long>(),    // timestamp
                            jObject["w"].Value<bool>(),    // is buyer // TODO
                            jObject["m"].Value<bool>(),    // is buyer maker
                            jObject["M"].Value<bool>());   // is best price // TODO
                        
                        var quantityOfLastFilledTrade = jObject["l"].Value<decimal>(); // TODO

                        RaiseTradeUpdateEvent(new TradeUpdateEventArgs(eventTime, order, executionType, rejectedReason, trade));
                    }
                    else // order update event.
                    {
                        RaiseOrderUpdateEvent(new OrderUpdateEventArgs(eventTime, order, executionType, rejectedReason));
                    }
                }
                else
                {
                    _logger?.LogWarning($"{nameof(UserDataWebSocketClient)}.{nameof(DeserializeJsonAndRaiseEvent)}: Unexpected event type ({eventType}) - \"{json}\"");
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception e)
            {
                LogException(e, $"{nameof(UserDataWebSocketClient)}.{nameof(DeserializeJsonAndRaiseEvent)}");
                throw;
            }
        }

        /// <summary>
        /// Raise account update event.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void RaiseAccountUpdateEvent(AccountUpdateEventArgs args)
        {
            Throw.IfNull(args, nameof(args));

            try { AccountUpdate?.Invoke(this, args); }
            catch (Exception e)
            {
                LogException(e, $"{nameof(UserDataWebSocketClient)}.{nameof(RaiseAccountUpdateEvent)}");
                throw;
            }
        }

        /// <summary>
        /// Raise order update event.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void RaiseOrderUpdateEvent(OrderUpdateEventArgs args)
        {
            Throw.IfNull(args, nameof(args));

            try { OrderUpdate?.Invoke(this, args); }
            catch (Exception e)
            {
                LogException(e, $"{nameof(UserDataWebSocketClient)}.{nameof(RaiseOrderUpdateEvent)}");
                throw;
            }
        }

        /// <summary>
        /// Raise trade update event.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void RaiseTradeUpdateEvent(TradeUpdateEventArgs args)
        {
            Throw.IfNull(args, nameof(args));

            try { TradeUpdate?.Invoke(this, args); }
            catch (Exception e)
            {
                LogException(e, $"{nameof(UserDataWebSocketClient)}.{nameof(RaiseTradeUpdateEvent)}");
                throw;
            }
        }

        #endregion Protected Methods

        #region Private Methods

        private async void OnKeepAliveTimer(object state)
        {
            try
            {
                await _api.UserStreamKeepAliveAsync(User, _listenKey, (CancellationToken)state)
                    .ConfigureAwait(false);
            }
            catch (Exception e)
            {
                LogException(e, $"{nameof(UserDataWebSocketClient)}.{nameof(OnKeepAliveTimer)}");
            }
        }

        private void FillOrder(Order order, JToken jToken)
        {
            order.Symbol = jToken["s"].Value<string>();
            order.Id = jToken["i"].Value<long>();
            order.Timestamp = jToken["T"].Value<long>();
            order.Price = jToken["p"].Value<decimal>();
            order.OriginalQuantity = jToken["q"].Value<decimal>();
            order.Status = jToken["X"].Value<string>().ConvertOrderStatus();
            order.TimeInForce = jToken["f"].Value<string>().ConvertTimeInForce();
            order.Type = jToken["o"].Value<string>().ConvertOrderType();
            order.Side = jToken["S"].Value<string>().ConvertOrderSide();

            // TODO
            order.ClientOrderId = jToken["c"].Value<string>();
            //order.ClientOrderId = jToken["C"].Value<string>();

            // TODO
            //order.ExecutedQuantity =
            //order.StopPrice = 
            //order.IcebergQuantity =

            // TODO
            //jObject["P"] // ignore?
            //jObject["F"] // ignore?
            //jObject["g"] // ignore?
            //jObject["I"] // ignore?
        }

        /// <summary>
        /// Deserialize order execution type.
        /// </summary>
        /// <param name="executionType"></param>
        /// <returns></returns>
        private OrderExecutionType ConvertOrderExecutionType(string executionType)
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

        /// <summary>
        /// Deserialize order rejected reason.
        /// </summary>
        /// <param name="rejectedReason"></param>
        /// <returns></returns>
        private OrderRejectedReason ConvertOrderRejectedReason(string rejectedReason)
        {
            switch (rejectedReason)
            {
                case "NONE": return OrderRejectedReason.None;
                case "UNKNOWN_INSTRUMENT": return OrderRejectedReason.UnknownInstrument;
                case "MARKET_CLOSED": return OrderRejectedReason.MarketClosed;
                case "PRICE_QTY_EXCEED_HARD_LIMITS": return OrderRejectedReason.PriceQuantityExceedHardLimits;
                case "UNKNOWN_ORDER": return OrderRejectedReason.UnknownOrder;
                case "DUPLICATE_ORDER": return OrderRejectedReason.DuplicateOrder;
                case "UNKNOWN_ACCOUNT": return OrderRejectedReason.UnknownAccount;
                case "INSUFFICIENT_BALANCE": return OrderRejectedReason.InsufficientBalance;
                case "ACCOUNT_INACTIVE": return OrderRejectedReason.AccountInactive;
                case "ACCOUNT_CANNOT_SETTLE": return OrderRejectedReason.AccountCannotSettle;
                default:
                    _logger?.LogError($"Failed to convert order rejected reason: \"{rejectedReason}\"");
                    return OrderRejectedReason.None;
            }
        }

        #endregion Private Methods

        #region IDisposable

        private bool _disposed = false;

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                try
                {
                    _keepAliveTimer?.Dispose();

                    if (!string.IsNullOrWhiteSpace(_listenKey))
                    {
                        _api.UserStreamCloseAsync(User, _listenKey)
                            .GetAwaiter().GetResult();
                    }
                }
                catch (OperationCanceledException) { }
                catch (Exception e)
                {
                    _logger?.LogError(e, $"{nameof(UserDataWebSocketClient)}.{nameof(Dispose)}: \"{e.Message}\"");
                }
            }

            _disposed = true;

            base.Dispose(disposing);
        }

        #endregion IDisposable
    }
}
