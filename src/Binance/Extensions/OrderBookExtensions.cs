using Binance.Market;

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
    }
}
