using System;
using System.Linq;
using Binance.Utility;

namespace Binance.WebSocket.Manager
{
    public sealed class JsonStreamController : RetryTaskController, IJsonStreamController
    {
        /// <summary>
        /// Get the JSON stream.
        /// </summary>
        public IJsonStream JsonStream { get; }

        /// <summary>
        /// Contstructor.
        /// </summary>
        /// <param name="jsonStream"></param>
        /// <param name="onError"></param>
        public JsonStreamController(IJsonStream jsonStream, Action<Exception> onError = null)
            : base(jsonStream.StreamAsync, onError)
        {
            Throw.IfNull(jsonStream, nameof(jsonStream));

            JsonStream = jsonStream;
        }

        /// <summary>
        /// Begin streaming after verifying JSON stream is not active and has
        /// at least one observer.
        /// </summary>
        public override void Begin()
        {
            if (!JsonStream.IsStreaming && JsonStream.Observers.Any())
            {
                base.Begin();
            }
        }
    }
}
