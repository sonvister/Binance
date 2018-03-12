using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Binance.Client;
using Binance.Stream;
using Binance.Tests.WebSocket;
using Binance.WebSocket;
using Moq;
using Xunit;

namespace Binance.Tests.Stream
{
    public class JsonStreamPublisherTest
    {
        private MockJsonStreamPublisher _publisher;

        private IWebSocketStream _webSocketStream;

        private string _json = "{\"unit\":\"test\"}";
        private string _streamName = "<stream-name>";

        public JsonStreamPublisherTest()
        {
            _webSocketStream = new BinanceWebSocketStream(DefaultWebSocketClientTest.CreateMockWebSocketClient(_json, _streamName));

            _publisher = new MockJsonStreamPublisher(_webSocketStream);
        }

        [Fact]
        public void Properties()
        {
            Assert.Equal(_webSocketStream, _publisher.Stream);
            Assert.NotNull(_publisher.PublishedStreams);
            Assert.Empty(_publisher.PublishedStreams);
            Assert.Empty(_publisher.GetSubscribers());
        }

        #region Subscribe

        [Fact]
        public void SubscribeToStream()
        {
            const string streamName = "<stream-name>";

            // Subscribe to single stream name.
            _publisher.Subscribe(streamName);

            Assert.Single(_publisher.PublishedStreams);
            Assert.Contains(streamName, _publisher.PublishedStreams);
            Assert.Equal(1, _publisher.OnPublishedStreamsChangedCount);

            // Verify subscriber/stream name associations.
            Assert.Empty(_publisher.GetSubscribers());

            // Subscribe to same stream doesn't fail or change published streams.
            _publisher.Subscribe(streamName);

            Assert.Single(_publisher.PublishedStreams);
            Assert.Contains(streamName, _publisher.PublishedStreams);
            Assert.Equal(1, _publisher.OnPublishedStreamsChangedCount);

            // Verify subscriber/stream name associations.
            Assert.Empty(_publisher.GetSubscribers());

            // Verify if message was recieved.
            Assert.Null(_publisher.JsonMessageEventArgsReceived);
        }

        [Fact]
        public void SubscribeToMultipleStreams()
        {
            const string streamName1 = "<stream-name-1>";
            const string streamName2 = "<stream-name-2>";

            // Subscribe to multiple stream names.
            _publisher.Subscribe(streamName1, streamName2);

            Assert.Equal(2, _publisher.PublishedStreams.Count());
            Assert.Contains(streamName1, _publisher.PublishedStreams);
            Assert.Contains(streamName2, _publisher.PublishedStreams);
            Assert.Equal(1, _publisher.OnPublishedStreamsChangedCount);

            // Verify subscriber/stream name associations.
            Assert.Empty(_publisher.GetSubscribers());

            // Subscribe to same multiple stream names doesn't fail or change published streams.
            _publisher.Subscribe(streamName1, streamName2);

            Assert.Equal(2, _publisher.PublishedStreams.Count());
            Assert.Contains(streamName1, _publisher.PublishedStreams);
            Assert.Contains(streamName2, _publisher.PublishedStreams);
            Assert.Equal(1, _publisher.OnPublishedStreamsChangedCount);

            // Verify subscriber/stream name associations.
            Assert.Empty(_publisher.GetSubscribers());

            // Verify if message was recieved.
            Assert.Null(_publisher.JsonMessageEventArgsReceived);
        }

        [Fact]
        public void SubscribeToMultipleEnumerableStreams()
        {
            var streamNames = new List<string> { "<stream-name-1>", "<stream-name-2>" };

            // Subscribe to enumerable list of stream names.
            _publisher.Subscribe(streamNames);

            Assert.Equal(2, _publisher.PublishedStreams.Count());
            Assert.Contains(streamNames[0], _publisher.PublishedStreams);
            Assert.Contains(streamNames[1], _publisher.PublishedStreams);
            Assert.Equal(1, _publisher.OnPublishedStreamsChangedCount);

            // Verify subscriber/stream name associations.
            Assert.Empty(_publisher.GetSubscribers());

            // Subscribe to same enumerable list of stream names doesn't fail or change published streams.
            _publisher.Subscribe(streamNames);

            Assert.Equal(2, _publisher.PublishedStreams.Count());
            Assert.Contains(streamNames[0], _publisher.PublishedStreams);
            Assert.Contains(streamNames[1], _publisher.PublishedStreams);
            Assert.Equal(1, _publisher.OnPublishedStreamsChangedCount);

            // Verify subscriber/stream name associations.
            Assert.Empty(_publisher.GetSubscribers());

            // Verify if message was recieved.
            Assert.Null(_publisher.JsonMessageEventArgsReceived);
        }

