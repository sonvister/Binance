using Binance.Client;
using Binance.Stream;

namespace Binance.Manager
{
    /// <summary>
    /// An aggregate trade client manager.
    /// </summary>
    public interface IAggregateTradeClientManager : IAggregateTradeClientManager<IJsonStream>, IControllerManager
    { }

    /// <summary>
    /// An aggregate trade client manager.
    /// </summary>
    public interface IAggregateTradeClientManager<out TStream> : IAggregateTradeClient, IControllerManager<TStream>
        where TStream : IJsonStream
    { }
}
