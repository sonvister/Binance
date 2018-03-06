namespace Binance.WebSocket
{
    public interface IClientWebSocketFactory
    {
        /// <summary>
        /// Create a new <see cref="IClientWebSocket"/>.
        /// </summary>
        /// <returns></returns>
        IClientWebSocket CreateClientWebSocket();
    }
}
