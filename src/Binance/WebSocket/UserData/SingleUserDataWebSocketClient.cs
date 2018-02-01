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
    /// A <see cref="IUserDataWebSocketClient"/> implementation.
    /// </summary>
    public class SingleUserDataWebSocketClient : UserDataWebSocketClient
    {
        #region Private Fields

        private readonly IUserDataKeepAliveTimerProvider _timerProvider;

        private IBinanceApiUser User;

        private string _listenKey;

        #endregion Private Fields

        #region Constructors

        /// <summary> 
        /// Default constructor provides default Binance API and web socket stream, 
        /// but no options support or logging. 
        /// </summary> 
        public SingleUserDataWebSocketClient()
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
        public SingleUserDataWebSocketClient(IBinanceApi api, IWebSocketStreamProvider streamProvider, IUserDataKeepAliveTimerProvider timerProvider, IOptions<UserDataWebSocketClientOptions> options = null, ILogger<SingleUserDataWebSocketClient> logger = null)
            : base(api, streamProvider?.CreateStream(), options, logger)
        {
            Throw.IfNull(timerProvider, nameof(timerProvider));

            _timerProvider = timerProvider;
        }

        #endregion Construtors

        #region Public Methods

        public override async Task SubscribeAndStreamAsync(IBinanceApiUser user, Action<UserDataEventArgs> callback, CancellationToken token)
        {
            Throw.IfNull(user, nameof(user));

            if (!token.CanBeCanceled)
                throw new ArgumentException("Token must be capable of being in the canceled state.", nameof(token));

            token.ThrowIfCancellationRequested();

            if (User != null && !User.Equals(user))
                throw new InvalidOperationException($"{nameof(SingleUserDataWebSocketClient)}: Already subscribed to a user.");

            try
            {
                if (User != null && _listenKey != null)
                {
                    Logger?.LogDebug($"{nameof(SingleUserDataWebSocketClient)}.{nameof(SubscribeAndStreamAsync)}: Closing user stream (\"{_listenKey}\").  [thread: {Thread.CurrentThread.ManagedThreadId}]");

                    await _api.UserStreamCloseAsync(User, _listenKey, token)
                        .ConfigureAwait(false);
                }

                User = user;

                Logger?.LogDebug($"{nameof(SingleUserDataWebSocketClient)}.{nameof(SubscribeAndStreamAsync)}: Starting user stream...  [thread: {Thread.CurrentThread.ManagedThreadId}]");

                _listenKey = await _api.UserStreamStartAsync(user, token)
                    .ConfigureAwait(false);

                if (string.IsNullOrWhiteSpace(_listenKey))
                    throw new Exception($"{nameof(IUserDataWebSocketClient)}: Failed to get listen key from API.");

                Logger?.LogDebug($"{nameof(SingleUserDataWebSocketClient)}.{nameof(SubscribeAndStreamAsync)}: User stream open (\"{_listenKey}\").  [thread: {Thread.CurrentThread.ManagedThreadId}]");

                var period =
                    Math.Min(
                        Math.Max(
                            _options?.KeepAliveTimerPeriod ?? UserDataKeepAliveTimer.PeriodDefault,
                            KeepAliveTimerPeriodMin),
                        KeepAliveTimerPeriodMax);

                var timer = _timerProvider.CreateTimer(period);

                timer.Add(User, _listenKey);

                SubscribeStream(_listenKey, callback);

                try
                {
                    Logger?.LogDebug($"{nameof(SingleUserDataWebSocketClient)}.{nameof(SubscribeAndStreamAsync)}: Streaming.  [thread: {Thread.CurrentThread.ManagedThreadId}]");

                    await WebSocket.StreamAsync(token)
                        .ConfigureAwait(false);
                }
                catch (OperationCanceledException) { }
                finally
                {
                    UnsubscribeStream(_listenKey, callback);
                    timer.Dispose();
                }

                Logger?.LogDebug($"{nameof(SingleUserDataWebSocketClient)}.{nameof(SubscribeAndStreamAsync)}: Closing user stream (\"{_listenKey}\").  [thread: {Thread.CurrentThread.ManagedThreadId}]");

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
                    Logger?.LogError(e, $"{nameof(SingleUserDataWebSocketClient)}.{nameof(SubscribeAndStreamAsync)}");
                    throw;
                }
            }
        }

        #endregion Public Methods

        #region Protected Methods

        protected override IBinanceApiUser GetUserForEvent(WebSocketStreamEventArgs args)
        {
            return User;
        }

        #endregion Protected Methods
    }
}
