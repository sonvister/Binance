using System;
using Binance.Manager;

namespace Binance.WebSocket.Manager
{
    /// <summary>
    /// A trade web socket client manager.
    /// </summary>
    public interface ITradeWebSocketClientManager : ITradeClientManager<IWebSocketStream>, IWebSocketControllerManager
    {
        /// <summary>
        /// The controller error event.
        /// </summary>
        event EventHandler<ErrorEventArgs> Error;
    }
}
