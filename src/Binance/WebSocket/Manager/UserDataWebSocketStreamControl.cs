using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

// ReSharper disable once CheckNamespace
namespace Binance.WebSocket
{
    /// <summary>
    /// The default <see cref="IUserDataWebSocketStreamControl"/> implementation.
    /// </summary>
    public sealed class UserDataWebSocketStreamControl : IUserDataWebSocketStreamControl
    {
        #region Public Events

        public event EventHandler<UserDataListenKeyUpdateEventArgs> ListenKeyUpdate;

        #endregion Public Events

        #region Public Constants

        public static readonly TimeSpan KeepAliveTimerPeriodDefault = TimeSpan.FromMinutes(15);

        #endregion Public Constants

        #region Public Properties

        public TimeSpan KeepAliveTimerPeriod
        {
            get => _period;
            set
            {
                _timer.Change(TimeSpan.Zero, value);
                _period = value;
            }
        }
        private TimeSpan _period = KeepAliveTimerPeriodDefault;

        public IEnumerable<IBinanceApiUser> Users => _listenKeys.Keys.ToArray();

        #endregion Public Properties

        #region Private Fields

        private readonly IBinanceApi _api;

        private readonly ILogger<UserDataWebSocketStreamControl> _logger;

        private readonly IDictionary<IBinanceApiUser, string> _listenKeys;

        private readonly SemaphoreSlim _syncLock = new SemaphoreSlim(1, 1);

        private readonly CancellationTokenSource _cts;

        private Timer _timer;

        #endregion Private Fields

        #region Constructors

        /// <summary>
        /// Default constructor without logging capability.
        /// </summary>
        public UserDataWebSocketStreamControl()
            : this(new BinanceApi())
        { }

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="api"></param>
        /// <param name="logger"></param>
        public UserDataWebSocketStreamControl(IBinanceApi api, ILogger<UserDataWebSocketStreamControl> logger = null)
        {
            Throw.IfNull(api, nameof(api));

            _api = api;
            _logger = logger;

            _listenKeys = new Dictionary<IBinanceApiUser, string>();

            _cts = new CancellationTokenSource();
        }

        #endregion Constructors

        #region Public Methods

        public async Task<string> GetStreamNameAsync(IBinanceApiUser user, CancellationToken token = default)
        {
            Throw.IfNull(user, nameof(user));

            ThrowIfDisposed();

            // Acquire synchronization lock.
            await _syncLock.WaitAsync(token)
                .ConfigureAwait(false);

            try
            {
                return _listenKeys.TryGetValue(user, out var listenKey)
                    ? listenKey
                    : null;
            }
            finally
            {
                // Release synchronization lock.
                _syncLock.Release();
            }
        }

        public async Task<string> OpenStreamAsync(IBinanceApiUser user, CancellationToken token = default)
        {
            Throw.IfNull(user, nameof(user));

            ThrowIfDisposed();

            // Acquire synchronization lock.
            await _syncLock.WaitAsync(token)
                .ConfigureAwait(false);

            try
            {
                if (_listenKeys.TryGetValue(user, out var listenKey))
                    return listenKey;

                listenKey = await _api.UserStreamStartAsync(user, token)
                    .ConfigureAwait(false);

                _listenKeys[user] = listenKey;

                if (_timer == null)
                {
                    _timer = new Timer(HandleTimer, _cts.Token, KeepAliveTimerPeriod, KeepAliveTimerPeriod);
                }

                return listenKey;
            }
            finally
            {
                // Release synchronization lock.
                _syncLock.Release();
            }
        }

        public async Task CloseStreamAsync(IBinanceApiUser user, CancellationToken token = default)
        {
            Throw.IfNull(user, nameof(user));

            ThrowIfDisposed();

            // Acquire synchronization lock.
            await _syncLock.WaitAsync(token)
                .ConfigureAwait(false);

            try
            {
                if (!_listenKeys.TryGetValue(user, out var listenKey))
                    return;

                await _api.UserStreamCloseAsync(user, listenKey, token)
                    .ConfigureAwait(false);

                _listenKeys.Remove(user);
            }
            finally
            {
                // Release synchronization lock.
                _syncLock.Release();
            }
        }

        public async Task CloseAllStreamsAsync(CancellationToken token = default)
        {
            ThrowIfDisposed();

            // Acquire synchronization lock.
            await _syncLock.WaitAsync(token)
                .ConfigureAwait(false);

            try
            {
                if (_listenKeys.Count == 0)
                    return;

                foreach (var userAndListenKey in _listenKeys.ToArray())
                {
                    await _api.UserStreamCloseAsync(userAndListenKey.Key, userAndListenKey.Value, token)
                        .ConfigureAwait(false);

                    _listenKeys.Remove(userAndListenKey.Key);
                }
            }
            finally
            {
                // Release synchronization lock.
                _syncLock.Release();
            }
        }

        #endregion Public Methods

        #region Private Methods

