using Binance.Client;

namespace Binance.WebSocket
{
    /// <summary>
    /// A user data web socket client.
    /// </summary>
    public interface IUserDataWebSocketClient : IBinanceWebSocketClient, IUserDataClient
    { }
}
