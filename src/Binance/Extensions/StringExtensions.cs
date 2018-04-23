using System;

// ReSharper disable once CheckNamespace
namespace Binance
{
    public static class StringExtensions
    {
        #region Public Extensions

        /// <summary>
        /// Return true if string is a JSON object.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsJsonObject(this string s)
        {
            return !string.IsNullOrWhiteSpace(s)
                && s.StartsWith("{") && s.EndsWith("}");
        }

        /// <summary>
        /// Return true if string is a JSON array.
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool IsJsonArray(this string s)
        {
            return !string.IsNullOrWhiteSpace(s)
                && s.StartsWith("[") && s.EndsWith("]");
        }

        /// <summary>
        /// Convert string to <see cref="CandlestickInterval"/>.
        /// </summary>
        /// <param name="s">The string.</param>
        /// <returns></returns>
        public static CandlestickInterval ToCandlestickInterval(this string s)
        {
            Throw.IfNullOrWhiteSpace(s, nameof(s));

            switch (s.Trim())
            {
                case "1m": return CandlestickInterval.Minute;
                case "3m": return CandlestickInterval.Minutes_3;
                case "5m": return CandlestickInterval.Minutes_5;
                case "15m": return CandlestickInterval.Minutes_15;
                case "30m": return CandlestickInterval.Minutes_30;
                case "60m":
                case "1h": return CandlestickInterval.Hour;
                case "2h": return CandlestickInterval.Hours_2;
                case "4h": return CandlestickInterval.Hours_4;
                case "6h": return CandlestickInterval.Hours_6;
                case "8h": return CandlestickInterval.Hours_8;
                case "12h": return CandlestickInterval.Hours_12;
                case "24h":
                case "1d": return CandlestickInterval.Day;
                case "3d": return CandlestickInterval.Days_3;
                case "1w": return CandlestickInterval.Week;
                case "1M": return CandlestickInterval.Month;
                default:
                    throw new ArgumentException($"{nameof(ToCandlestickInterval)}: interval not supported: {s}");
            }
        }

        #endregion Public Extensions

        #region Internal Extensions

        /// <summary>
        /// Attempt to transform a symbol string to an acceptable format.
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        internal static string FormatSymbol(this string symbol)
        {
            Throw.IfNullOrWhiteSpace(symbol);

            return string.Intern(symbol.Trim()
                .Replace(" ", string.Empty)
                .Replace("-", string.Empty)
                .Replace("_", string.Empty)
                .Replace("/", string.Empty)
                .ToUpperInvariant());
        }

        /// <summary>
        /// Deserialize symbol status.
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        internal static SymbolStatus ConvertSymbolStatus(this string status)
        {
            switch (status)
            {
                case "AUCTION_MATCH": return SymbolStatus.AuctionMatch;
                case "BREAK": return SymbolStatus.Break;
                case "END_OF_DAY": return SymbolStatus.EndOfDay;
                case "HALT": return SymbolStatus.Halt;
                case "POST_TRADING": return SymbolStatus.PostTrading;
                case "PRE_TRADING": return SymbolStatus.PreTrading;
                case "TRADING": return SymbolStatus.Trading;
                default:
                    throw new Exception($"Failed to convert symbol status: \"{status}\"");
            }
        }

        /// <summary>
        /// Deserialize order status.
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        internal static OrderStatus ConvertOrderStatus(this string status)
        {
            switch (status)
            {
                case "NEW": return OrderStatus.New;
                case "PARTIALLY_FILLED": return OrderStatus.PartiallyFilled;
                case "FILLED": return OrderStatus.Filled;
                case "CANCELED": return OrderStatus.Canceled;
                case "PENDING_CANCEL": return OrderStatus.PendingCancel;
                case "REJECTED": return OrderStatus.Rejected;
                case "EXPIRED": return OrderStatus.Expired;
                default:
                    throw new Exception($"Failed to convert order status: \"{status}\"");
            }
        }

        /// <summary>
        /// Deserialize order type.
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        internal static OrderType ConvertOrderType(this string type)
        {
            switch (type)
            {
                case "LIMIT": return OrderType.Limit;
                case "MARKET": return OrderType.Market;
                case "STOP_LOSS": return OrderType.StopLoss;
                case "STOP_LOSS_LIMIT": return OrderType.StopLossLimit;
                case "TAKE_PROFIT": return OrderType.TakeProfit;
                case "TAKE_PROFIT_LIMIT": return OrderType.TakeProfitLimit;
                case "LIMIT_MAKER": return OrderType.LimitMaker;
                default:
                    throw new Exception($"Failed to convert order type: \"{type}\"");
            }
        }

        /// <summary>
        /// Deserialize order side.
        /// </summary>
        /// <param name="side"></param>
        /// <returns></returns>
        internal static OrderSide ConvertOrderSide(this string side)
        {
            switch (side)
            {
                case "BUY": return OrderSide.Buy;
                case "SELL": return OrderSide.Sell;
                default:
                    throw new Exception($"Failed to convert order side: \"{side}\"");
            }
        }

        /// <summary>
        /// Deserialize time in force.
        /// </summary>
        /// <param name="timeInForce"></param>
        /// <returns></returns>
        internal static TimeInForce ConvertTimeInForce(this string timeInForce)
        {
            switch (timeInForce)
            {
                case "GTC": return TimeInForce.GTC;
                case "IOC": return TimeInForce.IOC;
                case "FOK": return TimeInForce.FOK;
                default:
                    throw new Exception($"Failed to convert time in force: \"{timeInForce}\"");
            }
        }

        #endregion Internal Extensions
    }
}
