using Binance;
using System;
using System.IO;
using System.Linq;
using System.Text;

namespace BinanceMarketDepth
{
    public static class OrderBookExtensions
    {
        /// <summary>
        /// Display the order book with the specifed writer (e.g. Console.Out).
        /// </summary>
        /// <param name="orderBook"></param>
        /// <param name="writer"></param>
        /// <param name="limit"></param>
        public static void Print(this IOrderBook orderBook, TextWriter writer, int limit = 25)
        {
            if (orderBook == null)
                throw new ArgumentNullException(nameof(orderBook));

            if (limit <= 0)
                throw new ArgumentException("Print limit must be greater than zero.", nameof(limit));

            var asks = orderBook.Asks.Take(limit).Reverse();
            var bids = orderBook.Bids.Take(limit);

            foreach (var ask in asks)
            {
                var bars = new StringBuilder("|");
                if (ask.Quantity > 50)
                {
                    bars.Append("---------------------  50 +  ---------------------");
                }
                else
                {
                    var size = ask.Quantity;

                    while (size-- >= 1)
                        bars.Append("-");

                    while (bars.Length < 50)
                        bars.Append(" ");
                }

                writer.WriteLine($" {ask.Price.ToString("0.00000000").PadLeft(14)} {ask.Quantity.ToString("0.00000000").PadLeft(15)}  {bars}");
            }

            writer.WriteLine();
            writer.WriteLine($"  {orderBook.MidMarketPrice().ToString("0.0000000000").PadLeft(16)} {orderBook.Spread().ToString("0.00000000").PadLeft(17)} Spread");
            writer.WriteLine();

            foreach (var bid in bids)
            {
                var bars = new StringBuilder("|");
                if (bid.Quantity > 50)
                {
                    bars.Append("---------------------  50 +  ---------------------");
                }
                else
                {
                    var size = bid.Quantity;

                    while (size-- >= 1)
                        bars.Append("-");

                    while (bars.Length < 50)
                        bars.Append(" ");
                }

                writer.WriteLine($" {bid.Price.ToString("0.00000000").PadLeft(14)} {bid.Quantity.ToString("0.00000000").PadLeft(15)}  {bars}");
            }
        }
    }
}
