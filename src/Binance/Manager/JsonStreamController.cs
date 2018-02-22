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
    }
}