        [Fact]
        public void SubscribeToMultipleStreamsInMultipleSteps()
        {
            const string streamName1 = "<stream-name-1>";
            const string streamName2 = "<stream-name-2>";

            // Subscribe to multiple stream names.
            _publisher.Subscribe(streamName1);
            _publisher.Subscribe(streamName2);

            Assert.Equal(2, _publisher.PublishedStreams.Count());
            Assert.Contains(streamName1, _publisher.PublishedStreams);
            Assert.Contains(streamName2, _publisher.PublishedStreams);
            Assert.Equal(2, _publisher.OnPublishedStreamsChangedCount);

            // Verify subscriber/stream name associations.
            Assert.Empty(_publisher.GetSubscribers());

            // Verify if message was recieved.
            Assert.Null(_publisher.JsonMessageEventArgsReceived);
        }

        [Fact]
        public void SubscribeSubscriberThrows()
        {
            var subscriber = new Mock<IJsonSubscriber>().Object;

            Assert.Throws<ArgumentException>(() => _publisher.Subscribe(subscriber));
        }

        [Fact]
        public void SubscribeSubscriberToStream()
        {
            const string streamName = "<stream-name>";
            var subscriber = new Mock<IJsonSubscriber>().Object;

            // Subscribe subscriber to single stream name.
            _publisher.Subscribe(subscriber, streamName);

            Assert.Single(_publisher.PublishedStreams);
            Assert.Contains(streamName, _publisher.PublishedStreams);
            Assert.Equal(1, _publisher.OnPublishedStreamsChangedCount);

            // Verify subscriber/stream name associations.
            Assert.Single(_publisher.GetSubscribers());
            Assert.Contains(subscriber, _publisher.GetSubscribers());
            Assert.Contains(subscriber, _publisher.GetSubscribers(streamName));

            // Subscribe same subscriber to same stream doesn't fail or change published streams.
            _publisher.Subscribe(subscriber, streamName);

            Assert.Single(_publisher.PublishedStreams);
            Assert.Contains(streamName, _publisher.PublishedStreams);
            Assert.Equal(1, _publisher.OnPublishedStreamsChangedCount);

            // Verify subscriber/stream name associations.
            Assert.Single(_publisher.GetSubscribers());
            Assert.Contains(subscriber, _publisher.GetSubscribers());
            Assert.Contains(subscriber, _publisher.GetSubscribers(streamName));

            // Verify if message was recieved.
            Assert.Null(_publisher.JsonMessageEventArgsReceived);
        }

        [Fact]
        public void SubscribeSubscriberToMultipleStreams()
        {
            const string streamName1 = "<stream-name-1>";
            const string streamName2 = "<stream-name-2>";
            var subscriber = new Mock<IJsonSubscriber>().Object;

            // Subscribe subscriber to multiple stream names.
            _publisher.Subscribe(subscriber, streamName1, streamName2);

            Assert.Equal(2, _publisher.PublishedStreams.Count());
            Assert.Contains(streamName1, _publisher.PublishedStreams);
            Assert.Contains(streamName2, _publisher.PublishedStreams);
            Assert.Equal(1, _publisher.OnPublishedStreamsChangedCount);

            // Verify subscriber/stream name associations.
            Assert.Equal(2, _publisher.GetSubscribers().Count());
            Assert.Contains(subscriber, _publisher.GetSubscribers());
            Assert.Contains(subscriber, _publisher.GetSubscribers(streamName1));
            Assert.Contains(subscriber, _publisher.GetSubscribers(streamName2));

            // Subscribe same subscriber to same stream names doesn't fail or change published streams.
            _publisher.Subscribe(subscriber, streamName1, streamName2);

            Assert.Equal(2, _publisher.PublishedStreams.Count());
            Assert.Contains(streamName1, _publisher.PublishedStreams);
            Assert.Contains(streamName2, _publisher.PublishedStreams);
            Assert.Equal(1, _publisher.OnPublishedStreamsChangedCount);

            // Verify subscriber/stream name associations.
            Assert.Equal(2, _publisher.GetSubscribers().Count());
            Assert.Contains(subscriber, _publisher.GetSubscribers());
            Assert.Contains(subscriber, _publisher.GetSubscribers(streamName1));
            Assert.Contains(subscriber, _publisher.GetSubscribers(streamName2));

            // Verify if message was recieved.
            Assert.Null(_publisher.JsonMessageEventArgsReceived);
        }

