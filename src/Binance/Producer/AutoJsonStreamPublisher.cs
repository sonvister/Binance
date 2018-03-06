using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Binance.Utility;
using Microsoft.Extensions.Logging;

namespace Binance.Producer
{
    public abstract class AutoJsonStreamPublisher<TStream> : JsonStreamPublisher<TStream>, IAutoJsonStreamPublisher<TStream>
        where TStream : class, IJsonStream
    {
        #region Public Constants

        public static readonly bool AutoStreamingEnabledDefault = true;

        #endregion Public Constants

        #region Public Properties

        public bool IsAutoStreamingEnabled
        {
            get => _isAutoStreamingEnabled;
            set
            {
                if (value == _isAutoStreamingEnabled)
                    return;

                // Modify automatic streaming enabled flag.
                _isAutoStreamingEnabled = value;

                // Handle automomatic streaming state change.
                HandleAutomaticStreaming();
            }
        }

        public IJsonStreamController<TStream> Controller { get; }

        #endregion Public Properties

        #region Private Fields

        private Task _task = Task.CompletedTask;

        private readonly object _sync = new object();

        private bool _isAutoStreamingEnabled = AutoStreamingEnabledDefault;

        #endregion Private Fields

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="controller">The JSON stream controller (required).</param>
        /// <param name="logger">The logger (optional).</param>
        protected AutoJsonStreamPublisher(IJsonStreamController<TStream> controller, ILogger<AutoJsonStreamPublisher<TStream>> logger = null)
            : base(controller?.Stream, logger)
        {
            Throw.IfNull(controller, nameof(controller));

            Controller = controller;
        }

        #endregion Constructors

        #region Protected Methods

        /// <summary>
        /// Handle automatic streaming when published streams change.
        /// </summary>
        protected override void OnPublishedStreamsChanged()
        {
            if (!IsAutoStreamingEnabled)
                return;

            HandleAutomaticStreaming();
        }

        #endregion Protected Methods

        #region Private Methods

        private void HandleAutomaticStreaming()
        {
            lock (_sync)
            {
                if (_task.IsCompleted)
                {
                    Logger?.LogDebug($"{GetType().Name}.{nameof(HandleAutomaticStreaming)}: Delayed automatic stream control...  [thread: {Thread.CurrentThread.ManagedThreadId}]");

                    _task = Task.Delay(250).ContinueWith(async _ =>
                    {
                        try
                        {
                            if (IsAutoStreamingEnabled && !Stream.IsStreaming && PublishedStreams.Any())
                            {
                                Logger?.LogDebug($"{GetType().Name}.{nameof(HandleAutomaticStreaming)}: Begin...  [thread: {Thread.CurrentThread.ManagedThreadId}]");

                                Controller.Begin();
                            }
                            else if (!IsAutoStreamingEnabled || Stream.IsStreaming && !PublishedStreams.Any())
                            {
                                Logger?.LogDebug($"{GetType().Name}.{nameof(HandleAutomaticStreaming)}: Cancel...  [thread: {Thread.CurrentThread.ManagedThreadId}]");

                                await Controller.CancelAsync()
                                    .ConfigureAwait(false);
                            }
                        }
                        catch (Exception e)
                        {
                            Logger?.LogError(e, $"{GetType().Name}: Automatic stream control failed.  [thread: {Thread.CurrentThread.ManagedThreadId}]");
                        }
                    });
                }
            }
        }

        #endregion Private Methods
    }
}
