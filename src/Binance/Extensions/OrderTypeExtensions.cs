using System;
using Binance.Account.Orders;

// ReSharper disable once CheckNamespace
namespace Binance
{
    public static class OrderTypeExtensions
    {
        /// <summary>
        /// Convert <see cref="OrderType"/> to string.
        /// </summary>
        /// <param name="orderType"></param>
        /// <returns></returns>
        public static string AsString(this OrderType orderType)
        {
            switch (orderType)
            {
                case OrderType.Limit: return "LIMIT";
                case OrderType.Market: return "MARKET";
                case OrderType.StopLoss: return "STOP_LOSS";
                case OrderType.StopLossLimit: return "STOP_LOSS_LIMIT";
                case OrderType.TakeProfit: return "TAKE_PROFIT";
                case OrderType.TakeProfitLimit: return "TAKE_PROFIT_LIMIT";
                case OrderType.LimitMaker: return "LIMIT_MAKER";
                default:
                    throw new ArgumentException($"{nameof(OrderTypeExtensions)}.{nameof(ToString)}: {nameof(OrderType)} not supported: {orderType}");
            }
        }
    }
}
