using Binance.Producer;

namespace Binance.WebSocket
{
    public interface IWebSocketStreamPublisher : IAutoJsonStreamPublisher<IWebSocketStream>
    { }
}
