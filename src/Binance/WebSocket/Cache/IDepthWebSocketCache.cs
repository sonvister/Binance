using Binance.Cache;
using Binance.Client;

// ReSharper disable once CheckNamespace
namespace Binance.WebSocket
{
    public interface IDepthWebSocketCache : IDepthWebSocketCache<IDepthWebSocketClient>
    { }

    public interface IDepthWebSocketCache<TClient> : IOrderBookCache<TClient>, IError
        where TClient : IDepthClient
    { }
}
