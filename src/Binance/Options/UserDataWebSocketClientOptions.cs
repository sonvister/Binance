using Binance.Api.WebSocket;

namespace Binance.Options
{
    public sealed class UserDataWebSocketClientOptions
    {
        /// <summary>
        /// Keep-alive timer period.
        /// </summary>
        public int KeepAliveTimerPeriod { get; set; } = UserDataWebSocketClient.KeepAliveTimerPeriodDefault;
    }
}
