using Microsoft.Extensions.Logging;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Binance.Api.WebSocket
{
    /// <summary>
    /// WebSocket client base class.
    /// </summary>
    public abstract class BinanceWebSocketClient
    {
        #region Protected Fields

        protected ClientWebSocket WebSocket;

        protected BufferBlock<string> BufferBlock;
        protected ActionBlock<string> ActionBlock;

        protected bool IsSubscribed;

        protected ILogger Logger;

        #endregion Protected Fields

        #region Private Constants

        private const string BaseUri = "wss://stream.binance.com:9443/ws/";

        private const int ReceiveBufferSize = 16 * 1024;

        #endregion Private Constants

        #region Constructors
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger"></param>
        protected BinanceWebSocketClient(ILogger logger = null)
        {
            Logger = logger;
        }

        #endregion Constructors

        #region Protected Methods

        /// <summary>
        /// 
        /// </summary>
        /// <param name="uriPath"></param>
        /// <param name="action"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        protected async Task SubscribeAsync(string uriPath, Action<string> action, CancellationToken token = default)
        {
            try
            {
                BufferBlock = new BufferBlock<string>(new DataflowBlockOptions()
                {
                    EnsureOrdered = true,
                    CancellationToken = token,
                    BoundedCapacity = DataflowBlockOptions.Unbounded,
                    MaxMessagesPerTask = DataflowBlockOptions.Unbounded,
                });

                ActionBlock = new ActionBlock<string>(action,
                    new ExecutionDataflowBlockOptions()
                    {
                        BoundedCapacity = 1,
                        EnsureOrdered = true,
                        MaxDegreeOfParallelism = 1,
                        CancellationToken = token,
                        SingleProducerConstrained = true,
                    });

                BufferBlock.LinkTo(ActionBlock);

                WebSocket = new ClientWebSocket();
                WebSocket.Options.KeepAliveInterval = TimeSpan.FromSeconds(30);

                var uri = new Uri($"{BaseUri}{uriPath}");

                await WebSocket
                    .ConnectAsync(uri, token)
                    .ConfigureAwait(false);

                if (WebSocket.State != WebSocketState.Open)
                {
                    Logger?.LogError($"{nameof(DepthWebSocketClient)}.{nameof(SubscribeAsync)}: WebSocket connect failed (State: {WebSocket.State}).");
                    WebSocket.Dispose();
                    return;
                }

                IsSubscribed = true;

                await ReceiveEventData(WebSocket, token)
                    .ConfigureAwait(false);
            }
            catch (OperationCanceledException) { }
            catch (Exception e)
            {
                LogException(e, $"{nameof(DepthWebSocketClient)}.{nameof(SubscribeAsync)}");
                throw;
            }
        }

        /// <summary>
        /// Receive event data from web socket.
        /// </summary>
        /// <param name="webSocket"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        protected virtual async Task ReceiveEventData(ClientWebSocket webSocket, CancellationToken token = default)
        {
            Throw.IfNull(webSocket, nameof(webSocket));
            Throw.IfNull(token, nameof(token));

            try
            {
                var bytes = new byte[ReceiveBufferSize];
                var buffer = new ArraySegment<byte>(bytes);

                var stringBuilder = new StringBuilder();

                while (webSocket.State == WebSocketState.Open)
                {
                    WebSocketReceiveResult result;
                    do
                    {
                        result = await webSocket
                            .ReceiveAsync(buffer, token)
                            .ConfigureAwait(false);

                        switch (result.MessageType)
                        {
                            case WebSocketMessageType.Close:
                                if (result.CloseStatus.HasValue)
                                {
                                    Logger?.LogWarning($"{nameof(BinanceWebSocketClient)}.{nameof(ReceiveEventData)}: WebSocket closed ({result.CloseStatus.Value}): \"{result.CloseStatusDescription ?? "[no reason provided]"}\"");
                                }
                                await webSocket
                                    .CloseAsync(WebSocketCloseStatus.NormalClosure, string.Empty, CancellationToken.None)
                                    .ConfigureAwait(false);
                                break;

                            case WebSocketMessageType.Text when result.Count > 0:
                                stringBuilder.Append(Encoding.UTF8.GetString(bytes, 0, result.Count));
                                break;

                            case WebSocketMessageType.Binary:
                                Logger?.LogWarning($"{nameof(BinanceWebSocketClient)}.{nameof(ReceiveEventData)}: Received binary message type.");
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
                        BufferBlock.Post(json);
                    }
                    else
                    {
                        Logger?.LogWarning($"{nameof(BinanceWebSocketClient)}.{nameof(ReceiveEventData)}: Received empty JSON message.");
                    }
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception e)
            {
                LogException(e, $"{nameof(BinanceWebSocketClient)}.{nameof(ReceiveEventData)}");
                throw;
            }
        }

        /// <summary>
        /// Log an exception if not already logged within this library.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="source"></param>
        protected void LogException(Exception e, string source)
        {
            if (e.IsLogged()) return;
            Logger?.LogError(e, $"{source}: \"{e.Message}\"");
            e.Logged();
        }

        #endregion Protected Methods

        #region IDisposable

        private bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                try
                {
                    WebSocket?.Dispose();

                    BufferBlock?.Complete();
                    ActionBlock?.Complete();
                }
                catch (OperationCanceledException) { }
                catch (Exception e)
                {
                    Logger?.LogError(e, $"{nameof(BinanceWebSocketClient)}.{nameof(Dispose)}: \"{e.Message}\"");
                }
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
