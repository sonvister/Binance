using Binance.Client;

namespace Binance.WebSocket
{
    public interface IAggregateTradeWebSocketClient : IAggregateTradeClient, IBinanceWebSocketClient
    { }
}
