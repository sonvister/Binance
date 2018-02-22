using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Binance.Api;
using Binance.Client;
using Binance.Client.Events;
using Binance.Manager;
using Microsoft.Extensions.Logging;

namespace Binance.WebSocket.Manager
{
    /// <summary>
    /// The default <see cref="IUserDataWebSocketManager"/> implementation.
    /// </summary>
    public class UserDataWebSocketClientManager : UserDataClientManager<IWebSocketStream>, IUserDataWebSocketManager
    {
        #region Private Fields

        private readonly IUserDataWebSocketStreamControl _streamControl;

        #endregion Private Fields

        #region Constructors

        /// <summary>
        /// Default constructor provides default <see cref="IUserDataClient"/>,
        /// default <see cref="IUserDataWebSocketStreamControl"/> and default
        /// <see cref="IWebSocketStream"/>, but no logger.
        /// </summary>
        public UserDataWebSocketClientManager()
            : this(new UserDataClient(), new WebSocketStreamController(new BinanceWebSocketStream()), new UserDataWebSocketStreamControl())
        { }

        /// <summary>
        /// The DI constructor.
        /// </summary>
        /// <param name="client">The JSON client (required).</param>
        /// <param name="controller">The web socket stream controller (required).</param>
        /// <param name="streamControl">The web socket stream control (required).</param>
        /// <param name="logger">The logger (optional).</param>
        public UserDataWebSocketClientManager(IUserDataClient client, IWebSocketStreamController controller, IUserDataWebSocketStreamControl streamControl, ILogger<UserDataWebSocketClientManager> logger = null)
            : base(client, controller, logger)
        {
            Throw.IfNull(streamControl, nameof(streamControl));

            _streamControl = streamControl;

            _streamControl.ListenKeyUpdate += (s, a) =>
            {
                try
                {
                    if (a.NewListenKey == null)
                    {
                        Logger?.LogError($"{nameof(UserDataWebSocketClientManager)}: Failed to get new listen key.");
                        return;
                    }

                    // Unsubscribe client from stream.
                    Controller.Stream.Unsubscribe(Client, a.OldListenKey);

                    // Update client listen key.
                    HandleListenKeyChange(a.OldListenKey, a.NewListenKey);

                    // Subscribe client to new stream.
                    Controller.Stream.Subscribe(Client, a.NewListenKey);
                }
                catch (Exception e)
                {
                    Logger?.LogError(e, $"{nameof(UserDataWebSocketClientManager)}: Failed to update listen key.");
                }
            };
        }

        #endregion Construtors

        #region Public Methods

        public async Task SubscribeAsync<TEventArgs>(IBinanceApiUser user, Action<TEventArgs> callback, CancellationToken token = default)
            where TEventArgs : UserDataEventArgs
        {
            var listenKey = await _streamControl.OpenStreamAsync(user, token)
                .ConfigureAwait(false);

            HandleSubscribe(() => Client.Subscribe(listenKey, user, callback));
        }

        public async Task UnsubscribeAsync<TEventArgs>(IBinanceApiUser user, Action<TEventArgs> callback, CancellationToken token = default)
            where TEventArgs : UserDataEventArgs
        {
            var listenKey = await _streamControl.GetStreamNameAsync(user, token)
                .ConfigureAwait(false);

            HandleUnsubscribe(() => Client.Unsubscribe(listenKey, callback));

            if (callback == null || !Controller.Stream.ProvidedStreams.Contains(listenKey))
            {
                await _streamControl.CloseStreamAsync(user, token)
                    .ConfigureAwait(false);
            }
        }

        public async Task UnsubscribeAllAsync(CancellationToken token = default)
        {
            foreach (var user in _streamControl.Users)
            {
                await this.UnsubscribeAsync(user, token)
                    .ConfigureAwait(false);
            }
        }

        #endregion Public Methods

        #region IDisposble

        private bool _disposed;

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _streamControl?.Dispose();
            }

            _disposed = true;

            base.Dispose(disposing);
        }

        #endregion IDisposable
    }
}
