using System;
using System.Threading;
using System.Threading.Tasks;
using Binance.WebSocket.Events;
using Microsoft.Extensions.Logging;

namespace Binance.WebSocket
{
    public abstract class WebSocketClient : IWebSocketClient
    {
        #region Public Events

        public event EventHandler<EventArgs> Open;

        public event EventHandler<WebSocketClientEventArgs> Message;

        public event EventHandler<EventArgs> Close;

        #endregion Public Events

        #region Public Properties

        public bool IsStreaming { get; protected set; }

        #endregion Public Properties

        #region Protected Fields

        protected readonly ILogger<WebSocketClient> Logger;

        #endregion Protected Fields

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger"></param>
        protected WebSocketClient(ILogger<WebSocketClient> logger = null)
        {
            Logger = logger;
        }

        #endregion Constructors

        #region Public Methods

        public abstract Task StreamAsync(Uri uri, CancellationToken token);

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Raise open event.
        /// </summary>
        protected void RaiseOpenEvent()
        {
            try { Open?.Invoke(this, EventArgs.Empty); }
            catch (OperationCanceledException) { }
            catch (Exception e)
            {
                Logger?.LogError(e, $"{GetType().Name}: Unhandled open event handler exception.");
            }
        }

        /// <summary>
        /// Raise message event.
        /// </summary>
        /// <param name="args"></param>
        protected void RaiseMessageEvent(WebSocketClientEventArgs args)
        {
            try { Message?.Invoke(this, args); }
            catch (OperationCanceledException) { }
            catch (Exception e)
            {
                Logger?.LogError(e, $"{GetType().Name}: Unhandled message event handler exception.");
            }
        }

        /// <summary>
        /// Raise close event.
        /// </summary>
        protected void RaiseCloseEvent()
        {
            try { Close?.Invoke(this, EventArgs.Empty); }
            catch (OperationCanceledException) { }
            catch (Exception e)
            {
                Logger?.LogError(e, $"{GetType().Name}: Unhandled close event handler exception.");
            }
        }

        #endregion Protected Methods
    }
}