        private async void HandleTimer(object state)
        {
            // Acquire synchronization lock.
            await _syncLock.WaitAsync(_cts.Token)
                .ConfigureAwait(false);

            try
            {
                if (_listenKeys.Count == 0)
                    return;

                // Get users and listen keys.
                var listenKeys = _listenKeys.ToArray();

                var token = (CancellationToken)state;

                var failedListenKeys = new List<KeyValuePair<IBinanceApiUser, string>>();

                // Ping user data stream and keep list of failed listen keys.
                foreach (var userAndListenKey in listenKeys)
                {
                    try
                    {
                        _logger?.LogDebug($"{nameof(UserDataWebSocketStreamControl)}.{nameof(HandleTimer)}: Keep-alive user stream ({userAndListenKey.Value}).  [thread: {Thread.CurrentThread.ManagedThreadId}]");

                        await _api.UserStreamKeepAliveAsync(userAndListenKey.Key, userAndListenKey.Value, token)
                            .ConfigureAwait(false);
                    }
                    catch (OperationCanceledException) { /* ignore */ }
                    catch (Exception e)
                    {
                        _logger?.LogWarning(e, $"{nameof(UserDataWebSocketStreamControl)}.{nameof(HandleTimer)}: Failed to ping user data stream (1st attempt).");
                        failedListenKeys.Add(userAndListenKey);
                    }
                }

                // Wait a bit and retry failed attempts...
                await Task.Delay(100, token)
                    .ConfigureAwait(false);

                foreach (var userAndListenKey in failedListenKeys.ToArray())
                {
                    try
                    {
                        _logger?.LogDebug($"{nameof(UserDataWebSocketStreamControl)}.{nameof(HandleTimer)}: Keep-alive user stream ({userAndListenKey.Value}).  [thread: {Thread.CurrentThread.ManagedThreadId}]");

                        await _api.UserStreamKeepAliveAsync(userAndListenKey.Key, userAndListenKey.Value, token)
                            .ConfigureAwait(false);

                        // If successful, remove listen key from list.
                        failedListenKeys.Remove(userAndListenKey);
                    }
                    catch (OperationCanceledException) { /* ignore */ }
                    catch (Exception e)
                    {
                        _logger?.LogWarning(e, $"{nameof(UserDataWebSocketStreamControl)}.{nameof(HandleTimer)}: Failed to ping user data stream (2nd attempt).");
                    }
                }

                // Wait a bit and close user streams, get new listen keys, and notify listeners.
                await Task.Delay(100, token)
                    .ConfigureAwait(false);

                foreach (var userAndListenKey in failedListenKeys.ToArray())
                {
                    try
                    {
                        _logger?.LogDebug($"{nameof(UserDataWebSocketStreamControl)}.{nameof(HandleTimer)}: Close user stream ({userAndListenKey.Value}).  [thread: {Thread.CurrentThread.ManagedThreadId}]");

                        await _api.UserStreamCloseAsync(userAndListenKey.Key, userAndListenKey.Value, token)
                            .ConfigureAwait(false);

                        var newListenKey = await _api.UserStreamStartAsync(userAndListenKey.Key, token)
                            .ConfigureAwait(false);

                        // Update the user listen key.
                        _listenKeys[userAndListenKey.Key] = newListenKey;

                        // Notify listeners of the listen key update.
                        OnUpdateListenKey(userAndListenKey.Key, userAndListenKey.Value, newListenKey);

                        // If successful, remove from list.
                        failedListenKeys.Remove(userAndListenKey);
                    }
                    catch (OperationCanceledException) { /* ignore */ }
                    catch (Exception e)
                    {
                        _logger?.LogError(e, $"{nameof(UserDataWebSocketStreamControl)}.{nameof(HandleTimer)}: Failed to get new listen key.");
                    }
                }

                // Notify listeners of any listen key failures remaining (null new key).
                foreach (var userAndListenKey in failedListenKeys)
                {
                    OnUpdateListenKey(userAndListenKey.Key, userAndListenKey.Value, null);
                }
            }
            catch (Exception e)
            {
                _logger?.LogError(e, $"{nameof(UserDataWebSocketStreamControl)}.{nameof(HandleTimer)}: Failed.");
            }
            finally
            {
                // Release synchronization lock.
                _syncLock.Release();
            }
        }

        private void OnUpdateListenKey(IBinanceApiUser user, string oldListenKey, string newListenKey)
        {
            var args = new UserDataListenKeyUpdateEventArgs(user, oldListenKey, newListenKey);

            try { ListenKeyUpdate?.Invoke(this, args); }
            catch (OperationCanceledException) { }
            catch (Exception e)
            {
                _logger?.LogWarning(e, $"{nameof(UserDataWebSocketStreamControl)}: Unhandled {nameof(ListenKeyUpdate)} event handler exception.");
            }
        }

        #endregion Private Methods

        #region IDisposable

        private bool _disposed;

        private void ThrowIfDisposed()
        {
            if (_disposed)
                throw new ObjectDisposedException(nameof(UserDataWebSocketStreamControl));
        }

        private void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                try
                {
                    CloseAllStreamsAsync().GetAwaiter().GetResult();

                    _cts?.Cancel();
                    _timer?.Dispose();
                    _cts?.Dispose();

                    _syncLock?.Dispose();
                }
                catch (Exception e)
                {
                    _logger?.LogError(e, $"{nameof(UserDataWebSocketStreamControl)}.{nameof(Dispose)}: Failed.");
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
