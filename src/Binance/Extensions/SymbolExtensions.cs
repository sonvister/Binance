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
        /// Determine if a symbol supports a client order type.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        public static bool IsOrderTypeSupported(this Symbol symbol, ClientOrder order)
        {
            Throw.IfNull(order, nameof(order));

            return IsOrderTypeSupported(symbol, order.Type);
        }

        /// <summary>
        /// Determine if a symbol supports an order type.
        /// </summary>
        /// <param name="symbol"></param>
        /// <param name="orderType"></param>
        /// <returns></returns>
        public static bool IsOrderTypeSupported(this Symbol symbol, OrderType orderType)
        {
            Throw.IfNull(symbol, nameof(symbol));

            return symbol.OrderTypes.Contains(orderType);
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
    }
}
