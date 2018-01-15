using System;
using Binance.Account.Orders;

// ReSharper disable once CheckNamespace
namespace Binance
{
    public static class OrderStatusExtensions
    {
        /// <summary>
        /// Convert <see cref="OrderStatus"/> to string.
        /// </summary>
        /// <param name="orderStatus"></param>
        /// <returns></returns>
        public static string AsString(this OrderStatus orderStatus)
        {
            switch (orderStatus)
            {
                case OrderStatus.Canceled: return "CANCELED";
                case OrderStatus.Expired: return "EXPIRED";
                case OrderStatus.Filled: return "FILLED";
                case OrderStatus.New: return "NEW";
                case OrderStatus.PartiallyFilled: return "PARTIALLY_FILLED";
                case OrderStatus.PendingCancel: return "PENDING_CANCEL";
                case OrderStatus.Rejected: return "REJECTED";
                default:
                    throw new ArgumentException($"{nameof(OrderStatusExtensions)}.{nameof(AsString)}: {nameof(OrderStatus)} not supported: {orderStatus}");
            }
        }
    }
}
