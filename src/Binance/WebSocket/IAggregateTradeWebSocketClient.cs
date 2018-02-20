using Binance.Client;

namespace Binance.WebSocket
{
    /// <summary>
    /// An aggregate trade web socket client.
    /// </summary>
    public interface IAggregateTradeWebSocketClient : IBinanceWebSocketClient, IAggregateTradeClient
    { }
}