        [Fact]
        public void SubscribeSubscriberToMultipleEnumerableStreams()
        {
            var streamNames = new List<string> { "<stream-name-1>", "<stream-name-2>" };
            var subscriber = new Mock<IJsonSubscriber>().Object;

            // Subscribe subscriber to enumerable list of stream names.
            _publisher.Subscribe(subscriber, streamNames);

            Assert.Equal(2, _publisher.PublishedStreams.Count());
            Assert.Contains(streamNames[0], _publisher.PublishedStreams);
            Assert.Contains(streamNames[1], _publisher.PublishedStreams);
            Assert.Equal(1, _publisher.OnPublishedStreamsChangedCount);

            // Verify unique subscriber/stream name combinations.
            Assert.Equal(2, _publisher.GetSubscribers().Count());
            Assert.Contains(subscriber, _publisher.GetSubscribers());
            Assert.Contains(subscriber, _publisher.GetSubscribers(streamNames[0]));
            Assert.Contains(subscriber, _publisher.GetSubscribers(streamNames[1]));

            // Subscribe same subscriber to same stream names doesn't fail or change published streams.
            _publisher.Subscribe(subscriber, streamNames);

            Assert.Equal(2, _publisher.PublishedStreams.Count());
            Assert.Contains(streamNames[0], _publisher.PublishedStreams);
            Assert.Contains(streamNames[1], _publisher.PublishedStreams);
            Assert.Equal(1, _publisher.OnPublishedStreamsChangedCount);

            // Verify subscriber/stream name associations.
            Assert.Equal(2, _publisher.GetSubscribers().Count());
            Assert.Contains(subscriber, _publisher.GetSubscribers());
            Assert.Contains(subscriber, _publisher.GetSubscribers(streamNames[0]));
            Assert.Contains(subscriber, _publisher.GetSubscribers(streamNames[1]));

            // Verify if message was recieved.
            Assert.Null(_publisher.JsonMessageEventArgsReceived);
        }

        [Fact]
        public void SubscribeMultipleSubscribersToStream()
        {
            const string streamName = "<stream-name>";
            var subscriber1 = new Mock<IJsonSubscriber>().Object;
            var subscriber2 = new Mock<IJsonSubscriber>().Object;

            // Subscribe multiple subscribers to single stream name.
            _publisher.Subscribe(subscriber1, streamName);
            _publisher.Subscribe(subscriber2, streamName);

            Assert.Single(_publisher.PublishedStreams);
            Assert.Contains(streamName, _publisher.PublishedStreams);
            Assert.Equal(1, _publisher.OnPublishedStreamsChangedCount);

            // Verify subscriber/stream name associations.
            Assert.Equal(2, _publisher.GetSubscribers().Count());
            Assert.Contains(subscriber1, _publisher.GetSubscribers());
            Assert.Contains(subscriber2, _publisher.GetSubscribers());
            Assert.Contains(subscriber1, _publisher.GetSubscribers(streamName));
            Assert.Contains(subscriber2, _publisher.GetSubscribers(streamName));

            // Verify if message was recieved.
            Assert.Null(_publisher.JsonMessageEventArgsReceived);
        }

