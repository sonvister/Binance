using System;
using Binance.Market;

// ReSharper disable once CheckNamespace
namespace Binance
{
    internal static class CandlestickIntervalExtensions
    {
        /// <summary>
        /// Convert <see cref="CandlestickInterval"/> to string.
        /// </summary>
        /// <param name="interval"></param>
        /// <returns></returns>
        public static string AsString(this CandlestickInterval interval)
        {
            switch (interval)
            {
                case CandlestickInterval.Minute: return "1m";
                case CandlestickInterval.Minutes_3: return "3m";
                case CandlestickInterval.Minutes_5: return "5m";
                case CandlestickInterval.Minutes_15: return "15m";
                case CandlestickInterval.Minutes_30: return "30m";
                case CandlestickInterval.Hour: return "1h";
                case CandlestickInterval.Hours_2: return "2h";
                case CandlestickInterval.Hours_4: return "4h";
                case CandlestickInterval.Hours_6: return "6h";
                case CandlestickInterval.Hours_8: return "8h";
                case CandlestickInterval.Hours_12: return "12h";
                case CandlestickInterval.Day: return "1d";
                case CandlestickInterval.Days_3: return "3d";
                case CandlestickInterval.Week: return "1w";
                case CandlestickInterval.Month: return "1M";
                default:
                    throw new ArgumentException($"{nameof(CandlestickIntervalExtensions)}.{nameof(ToString)}: {nameof(CandlestickInterval)} not supported: {interval}");
            }
        }
    }
}
