using Binance.Manager;

namespace Binance.WebSocket.Manager
{
    /// <summary>
    /// A depth web socket client manager.
    /// </summary>
    public interface ISymbolStatisticsWebSocketClientManager : ISymbolStatisticsClientManager<IWebSocketStream>, ISymbolStatisticsClientManager
    { }
}
