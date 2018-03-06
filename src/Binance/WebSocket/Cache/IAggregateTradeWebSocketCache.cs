using Binance.Cache;
using Binance.Client;

// ReSharper disable once CheckNamespace
namespace Binance.WebSocket
{
    public interface IAggregateTradeWebSocketCache : IAggregateTradeWebSocketCache<IAggregateTradeWebSocketClient>
    { }

    public interface IAggregateTradeWebSocketCache<TClient> : IAggregateTradeCache<TClient>, IError
        where TClient : IAggregateTradeClient
    { }
}
