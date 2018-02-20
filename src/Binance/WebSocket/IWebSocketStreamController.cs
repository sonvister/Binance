using Binance.Manager;

namespace Binance.WebSocket
{
    /// <summary>
    /// A web socket <see cref="IJsonStreamController"/>.
    /// </summary>
    public interface IWebSocketStreamController : IJsonStreamController<IWebSocketStream>
    { }
}
