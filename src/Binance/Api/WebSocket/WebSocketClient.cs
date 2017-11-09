using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Binance.Api.WebSocket
{
    public sealed class WebSocketClient : IWebSocketClient
    {
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
        }

        #endregion Constructors

        #region Public Methods

        public async Task OpenAsync(Uri uri, Action<string> onMessage, CancellationToken token)
        {
            _webSocket = new ClientWebSocket();
            _webSocket.Options.KeepAliveInterval = TimeSpan.FromSeconds(30);

            await _webSocket
                .ConnectAsync(uri, token)
                .ConfigureAwait(false);

            if (_webSocket.State != WebSocketState.Open)
            {
                var e = new Exception($"{nameof(WebSocketClient)}.{nameof(OpenAsync)}: WebSocket connect failed (state: {_webSocket.State}).");
                LogException(e, $"{nameof(WebSocketClient)}.{nameof(OpenAsync)}");

                if (!token.IsCancellationRequested)
                    throw e;
            }

            try
            {
                var bytes = new byte[ReceiveBufferSize];
                var buffer = new ArraySegment<byte>(bytes);

                var stringBuilder = new StringBuilder();

                while (_webSocket.State == WebSocketState.Open)
                {
                    WebSocketReceiveResult result;
                    do
                    {
                        result = await _webSocket
                            .ReceiveAsync(buffer, token)
                            .ConfigureAwait(false);

                        switch (result.MessageType)
                        {
                            case WebSocketMessageType.Close:
                                if (result.CloseStatus.HasValue)
                                {
                                    _logger?.LogWarning($"{nameof(WebSocketClient)}.{nameof(OpenAsync)}: WebSocket closed ({result.CloseStatus.Value}): \"{result.CloseStatusDescription ?? "[no reason provided]"}\"");
                                }
                                await _webSocket
                                    .CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None)
                                    .ConfigureAwait(false);
                                break;

                            case WebSocketMessageType.Text when result.Count > 0:
                                stringBuilder.Append(Encoding.UTF8.GetString(bytes, 0, result.Count));
                                break;

                            case WebSocketMessageType.Binary:
                                _logger?.LogWarning($"{nameof(WebSocketClient)}.{nameof(OpenAsync)}: Received binary message type.");
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
                        onMessage(json);
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
                LogException(e, $"{nameof(WebSocketClient)}.{nameof(OpenAsync)}");

                if (!token.IsCancellationRequested)
                    throw;
            }
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Log an exception if not already logged within this library.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="source"></param>
        private void LogException(Exception e, string source)
        {
            if (e.IsLogged()) return;
            _logger?.LogError(e, $"{source}: \"{e.Message}\"");
            e.Logged();
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
