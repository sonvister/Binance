using Binance.Cache;
using Binance.Client;

// ReSharper disable once CheckNamespace
namespace Binance.WebSocket
{
    public interface ISymbolStatisticsWebSocketCache : ISymbolStatisticsWebSocketCache<ISymbolStatisticsWebSocketClient>
    { }

    public interface ISymbolStatisticsWebSocketCache<TClient> : ISymbolStatisticsCache<TClient>, IError
        where TClient : ISymbolStatisticsClient
    { }
}
