using Binance.Client;
using Binance.Stream;

namespace Binance.Manager
{
    /// <summary>
    /// A user data client manager.
    /// </summary>
    public interface IUserDataClientManager : IUserDataClientManager<IJsonStream>, IControllerManager
    { }

    /// <summary>
    /// A user data client manager.
    /// </summary>
    public interface IUserDataClientManager<out TStream> : IUserDataClient, IControllerManager<TStream>
        where TStream : IJsonStream
    { }
}
