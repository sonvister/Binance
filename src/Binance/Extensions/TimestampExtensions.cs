using System;

// ReSharper disable once CheckNamespace
namespace Binance
{
    internal static class TimestampExtensions
    {
        /// <summary>
        /// Convert a timestamp to UTC <see cref="DateTime"/>.
        /// </summary>
        /// <param name="timestamp"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this long timestamp)
        {
            return DateTimeOffset.FromUnixTimeMilliseconds(timestamp).UtcDateTime;
        }
    }
}
