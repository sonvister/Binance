namespace Binance.Api.WebSocket
{
    public interface IBinanceWebSocketClient
    {
        /// <summary>
        /// The web socket stream.
        /// </summary>
        IWebSocketStream WebSocket { get; }
    }
}
