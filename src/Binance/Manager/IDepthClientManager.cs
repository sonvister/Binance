using Binance.Client;
using Binance.Stream;

namespace Binance.Manager
{
    /// <summary>
    /// A depth client manager.
    /// </summary>
    public interface IDepthClientManager : IDepthClientManager<IJsonStream>, IControllerManager
    { }

    /// <summary>
    /// A depth client manager.
    /// </summary>
    public interface IDepthClientManager<out TStream> : IDepthClient, IControllerManager<TStream>
        where TStream : IJsonStream
    { }
}
