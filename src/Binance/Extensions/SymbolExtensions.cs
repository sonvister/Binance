using System.Linq;
using Binance.Account.Orders;

// ReSharper disable once CheckNamespace
namespace Binance
{
    public static class SymbolExtensions
    {
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
    }
}
