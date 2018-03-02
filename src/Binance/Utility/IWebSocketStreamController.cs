using Binance.WebSocket;

namespace Binance.Utility
{
    /// <summary>
    /// A web socket <see cref="IJsonStreamController"/>.
    /// </summary>
    public interface IWebSocketStreamController : IJsonStreamController<IWebSocketStream>
    { }
}
