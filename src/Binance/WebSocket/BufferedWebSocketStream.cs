using Binance.Stream;
using Microsoft.Extensions.Logging;

namespace Binance.WebSocket
{
    /// <summary>
    /// An abstract buffered <see cref="IWebSocketStream"/> base class.
    /// </summary>
    public abstract class BufferedWebSocketStream : BufferedJsonStream<IWebSocketClient>, IWebSocketStream
    {
        #region Public Properties

        public IWebSocketClient WebSocket => JsonProvider;

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="webSocket"></param>
        /// <param name="logger"></param>
        protected BufferedWebSocketStream(IWebSocketClient webSocket, ILogger<BufferedWebSocketStream> logger = null)
            : base(webSocket, logger)
        { }

        #endregion Constructors
    }
}