        [Fact]
        public void SubscribeMultipleSubscribersToMultipleStreams()
        {
            const string streamName1 = "<stream-name-1>";
            const string streamName2 = "<stream-name-2>";
            const string streamName3 = "<stream-name-3>";
            var subscriber1 = new Mock<IJsonSubscriber>().Object;
            var subscriber2 = new Mock<IJsonSubscriber>().Object;

            // Subscribe multiple subscribers to multiple stream names.
            _publisher.Subscribe(subscriber1, streamName1, streamName2);
            _publisher.Subscribe(subscriber2, streamName2, streamName3);

            Assert.Equal(3, _publisher.PublishedStreams.Count());
            Assert.Contains(streamName1, _publisher.PublishedStreams);
            Assert.Contains(streamName2, _publisher.PublishedStreams);
            Assert.Contains(streamName3, _publisher.PublishedStreams);
            Assert.Equal(2, _publisher.OnPublishedStreamsChangedCount);

            // Verify subscriber/stream name associations.
            Assert.Equal(4, _publisher.GetSubscribers().Count());
            Assert.Contains(subscriber1, _publisher.GetSubscribers());
            Assert.Contains(subscriber2, _publisher.GetSubscribers());
            Assert.Contains(subscriber1, _publisher.GetSubscribers(streamName1));
            Assert.Contains(subscriber1, _publisher.GetSubscribers(streamName2));
            Assert.Contains(subscriber2, _publisher.GetSubscribers(streamName2));
            Assert.Contains(subscriber2, _publisher.GetSubscribers(streamName3));

            // Verify if message was recieved.
            Assert.Null(_publisher.JsonMessageEventArgsReceived);
        }

        #endregion Subscribe

        #region Unsubscribe

        [Fact]
        public void UnsubscribeFromStream()
        {
            const string streamName = "<stream-name>";

            // Unsubscribe from single stream name doesn't fail before subscribe.
            _publisher.Unsubscribe(streamName);
            Assert.Equal(0, _publisher.OnPublishedStreamsChangedCount);

            // Unsubscribe from single stream name.
            _publisher.Subscribe(streamName);
            _publisher.Unsubscribe(streamName);

            Assert.Empty(_publisher.PublishedStreams);
            Assert.Equal(2, _publisher.OnPublishedStreamsChangedCount);

            // Verify subscriber/stream name associations.
            Assert.Empty(_publisher.GetSubscribers());

            // Unsubscribe from same stream doesn't fail or change published streams.
            _publisher.Unsubscribe(streamName);

            Assert.Empty(_publisher.PublishedStreams);
            Assert.Equal(2, _publisher.OnPublishedStreamsChangedCount);

            // Verify subscriber/stream name associations.
            Assert.Empty(_publisher.GetSubscribers());

            // Verify if message was recieved.
            Assert.Null(_publisher.JsonMessageEventArgsReceived);
        }

        [Fact]
        public void UnsubscribeFromMultipleStreams()
        {
            const string streamName1 = "<stream-name-1>";
            const string streamName2 = "<stream-name-2>";

            // Unsubscribe from multiple stream names doesn't fail before subscribe.
            _publisher.Unsubscribe(streamName1, streamName2);
            Assert.Equal(0, _publisher.OnPublishedStreamsChangedCount);

            // Unsubsribe from multiple stream names.
            _publisher.Subscribe(streamName1, streamName2);
            _publisher.Unsubscribe(streamName1, streamName2);

            Assert.Empty(_publisher.PublishedStreams);
            Assert.Equal(2, _publisher.OnPublishedStreamsChangedCount);

            // Verify subscriber/stream name associations.
            Assert.Empty(_publisher.GetSubscribers());

            // Unsubscribe from same stream names doesn't fail or change published streams.
            _publisher.Unsubscribe(streamName1, streamName2);

            Assert.Empty(_publisher.PublishedStreams);
            Assert.Equal(2, _publisher.OnPublishedStreamsChangedCount);

            // Verify subscriber/stream name associations.
            Assert.Empty(_publisher.GetSubscribers());

            // Verify if message was recieved.
            Assert.Null(_publisher.JsonMessageEventArgsReceived);
        }

