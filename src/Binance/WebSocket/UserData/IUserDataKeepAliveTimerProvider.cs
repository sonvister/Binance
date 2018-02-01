namespace Binance.WebSocket.UserData
{
    public interface IUserDataKeepAliveTimerProvider
    {
        /// <summary>
        /// Create a new <see cref="IUserDataKeepAliveTimer"/>.
        /// </summary>
        /// <returns></returns>
        IUserDataKeepAliveTimer CreateTimer();
    }
}
