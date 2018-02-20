using Binance.Client;

namespace Binance.WebSocket
{
    /// <summary>
    /// A symbol [24-hour] statistics web socket client.
    /// </summary>
    public interface ISymbolStatisticsWebSocketClient : IBinanceWebSocketClient, ISymbolStatisticsClient
    { }
}
