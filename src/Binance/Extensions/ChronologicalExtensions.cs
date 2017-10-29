using System;

// ReSharper disable once CheckNamespace
namespace Binance
{
    public static class ChronologicalExtensions
    {
        /// <summary>
        /// Convert timestamp to <see cref="DateTime"/> (UTC).
        /// </summary>
        /// <param name="chronological"></param>
        /// <returns></returns>
        public static DateTime Time(this IChronological chronological)
        {
            Throw.IfNull(chronological, nameof(chronological));

            return DateTimeOffset.FromUnixTimeMilliseconds(chronological.Timestamp).UtcDateTime;
        }
    }
}
