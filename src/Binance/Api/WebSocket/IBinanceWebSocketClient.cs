namespace Binance.Api.WebSocket
{
    public interface IBinanceWebSocketClient
    {
        /// <summary>
        /// The web socket client.
        /// </summary>
        IWebSocketClient WebSocket { get; }
    }
}
