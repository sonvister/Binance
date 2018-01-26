using System;

// ReSharper disable once CheckNamespace
namespace Binance
{
    internal static class TimestampExtensions
    {
        /// <summary>
        /// Convert Unix time milliseconds to <see cref="DateTime"/> (UTC).
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this long timestamp)
        {
            return DateTimeOffset.FromUnixTimeMilliseconds(timestamp).UtcDateTime;
        }
    }
}
