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
    public class UserDataWebSocketManager : UserDataWebSocketClient, IUserDataWebSocketManager
    {
        #region Public Properties

        public IUserDataWebSocketClient Client => this; // TODO

        #endregion Public Properties

        #region Private Fields

        private readonly IUserDataKeepAliveTimerProvider _timerProvider;

        private readonly UserDataWebSocketManagerOptions _options;

        private IBinanceApiUser User;

        private string _listenKey;

        #endregion Private Fields

        #region Constructors

        /// <summary> 
        /// Default constructor provides default Binance API and web socket stream, 
        /// but no options support or logging. 
        /// </summary> 
        public UserDataWebSocketManager()
            : this(new BinanceApi(), new WebSocketStreamProvider(), new UserDataKeepAliveTimerProvider())
        { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="api">The Binance API.</param>
        /// <param name="streamProvider">The WebSocket stream provider.</param>
        /// <param name="timerProvider">The keep-alive timer provider.</param>
        /// <param name="options">The options.</param>
        /// <param name="logger">The logger.</param>
        public UserDataWebSocketManager(IBinanceApi api, IWebSocketStreamProvider streamProvider, IUserDataKeepAliveTimerProvider timerProvider, IOptions<UserDataWebSocketManagerOptions> options = null, ILogger<UserDataWebSocketClient> logger = null)
            : base(api, streamProvider?.CreateStream(), logger)
        {
            Throw.IfNull(timerProvider, nameof(timerProvider));

            _timerProvider = timerProvider;
            _options = options?.Value;
        }

        #endregion Construtors

        #region Public Methods

        public async Task SubscribeAndStreamAsync(IBinanceApiUser user, Action<UserDataEventArgs> callback, CancellationToken token)
        {
            Throw.IfNull(user, nameof(user));

            if (!token.CanBeCanceled)
                throw new ArgumentException("Token must be capable of being in the canceled state.", nameof(token));

            token.ThrowIfCancellationRequested();

            if (User != null && !User.Equals(user))
                throw new InvalidOperationException($"{nameof(UserDataWebSocketManager)}: Already subscribed to a user.");

            try
            {
                if (User != null && _listenKey != null)
                {
                    Logger?.LogDebug($"{nameof(UserDataWebSocketManager)}.{nameof(SubscribeAndStreamAsync)}: Closing user stream (\"{_listenKey}\").  [thread: {Thread.CurrentThread.ManagedThreadId}]");

                    await _api.UserStreamCloseAsync(User, _listenKey, token)
                        .ConfigureAwait(false);
                }

                User = user;

                Logger?.LogDebug($"{nameof(UserDataWebSocketManager)}.{nameof(SubscribeAndStreamAsync)}: Starting user stream...  [thread: {Thread.CurrentThread.ManagedThreadId}]");

                _listenKey = await _api.UserStreamStartAsync(user, token)
                    .ConfigureAwait(false);

                if (string.IsNullOrWhiteSpace(_listenKey))
                    throw new Exception($"{nameof(IUserDataWebSocketClient)}: Failed to get listen key from API.");

                Logger?.LogDebug($"{nameof(UserDataWebSocketManager)}.{nameof(SubscribeAndStreamAsync)}: User stream open (\"{_listenKey}\").  [thread: {Thread.CurrentThread.ManagedThreadId}]");

                var period =
                    Math.Min(
                        Math.Max(
                            _options?.KeepAliveTimerPeriod ?? UserDataKeepAliveTimer.PeriodDefault,
                            KeepAliveTimerPeriodMin),
                        KeepAliveTimerPeriodMax);

                var timer = _timerProvider.CreateTimer(period);

                timer.Add(User, _listenKey);

                Subscribe(_listenKey, User, callback);

                try
                {
                    Logger?.LogDebug($"{nameof(UserDataWebSocketManager)}.{nameof(SubscribeAndStreamAsync)}: Streaming.  [thread: {Thread.CurrentThread.ManagedThreadId}]");

                    await WebSocket.StreamAsync(token)
                        .ConfigureAwait(false);
                }
                catch (OperationCanceledException) { }
                finally
                {
                    UnsubscribeStream(_listenKey, callback);
                    timer.Dispose();
                }

                Logger?.LogDebug($"{nameof(UserDataWebSocketManager)}.{nameof(SubscribeAndStreamAsync)}: Closing user stream (\"{_listenKey}\").  [thread: {Thread.CurrentThread.ManagedThreadId}]");

                await _api.UserStreamCloseAsync(User, _listenKey, token)
                    .ConfigureAwait(false);

                _listenKey = null;
                User = null;
            }
            catch (OperationCanceledException) { }
            catch (Exception e)
            {
                if (!token.IsCancellationRequested)
                {
                    Logger?.LogError(e, $"{nameof(UserDataWebSocketManager)}.{nameof(SubscribeAndStreamAsync)}");
                    throw;
                }
            }
        }

        #endregion Public Methods
    }
}
