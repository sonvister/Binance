using System.Threading;
using Binance.Utility;
using Microsoft.Extensions.Logging;

namespace Binance.Stream
{
    /// <summary>
    /// An abstract <see cref="IBufferedJsonProducer{TProvider}"/> implemenation.
    /// </summary>
    public abstract class BufferedJsonProducer<TProducer> : JsonProducer, IBufferedJsonProducer<TProducer>
        where TProducer : IJsonProducer
    {
        #region Public Properties

        public TProducer JsonProducer { get; }

        #endregion Public Properties

        #region Private Fields

        private bool _isDataReceivedLogged;

        private QueuedProcessor<string> _buffer;

        #endregion Private Fields

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsonProvider">The JSON provider (required).</param>
        /// <param name="logger">The logger (optional).</param>
        protected BufferedJsonProducer(TProducer jsonProvider, ILogger<BufferedJsonProducer<TProducer>> logger = null)
            : base(logger)
        {
            Throw.IfNull(jsonProvider, nameof(jsonProvider));

            JsonProducer = jsonProvider;
        }

        #endregion Constructors

        #region Protected Methods

        protected abstract void ProcessJson(string json);

        protected void InitalizeBuffer(CancellationToken token = default)
        {
            _buffer = new QueuedProcessor<string>(ProcessJson);

            JsonProducer.Message += OnClientMessage;
        }

        protected void FinalizeBuffer()
        {
            JsonProducer.Message -= OnClientMessage;

            _buffer.Complete();

            _isDataReceivedLogged = false;
        }

        #endregion Protected Methods

        #region Private Methods

        private void OnClientMessage(object sender, JsonMessageEventArgs e)
        {
            if (!_isDataReceivedLogged)
            {
                Logger?.LogDebug($"{GetType().Name}: Buffering JSON data...  [thread: {Thread.CurrentThread.ManagedThreadId}]");

                _isDataReceivedLogged = true;
            }

            _buffer.Post(e.Json);
        }

        #endregion Private Methods
    }
}
