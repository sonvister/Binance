using Binance.Manager;

namespace Binance.WebSocket.Manager
{
    /// <summary>
    /// An aggregate trade web socket client manager.
    /// </summary>
    public interface IAggregateTradeWebSocketClientManager : IAggregateTradeClientManager<IWebSocketStream>, IWebSocketControllerManager
    { }
}
