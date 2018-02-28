using System;
using Binance.Cache;

namespace Binance.WebSocket.Manager
{
    public interface ITradeWebSocketCacheManager : ITradeCache, IWebSocketControllerManager
    {
        /// <summary>
        /// The controller error event.
        /// </summary>
        event EventHandler<ErrorEventArgs> Error;
    }
}
