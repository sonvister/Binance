using Binance.Accounts;
using Binance.Api.WebSocket.Events;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Binance.Api.WebSocket
{
    /// <summary>
    /// A <see cref="IUserDataWebSocketClient"/> implementation.
    /// </summary>
    public class UserDataWebSocketClient : BinanceWebSocketClient, IUserDataWebSocketClient
    {
        #region Public Events

        public event EventHandler<AccountUpdateEventArgs> AccountUpdate;

        #endregion Public Events

        #region Public Properties

        public IBinanceUser User { get; private set; }

        #endregion Public Properties

        #region Private Fields

        private IBinanceApi _api;

        private string _listenKey;

        private Timer _keepAliveTimer;

        #endregion Private Fields

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="api">The Binance API.</param>
        /// <param name="logger">The logger.</param>
        public UserDataWebSocketClient(IBinanceApi api, ILogger<TradesWebSocketClient> logger = null)
            : base(logger)
        {
            _api = api;
        }

        #endregion Construtors

        #region Public Methods

        public async Task SubscribeAsync(IBinanceUser user, CancellationToken token = default)
        {
            Throw.IfNull(user, nameof(user));

            User = user;

            if (_isSubscribed)
                throw new InvalidOperationException($"{nameof(UserDataWebSocketClient)} is already subscribed to a user.");

            _listenKey = await _api.UserStreamStartAsync(user, token)
                .ConfigureAwait(false);

            _keepAliveTimer = new Timer(OnKeepAliveTimer, token, 30000, 30000);

            await SubscribeAsync(_listenKey, json =>
            {
                try
                {
                    var eventArgs = DeserializeJson(json);
                    if (eventArgs != null)
                    {
                        FireUpdateEvent(eventArgs);
                    }
                }
                catch (OperationCanceledException) { }
                catch (Exception e)
                {
                    LogException(e, $"{nameof(UserDataWebSocketClient)}.{nameof(FireUpdateEvent)}");
                }
            }, token);
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Deserialize event JSON.
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        protected virtual AccountUpdateEventArgs DeserializeJson(string json)
        {
            Throw.IfNullOrWhiteSpace(json, nameof(json));

            try
            {
                _logger?.LogTrace($"{nameof(UserDataWebSocketClient)}.{nameof(DeserializeJson)}: \"{json}\"");

                var jObject = JObject.Parse(json);

                var eventType = jObject["e"].Value<string>();

                if (eventType == "outboundAccountInfo")
                {
                    var eventTime = jObject["E"].Value<long>();

                    var commissions = new AccountCommissions(
                        jObject["m"].Value<int>(),
                        jObject["t"].Value<int>(),
                        jObject["b"].Value<int>(),
                        jObject["s"].Value<int>());

                    var status = new AccountStatus(
                        jObject["T"].Value<bool>(),
                        jObject["W"].Value<bool>(),
                        jObject["D"].Value<bool>());

                    var balances = new List<AccountBalance>();
                    foreach (var entry in jObject["B"])
                    {
                        balances.Add(new AccountBalance(
                            entry["a"].Value<string>(),
                            entry["f"].Value<decimal>(),
                            entry["l"].Value<decimal>()));
                    }

                    return new AccountUpdateEventArgs(eventTime, new Account(commissions, status, balances));
                }
                else
                {
                    _logger?.LogWarning($"{nameof(UserDataWebSocketClient)}.{nameof(DeserializeJson)}: Unexpected event type ({eventType}).");
                }
            }
            catch (OperationCanceledException) { }
            catch (Exception e)
            {
                LogException(e, $"{nameof(UserDataWebSocketClient)}.{nameof(DeserializeJson)}");
                throw;
            }

            return null;
        }

        /// <summary>
        /// Fire account update event.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void FireUpdateEvent(AccountUpdateEventArgs args)
        {
            Throw.IfNull(args, nameof(args));

            try { AccountUpdate?.Invoke(this, args); }
            catch (Exception e)
            {
                LogException(e, $"{nameof(UserDataWebSocketClient)}.{nameof(FireUpdateEvent)}");
                throw;
            }
        }

        #endregion Protected Methods

        #region Private Methods

        private async void OnKeepAliveTimer(object state)
        {
            try
            {
                await _api.UserStreamKeepAliveAsync(User, _listenKey, (CancellationToken)state)
                    .ConfigureAwait(false);
            }
            catch (Exception e)
            {
                LogException(e, $"{nameof(UserDataWebSocketClient)}.{nameof(OnKeepAliveTimer)}");
            }
        }

        #endregion Private Methods

        #region IDisposable

        private bool _disposed = false;

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                try
                {
                    _keepAliveTimer?.Dispose();

                    if (!string.IsNullOrWhiteSpace(_listenKey))
                    {
                        _api.UserStreamCloseAsync(User, _listenKey)
                            .GetAwaiter().GetResult();
                    }
                }
                catch (OperationCanceledException) { }
                catch (Exception e)
                {
                    _logger?.LogError(e, $"{nameof(UserDataWebSocketClient)}.{nameof(Dispose)}: \"{e.Message}\"");
                }
            }

            _disposed = true;

            base.Dispose(disposing);
        }

        #endregion IDisposable
    }
}
