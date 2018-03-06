using Binance.Producer;

namespace Binance.WebSocket
{
    public interface IBinanceWebSocketStreamPublisher : IAutoJsonStreamPublisher<IBinanceWebSocketStream>
    { }
}
