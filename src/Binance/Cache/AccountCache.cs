using Binance.Account;
using Binance.Api;
using Binance.Api.WebSocket;
using Binance.Api.WebSocket.Events;
using Binance.Cache.Events;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Binance.Cache
{
    public class AccountCache : IAccountCache
    {
        #region Public Events

        public event EventHandler<AccountCacheEventArgs> Update;

        #endregion Public Events

        #region Public Properties

        public AccountInfo Account { get; private set; }

        public IUserDataWebSocketClient Client { get; private set; }

        #endregion Public Properties

        #region Private Fields

        private IBinanceApi _api;

        private ILogger<AccountCache> _logger;

        private bool _leaveWebSocketClientOpen;

        private BufferBlock<AccountUpdateEventArgs> _bufferBlock;
        private ActionBlock<AccountUpdateEventArgs> _actionBlock;

        private Action<AccountCacheEventArgs> _callback;

        #endregion Private Fields

        #region Constructors

        public AccountCache(IBinanceApi api, IUserDataWebSocketClient client, bool leaveWebSocketClientOpen = false, ILogger<AccountCache> logger = null)
        {
            Throw.IfNull(api, nameof(api));
            Throw.IfNull(client, nameof(client));

            _api = api;
            _logger = logger;

            Client = client;
            Client.AccountUpdate += OnAccountUpdate;
            _leaveWebSocketClientOpen = leaveWebSocketClientOpen;
        }

        #endregion Constructors

        #region Public Methods

        public Task SubscribeAsync(IBinanceApiUser user, CancellationToken token = default)
            => SubscribeAsync(user, null, token);

        public Task SubscribeAsync(IBinanceApiUser user, Action<AccountCacheEventArgs> callback, CancellationToken token = default)
        {
            Throw.IfNull(user, nameof(user));

            _callback = callback;

            _bufferBlock = new BufferBlock<AccountUpdateEventArgs>(new DataflowBlockOptions()
            {
                EnsureOrdered = true,
                CancellationToken = token,
                BoundedCapacity = DataflowBlockOptions.Unbounded,
                MaxMessagesPerTask = DataflowBlockOptions.Unbounded,
            });

            _actionBlock = new ActionBlock<AccountUpdateEventArgs>(@event =>
            {
                try { Modify(@event.Account); }
                catch (OperationCanceledException) { }
                catch (Exception e)
                {
                    _logger?.LogError(e, $"{nameof(AccountCache)}: \"{e.Message}\"");
                }
            }, new ExecutionDataflowBlockOptions()
            {
                BoundedCapacity = 1,
                EnsureOrdered = true,
                MaxDegreeOfParallelism = 1,
                CancellationToken = token,
                SingleProducerConstrained = true,
            });

            _bufferBlock.LinkTo(_actionBlock);

            return Client.SubscribeAsync(user, token: token);
        }

        #endregion Public Methods

        #region Protected Methods

        /// <summary>
        /// Raise account cache update event.
        /// </summary>
        /// <param name="args"></param>
        protected virtual void RaiseUpdateEvent(AccountCacheEventArgs args)
        {
            Throw.IfNull(args, nameof(args));

            try { Update?.Invoke(this, args); }
            catch (Exception e)
            {
                LogException(e, $"{nameof(AccountCache)}.{nameof(RaiseUpdateEvent)}");
                throw;
            }
        }

        /// <summary>
        /// Modify the account.
        /// </summary>
        /// <param name="account"></param>
        protected virtual void Modify(AccountInfo account)
        {
            Account = account;

            var eventArgs = new AccountCacheEventArgs(Account);

            _callback?.Invoke(eventArgs);
            RaiseUpdateEvent(eventArgs);
        }

        #endregion Protected Methods

        #region Private Methods

        /// <summary>
        /// <see cref="IUserDataWebSocketClient"/> event handler.
        /// </summary>
        /// <param name="sender">The <see cref="IUserDataWebSocketClient"/>.</param>
        /// <param name="event">The event arguments.</param>
        private void OnAccountUpdate(object sender, AccountUpdateEventArgs @event)
        {
            // Post event to buffer block (queue).
            _bufferBlock.Post(@event);
        }

        /// <summary>
        /// Log an exception if not already logged within this library.
        /// </summary>
        /// <param name="e"></param>
        /// <param name="source"></param>
        private void LogException(Exception e, string source)
        {
            if (!e.IsLogged())
            {
                _logger?.LogError(e, $"{source}: \"{e.Message}\"");
                e.Logged();
            }
        }

        #endregion Private Methods

        #region IDisposable

        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                Client.AccountUpdate -= OnAccountUpdate;

                if (!_leaveWebSocketClientOpen)
                {
                    Client.Dispose();
                }

                _bufferBlock?.Complete();
                _actionBlock?.Complete();
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
