using Binance.Cache;
using Binance.Client;

// ReSharper disable once CheckNamespace
namespace Binance.WebSocket
{
    public interface ICandlestickWebSocketCache : ICandlestickWebSocketCache<ICandlestickWebSocketClient>
    { }

    public interface ICandlestickWebSocketCache<TClient> : ICandlestickCache<TClient>, IError
        where TClient : ICandlestickClient
    { }
}
