using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Binance.Api.WebSocket.Events;
using Microsoft.Extensions.Logging;

namespace Binance.Api.WebSocket
{
    public sealed class WebSocketClient : IWebSocketClient
    {
        #region Public Events

        public event EventHandler<EventArgs> Open;

        public event EventHandler<WebSocketClientEventArgs> Message;

        public event EventHandler<EventArgs> Close;

        #endregion Public Events

        #region Public Properties

        public bool IsStreaming { get; private set; }

        #endregion Public Properties

        #region Private Constants

        private const int ReceiveBufferSize = 16 * 1024;

        #endregion Private Constants

        #region Private Fields

        private readonly ILogger<WebSocketClient> _logger;

        #endregion Private Fields

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger"></param>
        public WebSocketClient(ILogger<WebSocketClient> logger = null)
        {
            _logger = logger;
        }

        #endregion Constructors

        #region Public Methods

        public async Task StreamAsync(Uri uri, CancellationToken token)
        {
            Throw.IfNull(uri, nameof(uri));

            if (!token.CanBeCanceled)
                throw new ArgumentException("Token must be capable of being in the canceled state.", nameof(token));

            token.ThrowIfCancellationRequested();

            IsStreaming = true;

            var webSocket = new ClientWebSocket();
            webSocket.Options.KeepAliveInterval = TimeSpan.FromSeconds(30);

            try
            {
                try
                {
                    await webSocket.ConnectAsync(uri, token)
                        .ConfigureAwait(false);

                    if (webSocket.State == WebSocketState.Open)
                        RaiseOpenEvent();
                }
                catch (OperationCanceledException) { }
                catch (Exception e)
                {
                    if (!token.IsCancellationRequested)
                    {
                        _logger?.LogError(e, $"{nameof(WebSocketClient)}.{nameof(StreamAsync)}: WebSocket connect exception.");
                        throw;
                    }
                }

                var bytes = new byte[ReceiveBufferSize];
                var buffer = new ArraySegment<byte>(bytes);

                var stringBuilder = new StringBuilder();

                while (!token.IsCancellationRequested)
                {
                    stringBuilder.Clear();

                    try
                    {
                        WebSocketReceiveResult result;
                        do
                        {
                            if (webSocket.State != WebSocketState.Open)
                            {
                                throw new Exception($"{nameof(WebSocketClient)}.{nameof(StreamAsync)}: WebSocket is not open (state: {webSocket.State}).");
                            }

                            result = await webSocket
                                .ReceiveAsync(buffer, token)
                                .ConfigureAwait(false);

                            switch (result.MessageType)
                            {
                                case WebSocketMessageType.Close:
                                    throw new Exception(result.CloseStatus.HasValue
                                        ? $"{nameof(WebSocketClient)}.{nameof(StreamAsync)}: WebSocket closed ({result.CloseStatus.Value}): \"{result.CloseStatusDescription ?? "[no reason provided]"}\""
                                        : $"{nameof(WebSocketClient)}.{nameof(StreamAsync)}: WebSocket closed: \"{result.CloseStatusDescription ?? "[no reason provided]"}\"");

                                case WebSocketMessageType.Text when result.Count > 0:
                                    stringBuilder.Append(Encoding.UTF8.GetString(bytes, 0, result.Count));
                                    break;

                                case WebSocketMessageType.Binary:
                                    _logger?.LogWarning($"{nameof(WebSocketClient)}.{nameof(StreamAsync)}: Received unsupported binary message type.");
                                    break;

                                default:
                                    throw new ArgumentOutOfRangeException(nameof(result.MessageType), $"{nameof(WebSocketClient)}.{nameof(StreamAsync)}: Unknown result message type ({result.MessageType}).");
                            }
                        }
                        while (!result.EndOfMessage);
                    }
                    catch (OperationCanceledException) { }
                    catch (Exception e)
                    {
                        if (!token.IsCancellationRequested)
                        {
                            _logger?.LogError(e, $"{nameof(WebSocketClient)}.{nameof(StreamAsync)}: WebSocket receive exception.");
                            throw;
                        }
                    }

                    if (token.IsCancellationRequested)
                        continue;

                    var json = stringBuilder.ToString();
                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        RaiseMessageEvent(new WebSocketClientEventArgs(json));
                    }
                    else
                    {
                        _logger?.LogWarning($"{nameof(WebSocketClient)}.{nameof(StreamAsync)}: Received empty JSON message.");
                    }
                }
            }
            finally
            {
                IsStreaming = false;

                // NOTE: WebSocketState.CloseSent should not be encountered since CloseOutputAsync is not used.
                if (webSocket.State == WebSocketState.Open || webSocket.State == WebSocketState.CloseReceived)
                {
                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None)
                        .ConfigureAwait(false);
                }

                webSocket?.Dispose();

                RaiseCloseEvent();
            }
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Raise open event.
        /// </summary>
        private void RaiseOpenEvent()
        {
            try { Open?.Invoke(this, EventArgs.Empty); }
            catch (OperationCanceledException) { }
            catch (Exception e)
            {
                _logger?.LogError(e, $"{nameof(WebSocketClient)}: Unhandled open event handler exception.");
            }
        }

        /// <summary>
        /// Raise message event.
        /// </summary>
        /// <param name="args"></param>
        private void RaiseMessageEvent(WebSocketClientEventArgs args)
        {
            try { Message?.Invoke(this, args); }
            catch (OperationCanceledException) { }
            catch (Exception e)
            {
                _logger?.LogError(e, $"{nameof(WebSocketClient)}: Unhandled message event handler exception.");
            }
        }

        /// <summary>
        /// Raise close event.
        /// </summary>
        private void RaiseCloseEvent()
        {
            try { Close?.Invoke(this, EventArgs.Empty); }
            catch (OperationCanceledException) { }
            catch (Exception e)
            {
                _logger?.LogError(e, $"{nameof(WebSocketClient)}: Unhandled close event handler exception.");
            }
        }

        #endregion Private Methods
    }
}
