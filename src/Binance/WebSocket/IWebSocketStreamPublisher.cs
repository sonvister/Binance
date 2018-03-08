using Binance.Stream;

namespace Binance.WebSocket
{
    public interface IWebSocketStreamPublisher : IAutoJsonStreamPublisher<IWebSocketStream>
    { }
}
