using Binance.Cache;
using Binance.Client;

// ReSharper disable once CheckNamespace
namespace Binance.WebSocket
{
    public interface ITradeWebSocketCache : ITradeWebSocketCache<ITradeWebSocketClient>
    { }

    public interface ITradeWebSocketCache<TClient> : ITradeCache<TClient>
        where TClient : ITradeClient
    { }
}
