using System;
using Binance.Cache;

namespace Binance.WebSocket.Manager
{
    public interface ISymbolStatisticsWebSocketCacheManager : ISymbolStatisticsCache, IWebSocketControllerManager
    {
        /// <summary>
        /// The controller error event.
        /// </summary>
        event EventHandler<ErrorEventArgs> Error;
    }
}
