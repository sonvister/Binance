using System;
using Binance.Api;

namespace Binance.Account.Orders
{
    /// <summary>
    /// An order that has been sent to the matching engine.
    /// </summary>
    public sealed class Order : IChronological
    {
        #region Public Properties

        /// <summary>
        /// Get the user.
        /// </summary>
        public IBinanceApiUser User { get; internal set; }

        /// <summary>
        /// Get the symbol.
        /// </summary>
        public string Symbol { get; internal set; }

        /// <summary>
        /// Get the ID.
        /// </summary>
        public long Id { get; internal set; }

        /// <summary>
        /// Get the client order ID.
        /// </summary>
        public string ClientOrderId { get; internal set; }

        /// <summary>
        /// Get the price.
        /// </summary>
        public decimal Price { get; internal set; }

        /// <summary>
        /// Get the original quantity.
        /// </summary>
        public decimal OriginalQuantity { get; internal set; }

        /// <summary>
        /// Get the executed quantity.
        /// </summary>
        public decimal ExecutedQuantity { get; internal set; }

        /// <summary>
        /// Get the order status.
        /// </summary>
        public OrderStatus Status { get; internal set; }

        /// <summary>
        /// Get the time in force.
        /// </summary>
        public TimeInForce TimeInForce { get; internal set; }

        /// <summary>
        /// Get the order type.
        /// </summary>
        public OrderType Type { get; internal set; }

        /// <summary>
        /// Get the order side.
        /// </summary>
        public OrderSide Side { get; internal set; }

        /// <summary>
        /// Get the stop price.
        /// </summary>
        public decimal StopPrice { get; internal set; }

        /// <summary>
        /// Get the iceberg quantity.
        /// </summary>
        public decimal IcebergQuantity { get; internal set; }

        /// <summary>
        /// Get the timestamp.
        /// </summary>
        public long Timestamp { get; internal set; }

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="user"></param>
        /// <param name="symbol"></param>
        /// <param name="id"></param>
        /// <param name="clientOrderId"></param>
        /// <param name="price"></param>
        /// <param name="originalQuantity"></param>
        /// <param name="executedQuantity"></param>
        /// <param name="status"></param>
        /// <param name="timeInForce"></param>
        /// <param name="orderType"></param>
        /// <param name="orderSide"></param>
        /// <param name="stopPrice"></param>
        /// <param name="icebergQuantity"></param>
        /// <param name="timestamp"></param>
        public Order(
            IBinanceApiUser user,
            string symbol,
            long id,
            string clientOrderId,
            decimal price,
            decimal originalQuantity,
            decimal executedQuantity,
            OrderStatus status,
            TimeInForce timeInForce,
            OrderType orderType,
            OrderSide orderSide,
            decimal stopPrice,
            decimal icebergQuantity,
            long timestamp)
            : this(user)
        {
            Throw.IfNullOrWhiteSpace(symbol, nameof(symbol));

            if (id < 0)
                throw new ArgumentException($"{nameof(Order)} ID must not be less than 0.", nameof(id));

            if (price < 0)
                throw new ArgumentException($"{nameof(Order)} price must not be less than 0.", nameof(price));
            if (stopPrice < 0)
                throw new ArgumentException($"{nameof(Order)} price must not be less than 0.", nameof(stopPrice));

            if (originalQuantity < 0)
                throw new ArgumentException($"{nameof(Order)} quantity must not be less than 0.", nameof(originalQuantity));
            if (executedQuantity < 0)
                throw new ArgumentException($"{nameof(Order)} quantity must not be less than 0.", nameof(executedQuantity));
            if (icebergQuantity < 0)
                throw new ArgumentException($"{nameof(Order)} quantity must not be less than 0.", nameof(icebergQuantity));

            if (timestamp <= 0)
                throw new ArgumentException($"{nameof(Order)}: timestamp must be greater than 0.", nameof(timestamp));

            Symbol = symbol;
            Id = id;
            ClientOrderId = clientOrderId;
            Price = price;
            OriginalQuantity = originalQuantity;
            ExecutedQuantity = executedQuantity;
            Status = status;
            TimeInForce = timeInForce;
            Type = orderType;
            Side = orderSide;
            StopPrice = stopPrice;
            IcebergQuantity = icebergQuantity;
            Timestamp = timestamp;
        }

        /// <summary>
        /// Internal constructor.
        /// </summary>
        internal Order(IBinanceApiUser user)
        {
            Throw.IfNull(user, nameof(user));

            User = user;
        }

        #endregion Constructors
    }
}
