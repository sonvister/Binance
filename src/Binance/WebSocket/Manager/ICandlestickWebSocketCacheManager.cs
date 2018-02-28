using System;
using Binance.Cache;

namespace Binance.WebSocket.Manager
{
    public interface ICandlestickWebSocketCacheManager : ICandlestickCache, IWebSocketControllerManager
    {
        /// <summary>
        /// The controller error event.
        /// </summary>
        event EventHandler<ErrorEventArgs> Error;
    }
}
