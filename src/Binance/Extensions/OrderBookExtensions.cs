using System;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace Binance
{
    public static class OrderBookExtensions
    {
        /// <summary>
        /// Get the best (top) seller price and quantity.
        /// </summary>
        /// <param name="orderBook"></param>
        /// <returns></returns>
        public static OrderBookPriceLevel Ask(this OrderBook orderBook)
        {
            Throw.IfNull(orderBook, nameof(orderBook));

            return orderBook.Top.Ask;
        }

        /// <summary>
        /// Get the best (top) buyer price and quantity.
        /// </summary>
        /// <param name="orderBook"></param>
        /// <returns></returns>
        public static OrderBookPriceLevel Bid(this OrderBook orderBook)
        {
            Throw.IfNull(orderBook, nameof(orderBook));

            return orderBook.Top.Bid;
        }

        /// <summary>
        /// Get the price difference between the best (top) ask and bid prices.
        /// </summary>
        /// <param name="orderBook"></param>
        /// <returns></returns>
        public static decimal Spread(this OrderBook orderBook)
        {
            Throw.IfNull(orderBook, nameof(orderBook));

            return Spread(orderBook.Top);
        }

        /// <summary>
        /// Get the price difference between the best (top) ask and bid prices.
        /// </summary>
        /// <param name="top"></param>
        /// <returns></returns>
        public static decimal Spread(this OrderBookTop top)
        {
            Throw.IfNull(top, nameof(top));

            return top.Ask.Price - top.Bid.Price;
        }

        /// <summary>
        /// Get the mid-market price between the best (top) ask and bid prices.
        /// </summary>
        /// <param name="orderBook"></param>
        /// <returns></returns>
        public static decimal MidMarketPrice(this OrderBook orderBook)
        {
            Throw.IfNull(orderBook, nameof(orderBook));

            return MidMarketPrice(orderBook.Top);
        }

        /// <summary>
        /// Get the mid-market price between the best (top) ask and bid prices.
        /// </summary>
        /// <param name="top"></param>
        /// <returns></returns>
        public static decimal MidMarketPrice(this OrderBookTop top)
        {
            Throw.IfNull(top, nameof(top));

            return (top.Ask.Price + top.Bid.Price) / 2;
        }

        /// <summary>
        /// Get whether a price is the best bid or ask price.
        /// </summary>
        /// <param name="orderBook">The order book.</param>
        /// <param name="price">The price.</param>
        /// <returns></returns>
        public static bool IsBestPrice(this OrderBook orderBook, decimal price)
        {
            return IsBestPrice(orderBook.Top, price);
        }

        /// <summary>
        /// Get whether a price is the best bid or ask price.
        /// </summary>
        /// <param name="orderBookTop">The order book top.</param>
        /// <param name="price">The price.</param>
        /// <returns></returns>
        public static bool IsBestPrice(this OrderBookTop orderBookTop, decimal price)
        {
            return price == orderBookTop.Bid.Price || price == orderBookTop.Ask.Price;
        }

        /// <summary>
        /// Get the minimum bid or maximum ask price for a quantity.
        /// </summary>
        /// <param name="enumerable">The <see cref="OrderBookPriceLevel"/> enumerable.</param>
        /// <param name="quantity">The quantity.</param>
        /// <returns></returns>
        public static decimal PriceAt(this IEnumerable<OrderBookPriceLevel> enumerable, decimal quantity)
        {
            Throw.IfNull(enumerable, nameof(enumerable));

            // ReSharper disable once PossibleMultipleEnumeration
            if (!enumerable.Any())
            {
                throw new ArgumentException("OrderBookPriceLevel enumerable must not be empty.", nameof(enumerable));
            }

            decimal sum = 0;
            // ReSharper disable once PossibleMultipleEnumeration
            return enumerable.TakeWhile(_ =>
            {
                if (sum >= quantity) return false;
                sum += _.Quantity;
                return true;
            }).Last().Price;
        }
    }
}
