using Binance.Client;

namespace Binance.WebSocket
{
    public interface IBinanceWebSocketClient : IJsonStreamClient<IBinanceWebSocketStream>
    { }
}
