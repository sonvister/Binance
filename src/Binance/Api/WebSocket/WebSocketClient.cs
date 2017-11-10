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

        public event EventHandler<WebSocketClientMessageEventArgs> Message;

        #endregion Public Events

        #region Private Constants

        private const int ReceiveBufferSize = 16 * 1024;

        #endregion Private Constants

        #region Private Fields

        private ClientWebSocket _webSocket;

        private ILogger<WebSocketClient> _logger;

        #endregion Private Fields

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger"></param>
        public WebSocketClient(ILogger<WebSocketClient> logger = null)
        {
            _logger = logger;

            _webSocket = new ClientWebSocket();
            _webSocket.Options.KeepAliveInterval = TimeSpan.FromSeconds(30);
        }

        #endregion Constructors

        #region Public Methods

        public async Task OpenAsync(Uri uri, CancellationToken token)
        {
            Throw.IfNull(uri, nameof(uri));
            Throw.IfNull(token, nameof(token));

            token.ThrowIfCancellationRequested();

            try
            {
                await _webSocket
                    .ConnectAsync(uri, token)
                    .ConfigureAwait(false);

                var bytes = new byte[ReceiveBufferSize];
                var buffer = new ArraySegment<byte>(bytes);

                var stringBuilder = new StringBuilder();

                while (!token.IsCancellationRequested)
                {
                    WebSocketReceiveResult result;
                    do
                    {
                        if (_webSocket.State != WebSocketState.Open)
                            throw new Exception($"{nameof(WebSocketClient)}.{nameof(OpenAsync)}: WebSocket is not open (state: {_webSocket.State}).");

                        result = await _webSocket
                            .ReceiveAsync(buffer, token)
                            .ConfigureAwait(false);

                        switch (result.MessageType)
                        {
                            case WebSocketMessageType.Close:
                                throw new Exception(result.CloseStatus.HasValue
                                    ? $"{nameof(WebSocketClient)}.{nameof(OpenAsync)}: WebSocket closed ({result.CloseStatus.Value}): \"{result.CloseStatusDescription ?? "[no reason provided]"}\""
                                    : $"{nameof(WebSocketClient)}.{nameof(OpenAsync)}: WebSocket closed.");

                            case WebSocketMessageType.Text when result.Count > 0:
                                stringBuilder.Append(Encoding.UTF8.GetString(bytes, 0, result.Count));
                                break;

                            case WebSocketMessageType.Binary:
                                _logger?.LogWarning($"{nameof(WebSocketClient)}.{nameof(OpenAsync)}: Received unsupported binary message type.");
                                break;

                            default:
                                throw new ArgumentOutOfRangeException(nameof(result.MessageType), "Unknown result message type.");
                        }
                    }
                    while (!result.EndOfMessage);

                    var json = stringBuilder.ToString();
                    stringBuilder.Clear();

                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        RaiseMessageEvent(new WebSocketClientMessageEventArgs(json));
                    }
                    else
                    {
                        _logger?.LogWarning($"{nameof(WebSocketClient)}.{nameof(OpenAsync)}: Received empty JSON message.");
                    }
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception e)
            {
                if (!token.IsCancellationRequested)
                {
                    _logger?.LogError(e, $"{nameof(WebSocketClient)}.{nameof(OpenAsync)}: \"{e.Message}\"");
                    throw;
                }
            }
            finally
            {
                await _webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None)
                    .ConfigureAwait(false);
            }
        }

        public Task CloseAsync(CancellationToken token = default)
        {
            return _webSocket != null
                ? _webSocket.CloseOutputAsync(WebSocketCloseStatus.NormalClosure, string.Empty, token)
                : Task.CompletedTask;
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Raise message event.
        /// </summary>
        /// <param name="args"></param>
        private void RaiseMessageEvent(WebSocketClientMessageEventArgs args)
        {
            try { Message?.Invoke(this, args); }
            catch (OperationCanceledException) { }
            catch (Exception e)
            {
                _logger?.LogError(e, $"{nameof(WebSocketClient)}: Unhandled message event handler exception.");
            }
        }

        #endregion Private Methods

        #region IDisposable

        private bool _disposed = false;

        void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _webSocket?.Dispose();
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion IDisposable
    }
}
