// ReSharper disable once CheckNamespace
namespace Binance
{
    public static class ChronologicalExtensions
    {
        /// <summary>
        /// Convert <see cref="System.DateTime"/> (UTC) to Unix time milliseconds.
        /// </summary>
        /// <param name="chronological"></param>
        /// <returns></returns>
        public static long Timestamp(this IChronological chronological)
        {
            Throw.IfNull(chronological, nameof(chronological));

            return chronological.Time.ToTimestamp();
        }
    }
}
