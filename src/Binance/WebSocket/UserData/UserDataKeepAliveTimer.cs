using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Binance.Api;
using Microsoft.Extensions.Logging;

namespace Binance.WebSocket.UserData
{
    public sealed class UserDataKeepAliveTimer : IUserDataKeepAliveTimer
    {
        #region Public Constants

        public static readonly int PeriodDefault = 1800000; // 30 minutes

        #endregion Public Constants

        #region Public Properties

        public TimeSpan Period
        {
            get => _period;
            set
            {
                _timer.Change(TimeSpan.Zero, value);
                _period = value;
            }
        }
        private TimeSpan _period = TimeSpan.FromMilliseconds(PeriodDefault);

        public IEnumerable<IBinanceApiUser> Users
        {
            get { lock (_sync) return _listenKeys.Keys; }
        }

        #endregion Public Properties

        #region Private Fields

        private readonly IBinanceApi _api;

        private readonly ILogger<UserDataKeepAliveTimer> _logger;

        private readonly IDictionary<IBinanceApiUser, string> _listenKeys;

        private readonly Timer _timer;

        private readonly CancellationTokenSource _cts;

        private readonly object _sync = new object();

        #endregion Private Fields

        #region Constructors

        /// <summary>
        /// Default constructor without logging capability.
        /// </summary>
        public UserDataKeepAliveTimer()
            : this(new BinanceApi())
        { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="api"></param>
        /// <param name="logger"></param>
        public UserDataKeepAliveTimer(IBinanceApi api, ILogger <UserDataKeepAliveTimer> logger = null)
        {
            Throw.IfNull(api, nameof(api));

            _api = api;
            _logger = logger;

            _listenKeys = new Dictionary<IBinanceApiUser, string>();

            _cts = new CancellationTokenSource();

            _timer = new Timer(OnTimer, _cts.Token, _period, _period);
        }

        #endregion Constructors

        #region Public Methods

        public void Add(IBinanceApiUser user, string listenKey)
        {
            Throw.IfNull(user, nameof(user));
            Throw.IfNullOrWhiteSpace(listenKey, nameof(listenKey));

            ThrowIfDisposed();

            lock (_sync)
            {
                _listenKeys[user] = listenKey;
            }
        }

        public void Remove(IBinanceApiUser user)
        {
            Throw.IfNull(user, nameof(user));

            ThrowIfDisposed();

            lock (_sync)
            {
                if (!_listenKeys.ContainsKey(user))
                    return;

                _listenKeys.Remove(user);
            }
        }

        public void RemoveAll()
        {
            ThrowIfDisposed();

            lock (_sync)
            {
                _listenKeys.Clear();
            }
        }

        #endregion Public Methods

        #region Private Methods

        private async void OnTimer(object state)
        {
            KeyValuePair<IBinanceApiUser, string>[] listenKeys;

            lock (_sync)
            {
                if (_listenKeys.Count == 0)
                    return;

                listenKeys = _listenKeys.ToArray();
            }

            foreach (var _ in listenKeys)
            {
                try
                {
                    _logger?.LogDebug($"{nameof(UserDataKeepAliveTimer)}.{nameof(OnTimer)}: Keep-alive user stream (\"{_.Value}\").  [thread: {Thread.CurrentThread.ManagedThreadId}]");

                    var token = (CancellationToken)state;

                    await _api.UserStreamKeepAliveAsync(_.Key, _.Value, token)
                        .ConfigureAwait(false);
                }
                catch (OperationCanceledException) { /* ignored */ }
                catch (Exception e)
                {
                    _logger?.LogError(e, $"{nameof(UserDataKeepAliveTimer)}.{nameof(OnTimer)}: Failed to ping user data stream.");
                }
            }
        }

        #endregion Private Methods

        #region IDisposable

        private bool _disposed;

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(UserDataKeepAliveTimer));
        }

        void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                RemoveAll();
                _cts?.Cancel();

                _timer?.Dispose();
                _cts?.Dispose();
            }

            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion
    }
}