        [Fact]
        public void UnsubscribeFromMultipleEnumerableStreams()
        {
            var streamNames = new List<string> { "<stream-name-1>", "<stream-name-2>" };

            // Unsubscribe from enumerable list of stream names doesn't fail before subscribe.
            _publisher.Unsubscribe(streamNames);
            Assert.Equal(0, _publisher.OnPublishedStreamsChangedCount);

            // Unsubsribe from enumerable list of stream names.
            _publisher.Subscribe(streamNames);
            _publisher.Unsubscribe(streamNames);

            Assert.Empty(_publisher.PublishedStreams);
            Assert.Equal(2, _publisher.OnPublishedStreamsChangedCount);

            // Verify subscriber/stream name associations.
            Assert.Empty(_publisher.GetSubscribers());

            // Unsubsribe from same enumerable list of stream names doesn't fail or change published streams.
            _publisher.Unsubscribe(streamNames);

            Assert.Empty(_publisher.PublishedStreams);
            Assert.Equal(2, _publisher.OnPublishedStreamsChangedCount);

            // Verify subscriber/stream name associations.
            Assert.Empty(_publisher.GetSubscribers());

            // Verify if message was recieved.
            Assert.Null(_publisher.JsonMessageEventArgsReceived);
        }

        [Fact]
        public void UnsubscribeFromMultipleStreamsInMultipleSteps()
        {
            const string streamName1 = "<stream-name-1>";
            const string streamName2 = "<stream-name-2>";

            // Unsubscribe from multiple stream names.
            _publisher.Subscribe(streamName1, streamName2);
            _publisher.Unsubscribe(streamName1);
            _publisher.Unsubscribe(streamName2);

            Assert.Empty(_publisher.PublishedStreams);
            Assert.Equal(3, _publisher.OnPublishedStreamsChangedCount);

            // Verify subscriber/stream name associations.
            Assert.Empty(_publisher.GetSubscribers());

            // Verify if message was recieved.
            Assert.Null(_publisher.JsonMessageEventArgsReceived);
        }

        [Fact]
        public void UnsubscribeSubscriberFromStream()
        {
            const string streamName = "<stream-name>";
            var subscriber = new Mock<IJsonSubscriber>().Object;

            // Unsubscribe subscriber from stream name doesn't fail before subscribe.
            _publisher.Unsubscribe(subscriber, streamName);
            Assert.Equal(0, _publisher.OnPublishedStreamsChangedCount);

            // Unsubscribe subscriber from stream name.
            _publisher.Subscribe(subscriber, streamName);
            _publisher.Unsubscribe(subscriber, streamName);

            Assert.Empty(_publisher.PublishedStreams);
            Assert.Equal(2, _publisher.OnPublishedStreamsChangedCount);

            // Verify subscriber/stream name associations.
            Assert.Empty(_publisher.GetSubscribers());

            // Unsubscribe same subscriber from stream name doesn't fail or change published streams.
            _publisher.Unsubscribe(subscriber, streamName);

            Assert.Empty(_publisher.PublishedStreams);
            Assert.Equal(2, _publisher.OnPublishedStreamsChangedCount);

            // Verify subscriber/stream name associations.
            Assert.Empty(_publisher.GetSubscribers());

            // Verify if message was recieved.
            Assert.Null(_publisher.JsonMessageEventArgsReceived);
        }

        [Fact]
        public void UnsubscribeSubscriberFromMultipleStreams()
        {
            const string streamName1 = "<stream-name-1>";
            const string streamName2 = "<stream-name-2>";
            var subscriber = new Mock<IJsonSubscriber>().Object;

            // Unsubscribe subscriber from multiple stream names doesn't fail before subscribe.
            _publisher.Unsubscribe(subscriber, streamName1, streamName2);
            Assert.Equal(0, _publisher.OnPublishedStreamsChangedCount);

            // Unsubscribe subscriber from multiple stream names.
            _publisher.Subscribe(subscriber, streamName1, streamName2);
            _publisher.Unsubscribe(subscriber, streamName1, streamName2);

            Assert.Empty(_publisher.PublishedStreams);
            Assert.Equal(2, _publisher.OnPublishedStreamsChangedCount);

            // Verify subscriber/stream name associations.
            Assert.Empty(_publisher.GetSubscribers());

            // Unsubscribe subscriber from same stream names doesn't fail or change published streams.
            _publisher.Unsubscribe(subscriber, streamName1, streamName2);

            Assert.Empty(_publisher.PublishedStreams);
            Assert.Equal(2, _publisher.OnPublishedStreamsChangedCount);

            // Verify subscriber/stream name associations.
            Assert.Empty(_publisher.GetSubscribers());

            // Verify if message was recieved.
            Assert.Null(_publisher.JsonMessageEventArgsReceived);
        }

