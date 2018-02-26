using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Binance.WebSocket
{
    /// <summary>
    /// The abstract <see cref="IWebSocketClient"/> implementation base class.
    /// </summary>
    public abstract class WebSocketClient : JsonProvider, IWebSocketClient
    {
        #region Public Events

        public event EventHandler<EventArgs> Open;

        public event EventHandler<EventArgs> Close;

        #endregion Public Events

        #region Public Properties

        public bool IsStreaming { get; protected set; }

        public bool IsOpen { get; protected set; }

        #endregion Public Properties

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger"></param>
        protected WebSocketClient(ILogger<WebSocketClient> logger = null)
            : base(logger)
        { }

        #endregion Constructors

        #region Public Methods

        public abstract Task StreamAsync(Uri uri, CancellationToken token);

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Raise open event.
        /// </summary>
        protected void OnOpen()
        {
            IsOpen = true;

            try { Open?.Invoke(this, EventArgs.Empty); }
            catch (Exception e)
            {
                Logger?.LogError(e, $"{GetType().Name}: Unhandled {nameof(Open)} event handler exception.");
            }
        }

        /// <summary>
        /// Raise close event.
        /// </summary>
        protected void OnClose()
        {
            IsOpen = false;

            try { Close?.Invoke(this, EventArgs.Empty); }
            catch (Exception e)
            {
                Logger?.LogError(e, $"{GetType().Name}: Unhandled {nameof(Close)} event handler exception.");
            }
        }

        #endregion Protected Methods
    }
}
