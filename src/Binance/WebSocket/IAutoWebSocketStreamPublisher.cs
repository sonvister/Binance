using Binance.Producer;

namespace Binance.WebSocket
{
    public interface IAutoWebSocketStreamPublisher : IAutoJsonStreamPublisher<IWebSocketStream>
    { }
}
