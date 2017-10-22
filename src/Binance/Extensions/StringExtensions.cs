using System;

namespace Binance
{
    public static class StringExtensions
    {
        /// <summary>
        /// Convert string to <see cref="KlineInterval"/>.
        /// </summary>
        /// <param name="s">The string.</param>
        /// <returns></returns>
        public static KlineInterval ToKlineInterval(this string s)
        {
            Throw.IfNullOrWhiteSpace(s, nameof(s));

            switch (s.Trim().ToLower())
            {
                case "1m": return KlineInterval.Minute;
                case "3m": return KlineInterval.Minutes_3;
                case "5m": return KlineInterval.Minutes_5;
                case "15m": return KlineInterval.Minutes_15;
                case "30m": return KlineInterval.Minutes_30;
                case "60m": return KlineInterval.Hour;
                case "2h": return KlineInterval.Hours_2;
                case "4h": return KlineInterval.Hours_4;
                case "8h": return KlineInterval.Hours_8;
                case "12h": return KlineInterval.Hours_12;
                case "1d": return KlineInterval.Day;
                case "3d": return KlineInterval.Days_3;
                case "1w": return KlineInterval.Week;
                case "1M": return KlineInterval.Month;
                default:
                    throw new ArgumentException($"{nameof(ToKlineInterval)}: interval not supported: {s}");
            }
        }

        /// <summary>
        /// Try to transform a symbol string to an acceptable format.
        /// </summary>
        /// <param name="symbol"></param>
        /// <returns></returns>
        internal static string FixSymbol(this string symbol)
        {
            Throw.IfNullOrWhiteSpace(symbol);

            return symbol.Trim().Replace(" ", string.Empty).Replace("-", string.Empty).Replace("_", string.Empty).Replace("/", string.Empty).ToUpper();
        }
    }
}
