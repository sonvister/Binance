using Binance.Client;

namespace Binance.WebSocket
{
    public interface ITradeWebSocketClient : ITradeClient, IBinanceWebSocketClient
    { }
}
