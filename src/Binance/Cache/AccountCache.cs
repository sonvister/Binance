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

        public IUserDataWebSocketClient Client { get; }

        #endregion Public Properties

        #region Private Fields

        private readonly ILogger<AccountCache> _logger;

        private bool _leaveClientOpen;

        private BufferBlock<AccountUpdateEventArgs> _bufferBlock;
        private ActionBlock<AccountUpdateEventArgs> _actionBlock;

        private Action<AccountCacheEventArgs> _callback;

        private CancellationToken _token;

        #endregion Private Fields

        #region Constructors

        public AccountCache(IUserDataWebSocketClient client, bool leaveClientOpen = false, ILogger<AccountCache> logger = null)
        {
            Throw.IfNull(client, nameof(client));

            Client = client;
            _leaveClientOpen = leaveClientOpen;
            _logger = logger;
        }

        #endregion Constructors

        #region Public Methods

        public Task SubscribeAsync(IBinanceApiUser user, CancellationToken token = default)
            => SubscribeAsync(user, null, token);

        public Task SubscribeAsync(IBinanceApiUser user, Action<AccountCacheEventArgs> callback, CancellationToken token = default)
        {
            Throw.IfNull(user, nameof(user));

            _token = token;

            LinkTo(Client, callback, _leaveClientOpen);

            return Client.SubscribeAsync(user, token: token);
        }

        public void LinkTo(IUserDataWebSocketClient client, Action<AccountCacheEventArgs> callback = null, bool leaveClientOpen = true)
        {
            Throw.IfNull(client, nameof(client));

            if (_bufferBlock != null)
            {
                if (client == Client)
                    throw new InvalidOperationException($"{nameof(AccountCache)} is already linked to this {nameof(IUserDataWebSocketClient)}.");

                throw new InvalidOperationException($"{nameof(AccountCache)} is linked to another {nameof(IUserDataWebSocketClient)}.");
            }

            _callback = callback;
            _leaveClientOpen = leaveClientOpen;

            _bufferBlock = new BufferBlock<AccountUpdateEventArgs>(new DataflowBlockOptions()
            {
                EnsureOrdered = true,
                CancellationToken = _token,
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
                CancellationToken = _token,
                SingleProducerConstrained = true,
            });

            _bufferBlock.LinkTo(_actionBlock);

            Client.AccountUpdate += OnAccountUpdate;
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

        private bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                Client.AccountUpdate -= OnAccountUpdate;

                if (!_leaveClientOpen)
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
