using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Binance.Api;
using Binance.WebSocket.Events;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Binance.WebSocket.UserData
{
    /// <summary>
    /// A <see cref="IMultiUserDataWebSocketClient"/> implementation.
    /// </summary>
    public class MultiUserDataWebSocketClient : UserDataWebSocketClient, IMultiUserDataWebSocketClient
    {
        #region Private Fields

        private Timer _keepAliveTimer;

        #endregion Private Fields

        #region Constructors

        /// <summary>
        /// Default constructor provides default Binance API and web socket client,
        /// but no options support or logging.
        /// </summary>
        public MultiUserDataWebSocketClient()
            : this(new BinanceApi(), new BinanceWebSocketStream())
        { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="api">The Binance API.</param>
        /// <param name="webSocket">The WebSocket stream.</param>
        /// <param name="options">The options.</param>
        /// <param name="logger">The logger.</param>
        public MultiUserDataWebSocketClient(IBinanceApi api, IWebSocketStream webSocket, IOptions<UserDataWebSocketManagerOptions> options = null, ILogger<UserDataWebSocketClient> logger = null)
            : base(api, webSocket, logger)
        {
            webSocket.Open += (s, e) =>
            {
                var period =
                    Math.Min(
                        Math.Max(
                            options?.Value.KeepAliveTimerPeriod ?? UserDataKeepAliveTimer.PeriodDefault,
                            KeepAliveTimerPeriodMin),
                        KeepAliveTimerPeriodMax);

                _keepAliveTimer = new Timer(OnKeepAliveTimer, CancellationToken.None, period, period);
            };

            webSocket.Close += async (s, e) =>
            {
                // TODO: Add logging...

                _keepAliveTimer.Dispose();

                foreach (var _ in _listenKeys)
                {
                    // TODO: Close user stream... what if disconnected... ?

                    await _api.UserStreamCloseAsync(_.Value, _.Key, CancellationToken.None)
                        .ConfigureAwait(false);

                    // TODO: Unsubscribe listen key...

                    // TODO: Query new listen key and subscribe...

                    // TODO: Ensure this is done before streaming begins... ?
                }
            };
        }

        #endregion Construtors

        #region Public Methods

        public virtual async Task SubscribeAsync(IBinanceApiUser user, Action<UserDataEventArgs> callback, CancellationToken token = default)
        {
            Throw.IfNull(user, nameof(user));

            try
            {
                var listenKey = _listenKeys.SingleOrDefault(_ => _.Value == user).Key;

                if (listenKey == null)
                {
                    listenKey = await _api.UserStreamStartAsync(user, token)
                        .ConfigureAwait(false);

                    if (string.IsNullOrWhiteSpace(listenKey))
                        throw new Exception($"{nameof(MultiUserDataWebSocketClient)}: Failed to get listen key from API.");
                }

                Subscribe(listenKey, user, callback);
            }
            catch (OperationCanceledException) { }
            catch (Exception e)
            {
                if (!token.IsCancellationRequested)
                {
                    Logger?.LogError(e, $"{nameof(MultiUserDataWebSocketClient)}.{nameof(SubscribeAsync)}");
                    throw;
                }
            }
        }

        public virtual async Task UnsubscribeAsync(IBinanceApiUser user, Action<UserDataEventArgs> callback, CancellationToken token = default)
        {
            // TODO: ...

            await Task.CompletedTask;
        }

        public async Task SubscribeAndStreamAsync(IBinanceApiUser user, Action<UserDataEventArgs> callback, CancellationToken token)
        {
            await SubscribeAsync(user, callback, token)
                .ConfigureAwait(false);

            await WebSocket.StreamAsync(token)
                .ConfigureAwait(false);
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Keep-alive timer callback.
        /// </summary>
        /// <param name="state"></param>
        private async void OnKeepAliveTimer(object state)
        {
            foreach (var _ in _listenKeys)
            {
                try
                {
                    await _api.UserStreamKeepAliveAsync(_.Value, _.Key, (CancellationToken)state)
                        .ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    // TODO: ...

                    Logger?.LogWarning(e, $"{nameof(MultiUserDataWebSocketClient)}.{nameof(OnKeepAliveTimer)}: \"{e.Message}\"");
                }
            }
        }

        #endregion Private Methods
    }
}