        [Fact]
        public void UnubscribeSubscriberFromMultipleEnumerableStreams()
        {
            var streamNames = new List<string> { "<stream-name-1>", "<stream-name-2>" };
            var subscriber = new Mock<IJsonSubscriber>().Object;

            // Unsubscribe subscriber from enumerable list of stream names doesn't fail before subscribe.
            _publisher.Unsubscribe(subscriber, streamNames);
            Assert.Equal(0, _publisher.OnPublishedStreamsChangedCount);

            // Unsubscribe subscriber from enumerable list of stream names.
            _publisher.Subscribe(subscriber, streamNames);
            _publisher.Unsubscribe(subscriber, streamNames);

            Assert.Empty(_publisher.PublishedStreams);
            Assert.Equal(2, _publisher.OnPublishedStreamsChangedCount);

            // Verify subscriber/stream name associations.
            Assert.Empty(_publisher.GetSubscribers());

            // Unsubscribe subscriber from same stream names doesn't fail or change published streams.
            _publisher.Unsubscribe(subscriber, streamNames);

            Assert.Empty(_publisher.PublishedStreams);
            Assert.Equal(2, _publisher.OnPublishedStreamsChangedCount);

            // Verify subscriber/stream name associations.
            Assert.Empty(_publisher.GetSubscribers());

            // Verify if message was recieved.
            Assert.Null(_publisher.JsonMessageEventArgsReceived);
        }

        [Fact]
        public void UnsubscribeMultipleSubscribersFromStream()
        {
            const string streamName = "<stream-name>";
            var subscriber1 = new Mock<IJsonSubscriber>().Object;
            var subscriber2 = new Mock<IJsonSubscriber>().Object;

            // Unsubscribe subscriber from single stream names doesn't fail before subscribe.
            _publisher.Unsubscribe(subscriber1, streamName);
            Assert.Equal(0, _publisher.OnPublishedStreamsChangedCount);

            _publisher.Subscribe(subscriber1, streamName);
            _publisher.Subscribe(subscriber2, streamName);

            // Unsubscribe first subscriber from single stream name.
            _publisher.Unsubscribe(subscriber1, streamName);

            Assert.Single(_publisher.PublishedStreams);
            Assert.Contains(streamName, _publisher.PublishedStreams);
            Assert.Equal(1, _publisher.OnPublishedStreamsChangedCount);

            // Verify subscriber/stream name associations.
            Assert.Single(_publisher.GetSubscribers());
            Assert.Contains(subscriber2, _publisher.GetSubscribers());
            Assert.Contains(subscriber2, _publisher.GetSubscribers(streamName));

            // Unsubscribe second subscriber from single stream name.
            _publisher.Unsubscribe(subscriber2, streamName);

            Assert.Empty(_publisher.PublishedStreams);
            Assert.Equal(2, _publisher.OnPublishedStreamsChangedCount);

            // Verify subscriber/stream name associations.
            Assert.Empty(_publisher.GetSubscribers());

            // Verify if message was recieved.
            Assert.Null(_publisher.JsonMessageEventArgsReceived);
        }

        [Fact]
        public void UnsubscribeMultipleSubscribersFromMultipleStreams()
        {
            const string streamName1 = "<stream-name-1>";
            const string streamName2 = "<stream-name-2>";
            const string streamName3 = "<stream-name-3>";
            var subscriber1 = new Mock<IJsonSubscriber>().Object;
            var subscriber2 = new Mock<IJsonSubscriber>().Object;

            // Unsubscribe subscriber from multiple stream names doesn't fail before subscribe.
            _publisher.Unsubscribe(subscriber1);
            Assert.Equal(0, _publisher.OnPublishedStreamsChangedCount);

            _publisher.Subscribe(subscriber1, streamName1, streamName2);
            _publisher.Subscribe(subscriber2, streamName2, streamName3);

            // Unsubscribe first subscriber from multiple stream names.
            _publisher.Unsubscribe(subscriber1);

            Assert.Equal(2, _publisher.PublishedStreams.Count());
            Assert.Contains(streamName2, _publisher.PublishedStreams);
            Assert.Contains(streamName3, _publisher.PublishedStreams);
            Assert.Equal(3, _publisher.OnPublishedStreamsChangedCount);

            // Verify subscriber/stream name associations.
            Assert.Equal(2, _publisher.GetSubscribers().Count());
            Assert.Contains(subscriber2, _publisher.GetSubscribers());
            Assert.Contains(subscriber2, _publisher.GetSubscribers(streamName2));
            Assert.Contains(subscriber2, _publisher.GetSubscribers(streamName3));

            // Unsubscribe second subscriber from multiple stream names.
            _publisher.Unsubscribe(subscriber2);

            Assert.Empty(_publisher.PublishedStreams);
            Assert.Equal(4, _publisher.OnPublishedStreamsChangedCount);

            // Verify subscriber/stream name associations.
            Assert.Empty(_publisher.GetSubscribers());

            // Verify if message was recieved.
            Assert.Null(_publisher.JsonMessageEventArgsReceived);
        }

