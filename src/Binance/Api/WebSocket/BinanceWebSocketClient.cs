using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
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

            Logger?.LogInformation($"{nameof(BinanceWebSocketClient<TEventArgs>)}.{nameof(SubscribeToAsync)}: \"{BaseUri}{uriPath}\"");

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
                            LogException(e, $"{nameof(UserDataWebSocketClient)}");
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

                _client.Message += (s, e) => BufferBlock.Post(e.Message);

                await _client.OpenAsync(uri, token)
                    .ConfigureAwait(false);
            }
            catch (OperationCanceledException) { }
            catch (Exception e)
            {
                LogException(e, $"{nameof(BinanceWebSocketClient<TEventArgs>)}.{nameof(SubscribeToAsync)}");
                
                if (!token.IsCancellationRequested)
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
                    _client?.Dispose();

                    BufferBlock?.Complete();
                    ActionBlock?.Complete();
                }
                catch (OperationCanceledException) { }
                catch (Exception e)
                {
                    Logger?.LogError(e, $"{nameof(BinanceWebSocketClient<TEventArgs>)}.{nameof(Dispose)}: \"{e.Message}\"");
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
