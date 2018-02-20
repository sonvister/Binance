using Binance.Client;
using Binance.Stream;

namespace Binance.Manager
{
    /// <summary>
    /// A symbol statistics client manager.
    /// </summary>
    public interface ISymbolStatisticsClientManager : ISymbolStatisticsClientManager <IJsonStream>, IControllerManager
    { }

    /// <summary>
    /// A symbol statistics client manager.
    /// </summary>
    public interface ISymbolStatisticsClientManager<out TStream> : ISymbolStatisticsClient, IControllerManager<TStream>
        where TStream : IJsonStream
    { }
}