        [Fact]
        public void UnsubscribeAll()
        {
            const string streamName1 = "<stream-name-1>";
            const string streamName2 = "<stream-name-2>";
            const string streamName3 = "<stream-name-3>";
            var subscriber1 = new Mock<IJsonSubscriber>().Object;
            var subscriber2 = new Mock<IJsonSubscriber>().Object;

            // Unsubscribe all doesn't fail before subscribe.
            _publisher.Unsubscribe();
            Assert.Equal(0, _publisher.OnPublishedStreamsChangedCount);

            _publisher.Subscribe(subscriber1, streamName1, streamName2);
            _publisher.Subscribe(subscriber2, streamName2, streamName3);

            // Unsubscribe all subscribers from all stream names.
            _publisher.Unsubscribe();

            Assert.Empty(_publisher.PublishedStreams);
            Assert.Equal(3, _publisher.OnPublishedStreamsChangedCount);

            // Verify subscriber/stream name associations.
            Assert.Empty(_publisher.GetSubscribers());

            // Verify if message was recieved.
            Assert.Null(_publisher.JsonMessageEventArgsReceived);
        }

        #endregion Unsubscribe

        [Fact]
        public async Task MessageReceive()
        {
            var subscriber = new MockJsonSubscriber();

            // Subscribe subscriber to single stream name.
            _publisher.Subscribe(subscriber, _streamName);

            using (var cts = new CancellationTokenSource())
            {
                _publisher.Message += (s, e) => cts.Cancel();

                _webSocketStream.Uri = new Uri(BinanceWebSocketStream.BaseUri);

                await _webSocketStream.StreamAsync(cts.Token);
            }

            // Verify if message was recieved.
            Assert.NotNull(_publisher.JsonMessageEventArgsReceived);
            Assert.Equal(_streamName, subscriber.StreamName);
            Assert.Equal(_json, subscriber.Json);
        }

        #region Private Classes

        /// <summary>
        /// Mock JSON stream publisher implemenation for testing abstract class.
        /// </summary>
        private class MockJsonStreamPublisher : JsonStreamPublisher<IWebSocketStream>
        {
            public JsonMessageEventArgs JsonMessageEventArgsReceived { get; private set; }
            public int OnPublishedStreamsChangedCount { get; private set; }

            public MockJsonStreamPublisher(IWebSocketStream stream)
                : base(stream)
            { }

            public IEnumerable<IJsonSubscriber> GetSubscribers(string streamName = null)
            {
                if (streamName == null)
                    return Subscribers.Values.SelectMany(s => s);
                else
                    return Subscribers.ContainsKey(streamName) ? Subscribers[streamName] : new IJsonSubscriber[] { };
            }

            protected override void OnPublishedStreamsChanged()
            {
                OnPublishedStreamsChangedCount++;
            }

            protected override void HandleMessage(object sender, JsonMessageEventArgs args)
            {
                JsonMessageEventArgsReceived = args;
                base.HandleMessage(sender, args);
            }
        }

        /// <summary>
        /// Mock JSON subscriber implementation for testing message events.
        /// </summary>
        private class MockJsonSubscriber : IJsonSubscriber
        {
            public IEnumerable<string> SubscribedStreams => throw new NotImplementedException();

            public string StreamName { get; private set; }
            public string Json { get; private set; }

            public void HandleMessage(string streamName, string json)
            {
                StreamName = streamName;
                Json = json;
            }

            public IJsonSubscriber Unsubscribe()
            {
                throw new NotImplementedException();
            }
        }

        #endregion Private Classes
    }
}
