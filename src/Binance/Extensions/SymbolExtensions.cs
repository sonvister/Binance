using System;
using System.Linq;
using Binance.Account.Orders;

// ReSharper disable once CheckNamespace
namespace Binance
{
    public static class SymbolExtensions
    {
        /// <summary>
        /// Determine if an order is valid for a symbol.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public static bool IsValid(this Symbol symbol, ClientOrder order)
        {
            Throw.IfNull(symbol, nameof(symbol));
            Throw.IfNull(order, nameof(order));

            return order.Symbol == symbol
                && IsOrderTypeSupported(symbol, order)
                && IsPriceQuantityValid(symbol, order);
        }

        /// <summary>
        /// Determine if a symbol supports an order type.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="orderType"></param>
        /// <returns></returns>
        public static bool IsSupported(this Symbol symbol, OrderType orderType)
        {
            Throw.IfNull(symbol, nameof(symbol));

            return symbol.OrderTypes.Contains(orderType);
        }

        /// <summary>
        /// Determine if a symbol supports a client order type.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public static bool IsOrderTypeSupported(this Symbol symbol, ClientOrder order)
        {
            Throw.IfNull(order, nameof(order));

            return IsSupported(symbol, order.Type);
        }

        /// <summary>
        /// Determine if order price and quantity are valid for a symbol.
        /// Price is ignored unless order is <see cref="LimitOrder"/>.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public static bool IsPriceQuantityValid(this Symbol symbol, ClientOrder order)
        {
            Throw.IfNull(symbol, nameof(symbol));
            Throw.IfNull(order, nameof(order));

            if (!symbol.BaseAsset.IsAmountValid(order.Quantity))
                return false;

            if (!symbol.Quantity.IsValid(order.Quantity))
                return false;

            var limitOrder = (order as LimitOrder);
            if (limitOrder == null)
                return true;

            if (!symbol.QuoteAsset.IsAmountValid(limitOrder.Price))
                return false;

            return symbol.Price.IsValid(limitOrder.Price)
                && limitOrder.Price * order.Quantity >= symbol.NotionalMinimumValue;
        }

        /// <summary>
        /// Determine if price and quantity are valid for a symbol.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="price"></param>
        /// <param name="quantity"></param>
        /// <returns></returns>
        public static bool IsPriceQuantityValid(this Symbol symbol, decimal price, decimal quantity)
        {
            Throw.IfNull(symbol, nameof(symbol));

            return symbol.QuoteAsset.IsAmountValid(price)
                && symbol.BaseAsset.IsAmountValid(quantity)
                && symbol.Price.IsValid(price)
                && symbol.Quantity.IsValid(quantity)
                && price * quantity >= symbol.NotionalMinimumValue;
        }

        /// <summary>
        /// Validate an order for a symbol.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="order"></param>
        public static void Validate(this Symbol symbol, ClientOrder order)
        {
            Throw.IfNull(symbol, nameof(symbol));
            Throw.IfNull(order, nameof(order));

            if (order.Symbol != symbol)
                throw new ArgumentException($"The order symbol ({order.Symbol ?? "null"}) does not match symbol ({symbol}).", nameof(order.Symbol));

            ValidateOrderType(symbol, order);

            ValidatePriceQuantity(symbol, order);
        }

        /// <summary>
        /// Validate an order type for a symbol.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="orderType"></param>
        public static void Validate(this Symbol symbol, OrderType orderType)
        {
            Throw.IfNull(symbol, nameof(symbol));

            if (!IsSupported(symbol, orderType))
                throw new ArgumentException($"The order type ({orderType.AsString()}) is not supported.", nameof(orderType));
        }

        /// <summary>
        /// Validate an order type for a symbol.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="order"></param>
        public static void ValidateOrderType(this Symbol symbol, ClientOrder order)
        {
            Throw.IfNull(symbol, nameof(symbol));
            Throw.IfNull(order, nameof(order));

            if (!IsOrderTypeSupported(symbol, order))
                throw new ArgumentException($"The order type ({order.Type.AsString()}) is not supported.", nameof(order.Type));
        }

        /// <summary>
        /// Validate order price and quantity for a symbol.
        /// Price is ignored unless order is <see cref="LimitOrder"/>.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="order"></param>
        public static void ValidatePriceQuantity(this Symbol symbol, ClientOrder order)
        {
            Throw.IfNull(symbol, nameof(symbol));
            Throw.IfNull(order, nameof(order));

            symbol.BaseAsset.ValidateAmount(order.Quantity, nameof(order.Quantity));
            symbol.Quantity.Validate(order.Quantity, nameof(order.Quantity));

            var limitOrder = (order as LimitOrder);
            if (limitOrder == null)
                return;

            symbol.QuoteAsset.ValidateAmount(limitOrder.Price, nameof(limitOrder.Price));
            symbol.Price.Validate(limitOrder.Price, nameof(limitOrder.Price));

            var notionalValue = limitOrder.Price * order.Quantity;
            if (notionalValue < symbol.NotionalMinimumValue)
                throw new ArgumentOutOfRangeException(nameof(notionalValue), $"The price * quantity ({notionalValue}) must be greater than or equal to minimum notional value ({symbol.NotionalMinimumValue}).");
        }

        /// <summary>
        /// Validate price and quantity for a symbol.
        /// </summary>
        /// <param name=""></param>
        /// <param name="price"></param>
        /// <param name="quantity"></param>
        public static void ValidatePriceQuantity(this Symbol symbol, decimal price, decimal quantity)
        {
            Throw.IfNull(symbol, nameof(symbol));

            symbol.QuoteAsset.ValidateAmount(price, nameof(price));
            symbol.BaseAsset.ValidateAmount(quantity, nameof (quantity));

            symbol.Price.Validate(price, nameof(price));
            symbol.Quantity.Validate(quantity, nameof(quantity));

            var notionalValue = price * quantity;
            if (notionalValue < symbol.NotionalMinimumValue)
                throw new ArgumentOutOfRangeException(nameof(notionalValue), $"The price * quantity ({notionalValue}) must be greater than or equal to minimum notional value ({symbol.NotionalMinimumValue}).");
        }
    }
}
