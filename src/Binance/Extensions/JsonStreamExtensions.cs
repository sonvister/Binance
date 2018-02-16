using System;
using System.Linq;

// ReSharper disable once CheckNamespace
namespace Binance.Stream
{
    public static class JsonStreamExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsonStream"></param>
        /// <param name="observer"></param>
        public static void Subscribe(this IJsonStream jsonStream, IJsonStreamObserver observer)
        {
            Throw.IfNull(jsonStream, nameof(jsonStream));
            Throw.IfNull(observer, nameof(observer));

            jsonStream.Subscribe(observer, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsonStream"></param>
        /// <param name="observer"></param>
        public static void Unsubscribe(this IJsonStream jsonStream, IJsonStreamObserver observer)
        {
            Throw.IfNull(jsonStream, nameof(jsonStream));
            Throw.IfNull(observer, nameof(observer));

            jsonStream.Unsubscribe(observer, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsonStream"></param>
        /// <param name="streamNames"></param>
        public static void Subscribe(this IJsonStream jsonStream, params string[] streamNames)
        {
            Throw.IfNull(jsonStream, nameof(jsonStream));
            Throw.IfNull(streamNames, nameof(streamNames));

            if (!streamNames.Any())
                throw new ArgumentException($"{nameof(IJsonStream)}.{nameof(Subscribe)}(params string[]): At least one stream name must be specified.");

            jsonStream.Subscribe(null, streamNames);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="jsonStream"></param>
        /// <param name="streamNames"></param>
        public static void Unsubscribe(this IJsonStream jsonStream, params string[] streamNames)
        {
            Throw.IfNull(jsonStream, nameof(jsonStream));
            Throw.IfNull(streamNames, nameof(streamNames));

            if (!streamNames.Any())
                throw new ArgumentException($"{nameof(IJsonStream)}.{nameof(Unsubscribe)}(params string[]): At least one stream name must be specified.");

            jsonStream.Unsubscribe(null, streamNames);
        }
    }
}
