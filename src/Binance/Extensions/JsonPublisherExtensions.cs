using System;
using System.Collections.Generic;
using System.Linq;
using Binance.Client;

// ReSharper disable once CheckNamespace
namespace Binance.Producer
{
    public static class JsonPublisherExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="publisher"></param>
        /// <param name="streamNames"></param>
        public static void Subscribe(this IJsonPublisher publisher, IEnumerable<string> streamNames)
        {
            Throw.IfNull(streamNames, nameof(streamNames));

            Subscribe(publisher, streamNames.ToArray());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="publisher"></param>
        /// <param name="streamNames"></param>
        public static void Subscribe(this IJsonPublisher publisher, params string[] streamNames)
        {
            Throw.IfNull(publisher, nameof(publisher));
            Throw.IfNull(streamNames, nameof(streamNames));

            if (!streamNames.Any())
                throw new ArgumentException($"{nameof(IJsonPublisher)}.{nameof(Subscribe)}(params string[]): At least one stream name must be specified.");

            publisher.Subscribe(null, streamNames);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="publisher"></param>
        /// <param name="subscriber"></param>
        /// <param name="streamNames"></param>
        public static void Subscribe(this IJsonPublisher publisher, IJsonSubscriber subscriber, IEnumerable<string> streamNames)
        {
            Throw.IfNull(publisher, nameof(publisher));
            Throw.IfNull(streamNames, nameof(streamNames));

            publisher.Subscribe(subscriber, streamNames.ToArray());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="publisher"></param>
        /// <param name="subscriber"></param>
        public static void Unsubscribe(this IJsonPublisher publisher, IJsonSubscriber subscriber)
        {
            Throw.IfNull(publisher, nameof(publisher));

            publisher.Unsubscribe(subscriber, null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="publisher"></param>
        /// <param name="streamNames"></param>
        public static void Unsubscribe(this IJsonPublisher publisher, params string[] streamNames)
        {
            Throw.IfNull(publisher, nameof(publisher));

            publisher.Unsubscribe(null, streamNames);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="publisher"></param>
        /// <param name="streamNames"></param>
        public static void Unsubscribe(this IJsonPublisher publisher, IEnumerable<string> streamNames)
        {
            Throw.IfNull(streamNames, nameof(streamNames));

            Unsubscribe(publisher, streamNames.ToArray());
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="publisher"></param>
        /// <param name="subscriber"></param>
        /// <param name="streamNames"></param>
        public static void Unsubscribe(this IJsonPublisher publisher, IJsonSubscriber subscriber, IEnumerable<string> streamNames)
        {
            Throw.IfNull(publisher, nameof(publisher));
            Throw.IfNull(streamNames, nameof(streamNames));

            publisher.Unsubscribe(subscriber, streamNames.ToArray());
        }
    }
}
