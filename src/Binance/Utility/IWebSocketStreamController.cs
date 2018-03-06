using Binance.WebSocket;

namespace Binance.Utility
{
    public interface IWebSocketStreamController : IWebSocketStreamController<IWebSocketStream>
    { }

    /// <summary>
    /// A web socket JSON stream controller.
    /// </summary>
    public interface IWebSocketStreamController<out TStream> : IJsonStreamController<TStream>
        where TStream : IWebSocketStream
    { }
}
