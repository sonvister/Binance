using Binance.Client;

namespace Binance.WebSocket
{
    public interface ICandlestickWebSocketClient : ICandlestickClient, IBinanceWebSocketClient
    { }
}
