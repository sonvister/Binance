using System;

// ReSharper disable once CheckNamespace
namespace Binance.WebSocket.UserData
{
    public static class UserDataKeepAliveTimerProviderExtensions
    {
        /// <summary>
        /// Create <see cref="IUserDataKeepAliveTimer"/> with specified period.
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="periodMilliseconds"></param>
        /// <returns></returns>
        public static IUserDataKeepAliveTimer CreateTimer(this IUserDataKeepAliveTimerProvider provider, int periodMilliseconds)
            => CreateTimer(provider, TimeSpan.FromMilliseconds(periodMilliseconds));

        /// <summary>
        /// Create <see cref="IUserDataKeepAliveTimer"/> with specified period.
        /// </summary>
        /// <param name="provider"></param>
        /// <param name="period"></param>
        /// <returns></returns>
        public static IUserDataKeepAliveTimer CreateTimer(this IUserDataKeepAliveTimerProvider provider, TimeSpan period)
        {
            Throw.IfNull(provider, nameof(provider));

            var timer = provider.CreateTimer();
            timer.Period = period;
            return timer;
        }
    }
}
