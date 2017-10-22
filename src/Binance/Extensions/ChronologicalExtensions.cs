using System;

namespace Binance
{
    public static class ChronologicalExtensions
    {
        /// <summary>
        /// Convert timestamp to <see cref="DateTime"/> (UTC).
        /// </summary>
        /// <param name="">The trade.</param>
        /// <returns></returns>
        public static DateTime Time(this IChronological chronological)
        {
            Throw.IfNull(chronological, nameof(chronological));

            return DateTimeOffset.FromUnixTimeMilliseconds(chronological.Timestamp).UtcDateTime;
        }
    }
}
