using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Binance.Client;
using Binance.Stream;
using Binance.Utility;
using Microsoft.Extensions.Logging;

namespace Binance.Manager
{
    public abstract class JsonStreamClientManager : JsonStreamClientManager<IJsonClient, IJsonStream>
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="client">The JSON client (required).</param>
        /// <param name="controller">The JSON stream controller (required).</param>
        /// <param name="logger">The logger (optional).</param>
        protected JsonStreamClientManager(IJsonClient client, IJsonStreamController controller, ILogger<JsonStreamClientManager> logger = null)
            : base(client, controller, logger)
        { }

        #endregion Constructors
    }

    public abstract class JsonStreamClientManager<TClient> : JsonStreamClientManager<TClient, IJsonStream>
        where TClient : IJsonClient
    {
        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="client">The JSON client (required).</param>
        /// <param name="controller">The JSON stream controller (required).</param>
        /// <param name="logger">The logger (optional).</param>
        protected JsonStreamClientManager(TClient client, IJsonStreamController controller, ILogger<JsonStreamClientManager<TClient>> logger = null)
            : base(client, controller, logger)
        { }

        #endregion Constructors
    }

    public abstract class JsonStreamClientManager<TClient, TStream> : JsonStreamClient<TClient, TStream>, IControllerManager<TStream>
        where TClient : IJsonClient
        where TStream : IJsonStream
    {
        #region Public Properties

        public IJsonStreamController<TStream> Controller { get; }

        public IWatchdogTimer Watchdog { get; }

        #endregion Public Properties

        #region Private Fields

        private bool _controllerActive;

        private Task Task = Task.CompletedTask;

        private readonly object _sync = new object();

        #endregion Private Fields

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="client">The JSON client (required).</param>
        /// <param name="controller">The JSON stream controller (required).</param>
        /// <param name="logger">The logger (optional).</param>
        protected JsonStreamClientManager(TClient client, IJsonStreamController<TStream> controller, ILogger<JsonStreamClientManager<TClient, TStream>> logger = null)
            : base(client, controller.Stream, logger)
        {
            Throw.IfNull(controller, nameof(controller));

            Controller = controller;

            Watchdog = new WatchdogTimer(() => Controller.Abort())
            {
                Interval = TimeSpan.FromHours(1)
            };
        }

        #endregion Constructors

        #region Public Methods

        public override Task HandleMessageAsync(string stream, string json, CancellationToken token = default)
        {
            Watchdog.Kick();

            return base.HandleMessageAsync(stream, json, token);
        }

        public override void Unsubscribe()
        {
            lock (_sync)
            {
                try
                {
                    base.Unsubscribe();

                    // Cancel streaming if there are no subscribed streams and controller is active.
                    if (_controllerActive && !Stream.ProvidedStreams.Any())
                    {
                        Logger?.LogDebug($"{GetType().Name}.{nameof(HandleUnsubscribe)}: Cancel streaming...  [thread: {Thread.CurrentThread.ManagedThreadId}]");

                        _controllerActive = false;

                        // Add controller cancel task continuation.
                        Task = Task.ContinueWith(async _ =>
                        {
                            try { await Controller.CancelAsync().ConfigureAwait(false); }
                            catch (Exception e)
                            {
                                Logger?.LogError(e, $"{GetType().Name}.{nameof(HandleUnsubscribe)}: Failed to cancel controller.  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                            }
                        });
                    }
                }
                catch (Exception e)
                {
                    Logger?.LogError(e, $"{GetType().Name}.{nameof(HandleUnsubscribe)}: Failed.  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                    throw;
                }
            }
        }

        #endregion Public Methods

        #region Protected Methods

        protected override void HandleSubscribe(Action subscribeAction)
        {
            lock (_sync)
            {
                try
                {
                    base.HandleSubscribe(subscribeAction);

                    // Begin streaming if there are subscribed streams and controller is not active.
                    if (!_controllerActive && Stream.ProvidedStreams.Any())
                    {
                        Logger?.LogDebug($"{GetType().Name}.{nameof(HandleSubscribe)}: Begin streaming...  [thread: {Thread.CurrentThread.ManagedThreadId}]");

                        Watchdog.Kick();

                        _controllerActive = true;

                        // Add controller begin task continuation.
                        Task = Task.ContinueWith(_ =>
                        {
                            try { Controller.Begin(); }
                            catch (Exception e)
                            {
                                Logger?.LogError(e, $"{GetType().Name}.{nameof(HandleUnsubscribe)}: Failed to begin controller.  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                            }
                        });
                    }
                }
                catch (Exception e)
                {
                    Logger?.LogError(e, $"{GetType().Name}.{nameof(HandleSubscribe)}: Failed.  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                    throw;
                }
            }
        }

        protected override void HandleUnsubscribe(Action unsubscribeAction)
        {
            lock (_sync)
            {
                try
                {
                    base.HandleUnsubscribe(unsubscribeAction);

                    // Cancel streaming if there are no subscribed streams and controller is active.
                    if (_controllerActive && !Stream.ProvidedStreams.Any())
                    {
                        Logger?.LogDebug($"{GetType().Name}.{nameof(HandleUnsubscribe)}: Cancel streaming...  [thread: {Thread.CurrentThread.ManagedThreadId}]");

                        _controllerActive = false;

                        // Add controller cancel task continuation.
                        Task = Task.ContinueWith(async _ =>
                        {
                            try { await Controller.CancelAsync().ConfigureAwait(false); }
                            catch (Exception e)
                            {
                                Logger?.LogError(e, $"{GetType().Name}.{nameof(HandleUnsubscribe)}: Failed to cancel controller.  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                            }
                        });
                    }
                }
                catch (Exception e)
                {
                    Logger?.LogError(e, $"{GetType().Name}.{nameof(HandleUnsubscribe)}: Failed.  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                    throw;
                }
            }
        }

        #endregion Protected Methods

        #region IDisposable

        private bool _disposed;

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                Controller?.Dispose();
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
