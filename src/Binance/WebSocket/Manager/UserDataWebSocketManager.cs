using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Binance.Client;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace Binance.WebSocket
{
    /// <summary>
    /// The default <see cref="IUserDataWebSocketManager"/> implementation.
    /// </summary>
    public class UserDataWebSocketManager : IUserDataWebSocketManager
    {
        #region Public Events

        public event EventHandler<AccountUpdateEventArgs> AccountUpdate
        {
            add => Client.AccountUpdate += value;
            remove => Client.AccountUpdate -= value;
        }

        public event EventHandler<OrderUpdateEventArgs> OrderUpdate
        {
            add => Client.OrderUpdate += value;
            remove => Client.OrderUpdate -= value;
        }

        public event EventHandler<AccountTradeUpdateEventArgs> TradeUpdate
        {
            add => Client.TradeUpdate += value;
            remove => Client.TradeUpdate -= value;
        }

        #endregion Public Events

        #region Public Properties

        public IUserDataWebSocketClient Client { get; }

        #endregion Public Properties

        #region Private Fields

        private readonly IUserDataWebSocketStreamControl _streamControl;

        // ReSharper disable once PrivateFieldCanBeConvertedToLocalVariable
        private readonly ILogger<UserDataWebSocketManager> _logger;

        #endregion Private Fields

        #region Constructors

        /// <summary>
        /// Default constructor provides default <see cref="IUserDataWebSocketClient"/>
        /// and default <see cref="IUserDataWebSocketStreamControl"/>, but no logger.
        /// </summary>
        public UserDataWebSocketManager()
            : this(new UserDataWebSocketClient(), new UserDataWebSocketStreamControl())
        { }

        /// <summary>
        /// The DI constructor.
        /// </summary>
        /// <param name="client">The user data web socket client (required).</param>
        /// <param name="streamControl">The web socket stream control (required).</param>
        /// <param name="logger">The logger (optional).</param>
        public UserDataWebSocketManager(IUserDataWebSocketClient client, IUserDataWebSocketStreamControl streamControl, ILogger<UserDataWebSocketManager> logger = null)
        {
            Throw.IfNull(client, nameof(client));
            Throw.IfNull(streamControl, nameof(streamControl));

            Client = client;
            _streamControl = streamControl;
            _logger = logger;

            _streamControl.ListenKeyUpdate += (s, a) =>
            {
                try
                {
                    if (a.NewListenKey == null)
                    {
                        _logger?.LogError($"{nameof(UserDataWebSocketManager)}: Failed to get new listen key.");
                        return;
                    }

                    // Update client listen key (need this to preserve callbacks).
                    Client.HandleListenKeyChange(a.OldListenKey, a.NewListenKey);
                }
                catch (Exception e)
                {
                    _logger?.LogError(e, $"{nameof(UserDataWebSocketManager)}: Failed to update listen key.");
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

            Client.Subscribe(listenKey, user, callback);
        }

        public async Task UnsubscribeAsync<TEventArgs>(IBinanceApiUser user, Action<TEventArgs> callback, CancellationToken token = default)
            where TEventArgs : UserDataEventArgs
        {
            var listenKey = await _streamControl.GetStreamNameAsync(user, token)
                .ConfigureAwait(false);

            Client.Unsubscribe(listenKey, callback);

            if (callback == null || !Client.Publisher.PublishedStreams.Contains(listenKey))
            {
                await _streamControl.CloseStreamAsync(user, token)
                    .ConfigureAwait(false);
            }
        }

        public async Task UnsubscribeAllAsync(CancellationToken token = default)
        {
            foreach (var user in _streamControl.Users)
            {
                await UnsubscribeAsync(user, (Action<UserDataEventArgs>)null, token)
                    .ConfigureAwait(false);
            }
        }

        #endregion Public Methods

        #region IDisposble

        private bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                _streamControl?.Dispose();
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
