using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Binance.Account;
using Binance.Account.Orders;
using Binance.Api;
using Binance.WebSocket.Events;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace Binance.WebSocket.UserData
{
    public abstract class UserDataWebSocketClient : BinanceWebSocketClient<UserDataEventArgs>, IUserDataWebSocketClient
    {
        #region Public Constants

        public static readonly int KeepAliveTimerPeriodMax = 3600000; // 1 hour
        public static readonly int KeepAliveTimerPeriodMin =   60000; // 1 minute
        public static readonly int KeepAliveTimerPeriodDefault = 1800000; // 30 minutes

        #endregion Public Constants

        #region Public Events

        public event EventHandler<AccountUpdateEventArgs> AccountUpdate;

        public event EventHandler<OrderUpdateEventArgs> OrderUpdate;

        public event EventHandler<AccountTradeUpdateEventArgs> TradeUpdate;

        #endregion Public Events

        #region Protected Fields

        protected readonly IBinanceApi _api;

        protected readonly UserDataWebSocketClientOptions _options;

        #endregion Protected Fields

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="api">The Binance API.</param>
        /// <param name="webSocket">The WebSocket stream.</param>
        /// <param name="options">The options.</param>
        /// <param name="logger">The logger.</param>
        protected UserDataWebSocketClient(IBinanceApi api, IWebSocketStream webSocket, IOptions<UserDataWebSocketClientOptions> options = null, ILogger<UserDataWebSocketClient> logger = null)
            : base(webSocket, logger)
        {
            Throw.IfNull(api, nameof(api));

            _api = api;
            _options = options?.Value;
        }

        #endregion Construtors

        #region Public Methods

        public abstract Task SubscribeAndStreamAsync(IBinanceApiUser user, Action<UserDataEventArgs> callback, CancellationToken token);

        #endregion Public Methods

        #region Protected Methods

        protected abstract IBinanceApiUser GetUserForEvent(WebSocketStreamEventArgs args);

        protected override void OnWebSocketEvent(WebSocketStreamEventArgs args, IEnumerable<Action<UserDataEventArgs>> callbacks)
        {
            var user = GetUserForEvent(args);

            Logger?.LogDebug($"{nameof(UserDataWebSocketClient)}: \"{args.Json}\"");

            try
            {
                var jObject = JObject.Parse(args.Json);

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

                    var eventArgs = new AccountUpdateEventArgs(eventTime, args.Token, new AccountInfo(user, commissions, status, jObject["u"].Value<long>().ToDateTime(), balances));

                    try
                    {
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
                        if (!args.Token.IsCancellationRequested)
                        {
                            Logger?.LogError(e, $"{nameof(UserDataWebSocketClient)}: Unhandled account update event handler exception.");
                        }
                    }
                }
                else if (eventType == "executionReport")
                {
                    var order = new Order(user);

                    FillOrder(order, jObject);

                    var executionType = ConvertOrderExecutionType(jObject["x"].Value<string>());
                    var rejectedReason = ConvertOrderRejectedReason(jObject["r"].Value<string>());
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

                        var eventArgs = new AccountTradeUpdateEventArgs(eventTime, args.Token, order, rejectedReason, newClientOrderId, trade, quantityOfLastFilledTrade);

                        try
                        {
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
                            if (!args.Token.IsCancellationRequested)
                            {
                                Logger?.LogError(e, $"{nameof(UserDataWebSocketClient)}: Unhandled trade update event handler exception.");
                            }
                        }
                    }
                    else // order update event.
                    {
                        var eventArgs = new OrderUpdateEventArgs(eventTime, args.Token, order, executionType, rejectedReason, newClientOrderId);

                        try
                        {
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
                            if (!args.Token.IsCancellationRequested)
                            {
                                Logger?.LogError(e, $"{nameof(UserDataWebSocketClient)}: Unhandled order update event handler exception.");
                            }
                        }
                    }
                }
                else
                {
                    Logger?.LogWarning($"{nameof(UserDataWebSocketClient)}.{nameof(OnWebSocketEvent)}: Unexpected event type ({eventType}) - \"{args.Json}\"");
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception e)
            {
                if (!args.Token.IsCancellationRequested)
                {
                    Logger?.LogError(e, $"{nameof(UserDataWebSocketClient)}.{nameof(OnWebSocketEvent)}");
                }
            }
        }

        #endregion Protected Methods

        #region Private Methods

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
                    Logger?.LogError($"Failed to convert order rejected reason: \"{rejectedReason}\"");
                    return OrderRejectedReason.None;
            }
        }

        #endregion Private Methods
    }
}
