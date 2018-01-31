using System;
using System.Threading;
using System.Threading.Tasks;
using Binance.Api;
using Binance.WebSocket.Events;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Binance.WebSocket
{
    /// <summary>
    /// A <see cref="IUserDataWebSocketClient"/> implementation.
    /// </summary>
    public class SingleUserDataWebSocketClient : UserDataWebSocketClient
    {
        #region Private Fields

        private IBinanceApiUser User;

        private string _listenKey;

        private Timer _keepAliveTimer;

        #endregion Private Fields

        #region Constructors

        /// <summary> 
        /// Default constructor provides default Binance API and web socket stream, 
        /// but no options support or logging. 
        /// </summary> 
        public SingleUserDataWebSocketClient()
            : this(new BinanceApi(), new BinanceWebSocketStream())
        { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="api">The Binance API.</param>
        /// <param name="webSocket">The WebSocket stream.</param>
        /// <param name="options">The options.</param>
        /// <param name="logger">The logger.</param>
        public SingleUserDataWebSocketClient(IBinanceApi api, BinanceWebSocketStream webSocket, IOptions<UserDataWebSocketClientOptions> options = null, ILogger<SingleUserDataWebSocketClient> logger = null)
            : base(api, webSocket, options, logger)
        { }

        #endregion Construtors

        #region Public Methods

        public override async Task SubscribeAndStreamAsync(IBinanceApiUser user, Action<UserDataEventArgs> callback, CancellationToken token)
        {
            Throw.IfNull(user, nameof(user));

            if (!token.CanBeCanceled)
                throw new ArgumentException("Token must be capable of being in the canceled state.", nameof(token));

            token.ThrowIfCancellationRequested();

            if (User != null)
                throw new InvalidOperationException($"{nameof(SingleUserDataWebSocketClient)}: Already subscribed to a user.");

            User = user;

            try
            {
                _listenKey = await _api.UserStreamStartAsync(user, token)
                    .ConfigureAwait(false);

                if (string.IsNullOrWhiteSpace(_listenKey))
                    throw new Exception($"{nameof(IUserDataWebSocketClient)}: Failed to get listen key from API.");

                SubscribeStream(_listenKey, callback);

                var period = _options?.KeepAliveTimerPeriod ?? KeepAliveTimerPeriodDefault;
                period = Math.Min(Math.Max(period, KeepAliveTimerPeriodMin), KeepAliveTimerPeriodMax);

                _keepAliveTimer = new Timer(OnKeepAliveTimer, token, period, period);

                try
                {
                    await WebSocket.StreamAsync(token)
                        .ConfigureAwait(false);
                }
                finally
                {
                    _keepAliveTimer.Dispose();

                    await _api.UserStreamCloseAsync(User, _listenKey, CancellationToken.None)
                        .ConfigureAwait(false);
                }
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

        #region Private Methods

        /// <summary>
        /// Keep-alive timer callback.
        /// </summary>
        /// <param name="state"></param>
        private async void OnKeepAliveTimer(object state)
        {
            try
            {
                await _api.UserStreamKeepAliveAsync(User, _listenKey, (CancellationToken)state)
                    .ConfigureAwait(false);
            }
            catch (Exception e)
            {
                Logger?.LogWarning(e, $"{nameof(SingleUserDataWebSocketClient)}.{nameof(OnKeepAliveTimer)}: \"{e.Message}\"");
            }
        }

        #endregion Private Methods
    }
}
