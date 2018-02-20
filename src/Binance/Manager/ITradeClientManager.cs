using Binance.Client;
using Binance.Stream;

namespace Binance.Manager
{
    /// <summary>
    /// A trade client manager.
    /// </summary>
    public interface ITradeClientManager : ITradeClientManager<IJsonStream>, IControllerManager
    { }

    /// <summary>
    /// A trade client manager.
    /// </summary>
    public interface ITradeClientManager<out TStream> : ITradeClient, IControllerManager<TStream>
        where TStream : IJsonStream
    { }
}
