using Binance.Market;
using System;

// ReSharper disable once CheckNamespace
namespace Binance
{
    internal static class KlineIntervalExtensions
    {
        /// <summary>
        /// Convert <see cref="KlineInterval"/> to string.
        /// </summary>
        /// <param name="interval"></param>
        /// <returns></returns>
        public static string AsString(this KlineInterval interval)
        {
            switch (interval)
            {
                case KlineInterval.Minute: return "1m";
                case KlineInterval.Minutes_3: return "3m";
                case KlineInterval.Minutes_5: return "5m";
                case KlineInterval.Minutes_15: return "15m";
                case KlineInterval.Minutes_30: return "30m";
                case KlineInterval.Hour: return "1h";
                case KlineInterval.Hours_2: return "2h";
                case KlineInterval.Hours_4: return "4h";
                case KlineInterval.Hours_6: return "6h";
                case KlineInterval.Hours_8: return "8h";
                case KlineInterval.Hours_12: return "12h";
                case KlineInterval.Day: return "1d";
                case KlineInterval.Days_3: return "3d";
                case KlineInterval.Week: return "1w";
                case KlineInterval.Month: return "1M";
                default:
                    throw new ArgumentException($"{nameof(KlineIntervalExtensions)}.{nameof(ToString)}: {nameof(KlineInterval)} not supported: {interval}");
            }
        }
    }
}
