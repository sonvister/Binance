using System;
using System.Threading;
using System.Threading.Tasks;
using Binance.Api;
using Binance.WebSocket.Events;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Binance.WebSocket.UserData
{
    /// <summary>
    /// A <see cref="IUserDataWebSocketManager"/> implementation.
    /// </summary>
    public class UserDataWebSocketManager : IUserDataWebSocketManager
    {
        #region Public Constants

        public static readonly int KeepAliveTimerPeriodMax = 3600000; // 1 hour
        public static readonly int KeepAliveTimerPeriodMin = 60000; // 1 minute

        #endregion Public Constants

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

        public ISingleUserDataWebSocketClient Client { get; }

        #endregion Public Properties

        #region Private Fields

        private readonly IBinanceApi _api;

        private readonly IUserDataKeepAliveTimerProvider _timerProvider;

        private readonly UserDataWebSocketManagerOptions _options;

        private readonly ILogger<UserDataWebSocketClient> _logger;

        private IBinanceApiUser _user;

        private string _listenKey;

        #endregion Private Fields

        #region Constructors

        /// <summary> 
        /// Default constructor provides default Binance API and web socket stream, 
        /// but no options support or logging. 
        /// </summary> 
        public UserDataWebSocketManager()
            : this(new BinanceApi(), new SingleUserDataWebSocketClient(), new UserDataKeepAliveTimerProvider())
        { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="api">The Binance API.</param>
        /// <param name="client">The user data web socket client.</param>
        /// <param name="timerProvider">The keep-alive timer provider.</param>
        /// <param name="options">The options.</param>
        /// <param name="logger">The logger.</param>
        public UserDataWebSocketManager(IBinanceApi api, ISingleUserDataWebSocketClient client, IUserDataKeepAliveTimerProvider timerProvider, IOptions<UserDataWebSocketManagerOptions> options = null, ILogger<UserDataWebSocketClient> logger = null)
        {
            Throw.IfNull(api, nameof(api));
            Throw.IfNull(client, nameof(client));
            Throw.IfNull(timerProvider, nameof(timerProvider));

            _api = api;
            Client = client;
            _timerProvider = timerProvider;
            _options = options?.Value;
            _logger = logger;
        }

        #endregion Construtors

        #region Public Methods

        public async Task SubscribeAndStreamAsync(IBinanceApiUser user, Action<UserDataEventArgs> callback, CancellationToken token)
        {
            Throw.IfNull(user, nameof(user));

            if (!token.CanBeCanceled)
                throw new ArgumentException("Token must be capable of being in the canceled state.", nameof(token));

            token.ThrowIfCancellationRequested();

            if (_user != null && !_user.Equals(user))
                throw new InvalidOperationException($"{nameof(UserDataWebSocketManager)}: Already subscribed to a user.");

            try
            {
                if (_user != null && _listenKey != null)
                {
                    _logger?.LogDebug($"{nameof(UserDataWebSocketManager)}.{nameof(SubscribeAndStreamAsync)}: Closing user stream (\"{_listenKey}\").  [thread: {Thread.CurrentThread.ManagedThreadId}]");

                    await _api.UserStreamCloseAsync(_user, _listenKey, token)
                        .ConfigureAwait(false);
                }

                _user = user;

                _logger?.LogDebug($"{nameof(UserDataWebSocketManager)}.{nameof(SubscribeAndStreamAsync)}: Starting user stream...  [thread: {Thread.CurrentThread.ManagedThreadId}]");

                _listenKey = await _api.UserStreamStartAsync(user, token)
                    .ConfigureAwait(false);

                if (string.IsNullOrWhiteSpace(_listenKey))
                    throw new Exception($"{nameof(IUserDataWebSocketClient)}: Failed to get listen key from API.");

                _logger?.LogDebug($"{nameof(UserDataWebSocketManager)}.{nameof(SubscribeAndStreamAsync)}: User stream open (\"{_listenKey}\").  [thread: {Thread.CurrentThread.ManagedThreadId}]");

                var period =
                    Math.Min(
                        Math.Max(
                            _options?.KeepAliveTimerPeriod ?? UserDataKeepAliveTimer.PeriodDefault,
                            KeepAliveTimerPeriodMin),
                        KeepAliveTimerPeriodMax);

                var timer = _timerProvider.CreateTimer(period);

                timer.Add(_user, _listenKey);

                Client.Subscribe(_listenKey, _user, callback);

                try
                {
                    _logger?.LogDebug($"{nameof(UserDataWebSocketManager)}.{nameof(SubscribeAndStreamAsync)}: Streaming.  [thread: {Thread.CurrentThread.ManagedThreadId}]");

                    await Client.WebSocket.StreamAsync(token)
                        .ConfigureAwait(false);
                }
                catch (OperationCanceledException) { }
                finally
                {
                    Client.Unsubscribe(_listenKey, callback);
                    timer.Dispose();
                }

                _logger?.LogDebug($"{nameof(UserDataWebSocketManager)}.{nameof(SubscribeAndStreamAsync)}: Closing user stream (\"{_listenKey}\").  [thread: {Thread.CurrentThread.ManagedThreadId}]");

                await _api.UserStreamCloseAsync(_user, _listenKey, token)
                    .ConfigureAwait(false);

                _listenKey = null;
                _user = null;
            }
            catch (OperationCanceledException) { }
            catch (Exception e)
            {
                if (!token.IsCancellationRequested)
                {
                    _logger?.LogError(e, $"{nameof(UserDataWebSocketManager)}.{nameof(SubscribeAndStreamAsync)}");
                    throw;
                }
            }
        }

        #endregion Public Methods
    }
}
