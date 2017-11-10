using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Binance.Api.WebSocket.Events;
using Microsoft.Extensions.Logging;

namespace Binance.Api.WebSocket
{
    /// <summary>
    /// WebSocket client base class.
    /// </summary>
    public abstract class BinanceWebSocketClient<TEventArgs>
        where TEventArgs : EventArgs
    {
        #region Protected Fields

        protected BufferBlock<string> BufferBlock;
        protected ActionBlock<string> ActionBlock;

        protected bool IsSubscribed;

        protected ILogger Logger;

        #endregion Protected Fields

        #region Private Constants

        private const string BaseUri = "wss://stream.binance.com:9443/ws/";

        #endregion Private Constants

        #region Private Fields

        private IWebSocketClient _client;

        #endregion Private Fields

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="logger"></param>
        protected BinanceWebSocketClient(IWebSocketClient client, ILogger logger = null)
        {
            Throw.IfNull(client, nameof(client));

            _client = client;
            Logger = logger;
        }

        #endregion Constructors

        #region Protected Methods

        protected abstract void DeserializeJsonAndRaiseEvent(string json, CancellationToken token, Action<TEventArgs> callback = null);

        /// <summary>
        /// Subscribe.
        /// </summary>
        /// <param name="uriPath"></param>
        /// <param name="callback"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        protected async Task SubscribeToAsync(string uriPath, Action<TEventArgs> callback, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            Logger?.LogInformation($"{GetType().Name}.{nameof(SubscribeToAsync)}: \"{BaseUri}{uriPath}\"");

            IsSubscribed = true;

            try
            {
                BufferBlock = new BufferBlock<string>(new DataflowBlockOptions
                {
                    EnsureOrdered = true,
                    CancellationToken = token,
                    BoundedCapacity = DataflowBlockOptions.Unbounded,
                    MaxMessagesPerTask = DataflowBlockOptions.Unbounded
                });

                ActionBlock = new ActionBlock<string>(json =>
                    {
                        try { DeserializeJsonAndRaiseEvent(json, token, callback); }
                        catch (OperationCanceledException) { }
                        catch (Exception e)
                        {
                            if (!token.IsCancellationRequested)
                            {
                                Logger?.LogError(e, $"{GetType().Name}: Unhandled {nameof(DeserializeJsonAndRaiseEvent)} exception.");
                            }
                        }
                    },
                    new ExecutionDataflowBlockOptions
                    {
                        BoundedCapacity = 1,
                        EnsureOrdered = true,
                        MaxDegreeOfParallelism = 1,
                        CancellationToken = token,
                        SingleProducerConstrained = true
                    });

                BufferBlock.LinkTo(ActionBlock);

                var uri = new Uri($"{BaseUri}{uriPath}");

                _client.Message += OnClientMessage;

                await _client.OpenAsync(uri, token)
                    .ConfigureAwait(false);
            }
            catch (OperationCanceledException) { }
            catch (Exception e)
            {
                if (!token.IsCancellationRequested)
                {
                    Logger?.LogError(e, $"{GetType().Name}.{nameof(SubscribeToAsync)}: Exception.");
                    throw;
                }
            }
            finally
            {
                _client.Message -= OnClientMessage;

                BufferBlock?.Complete();
                ActionBlock?.Complete();
            }
        }

        #endregion Protected Methods

        #region Private Methods

        private void OnClientMessage(object sender, WebSocketClientMessageEventArgs e)
        {
            BufferBlock.Post(e.Message);
        }

        #endregion Private Methods

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
                    _client?.Dispose();
                }
                catch (OperationCanceledException) { }
                catch (Exception e)
                {
                    Logger?.LogError(e, $"{GetType().Name}.{nameof(Dispose)}");
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
