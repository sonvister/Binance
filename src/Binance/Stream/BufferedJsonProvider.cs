using System.Threading;
using System.Threading.Tasks;
using Binance.Utility;
using Microsoft.Extensions.Logging;

namespace Binance.Stream
{
    /// <summary>
    /// An abstract <see cref="IBufferedJsonProvider"/> implemenation.
    /// </summary>
    public abstract class BufferedJsonProvider<TProvider> : JsonProvider, IBufferedJsonProvider<TProvider>
        where TProvider : IJsonProvider
    {
        #region Public Properties

        public TProvider JsonProvider { get; }

        #endregion Public Properties

        #region Private Fields

        private SequentialBuffer<string> _buffer;

        #endregion Private Fields

        #region Constructors

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="jsonProvider">The JSON provider (required).</param>
        /// <param name="logger">The logger (optional).</param>
        protected BufferedJsonProvider(TProvider jsonProvider, ILogger<BufferedJsonProvider<TProvider>> logger = null)
            : base(logger)
        {
            Throw.IfNull(jsonProvider, nameof(jsonProvider));

            JsonProvider = jsonProvider;
        }

        #endregion Constructors

        #region Protected Methods

        protected abstract Task ProcessJsonAsync(string json, CancellationToken token = default);

        protected void InitalizeBuffer(CancellationToken token = default)
        {
            _buffer = new SequentialBuffer<string>(ProcessJsonAsync, token);
            JsonProvider.Message += OnClientMessage;
        }

        protected void FinalizeBuffer()
        {
            JsonProvider.Message -= OnClientMessage;
            _buffer.Complete();
        }

        #endregion Protected Methods

        #region Private Methods

        private void OnClientMessage(object sender, JsonMessageEventArgs e)
        {
            _buffer.Post(e.Json);
        }

        #endregion Private Methods
    }
}
