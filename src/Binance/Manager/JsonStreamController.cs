using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Binance.Stream;
using Binance.Utility;

namespace Binance.Manager
{
    /// <summary>
    /// The default <see cref="IJsonStreamController"/> implementation.
    /// </summary>
    public sealed class JsonStreamController : JsonStreamController<IJsonStream>, IJsonStreamController
    {
        #region Constructors

        /// <summary>
        /// Contstructor.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="onError"></param>
        public JsonStreamController(IJsonStream stream, Action<Exception> onError = null)
            : base(stream, onError)
        { }

        #endregion Constructors
    }

    public class JsonStreamController<TStream> : RetryTaskController, IJsonStreamController<TStream>
        where TStream : IJsonStream
    {
        #region Public Properties

        public TStream Stream { get; }

        #endregion Public Properties

        #region Private Fields

        private Task _task = Task.CompletedTask;

        private readonly object _sync = new object();

        #endregion Private Fields

        #region Constructors

        /// <summary>
        /// Contstructor.
        /// </summary>
        /// <param name="stream"></param>
        /// <param name="onError"></param>
        public JsonStreamController(TStream stream, Action<Exception> onError = null)
            : base(stream.StreamAsync, onError)
        {
            Throw.IfNull(stream, nameof(stream));

            Stream = stream;

            Stream.ProvidedStreamsChanged += OnProvidedStreamsChanged;
        }

        #endregion Constructors

        #region Public Methods

        /// <summary>
        /// Begin streaming after verifying stream is not active
        /// and has at least one provided stream (and subscriber).
        /// </summary>
        public override void Begin(Func<CancellationToken, Task> action = null, Action<Exception> onError = null)
        {
            if (!Stream.IsStreaming && Stream.ProvidedStreams.Any())
            {
                base.Begin(action, onError);
            }
        }

        #endregion Public Methods

        #region Private Methods

        /// <summary>
        /// Automatically start/stop streaming based on provided streams.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void OnProvidedStreamsChanged(object sender, EventArgs args)
        {
            lock (_sync)
            {
                if (_task.IsCompleted)
                {
                    _task = Task.Delay(250).ContinueWith(async _ =>
                    {
                        try
                        {
                            if (!Stream.IsStreaming && Stream.ProvidedStreams.Any())
                            {
                                base.Begin();
                            }
                            else if (Stream.IsStreaming && !Stream.ProvidedStreams.Any())
                            {
                                await CancelAsync()
                                    .ConfigureAwait(false);
                            }
                        }
                        catch { /* ignored */ }
                    });
                }
            }
        }

        #endregion Private Methods

        #region IDisposable

        private bool _disposed;

        protected override void Dispose(bool disposing)
        {
            if (_disposed)
                return;

            if (disposing)
            {
                Stream.ProvidedStreamsChanged -= OnProvidedStreamsChanged;
            }

            _disposed = true;

            base.Dispose(disposing);
        }

        #endregion IDisposable
    }
}
