using Binance.Client;
using Binance.Stream;

namespace Binance.Manager
{
    /// <summary>
    /// A candlestick client manager.
    /// </summary>
    public interface ICandlestickClientManager : ICandlestickClientManager<IJsonStream>, IControllerManager
    { }

    /// <summary>
    /// A candlestick client manager.
    /// </summary>
    public interface ICandlestickClientManager<out TStream> : ICandlestickClient, IControllerManager<TStream>
        where TStream : IJsonStream
    { }
}
