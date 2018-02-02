using Binance.WebSocket.UserData;

// ReSharper disable once CheckNamespace
namespace Binance
{
    public sealed class UserDataWebSocketManagerOptions
    {
        /// <summary>
        /// Keep-alive timer period.
        /// </summary>
        public int KeepAliveTimerPeriod { get; set; } = UserDataKeepAliveTimer.PeriodDefault;
    }
}
