using Binance.Client;

namespace Binance.WebSocket
{
    /// <summary>
    /// A depth web socket client.
    /// </summary>
    public interface IDepthWebSocketClient : IBinanceWebSocketClient, IDepthClient
    { }
}
