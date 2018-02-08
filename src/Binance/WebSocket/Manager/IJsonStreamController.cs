using Binance.Utility;

namespace Binance.WebSocket.Manager
{
    public interface IJsonStreamController : ITaskController
    {
        /// <summary>
        /// Get the JSON stream.
        /// </summary>
        IJsonStream JsonStream { get; }
    }
}
