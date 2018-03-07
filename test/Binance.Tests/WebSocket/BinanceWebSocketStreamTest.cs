using System;
using System.Threading;
using System.Threading.Tasks;
using Binance.WebSocket;
using Xunit;

namespace Binance.Tests.WebSocket
{
    [Collection("Timing Sensitive Tests")]
    public class BinanceWebSocketStreamTest
    {
        private readonly Uri _uri;
        // ReSharper disable once InconsistentNaming
        private const string _message = "{}";
        // ReSharper disable once InconsistentNaming
        private const string _streamName = "test";
        private readonly BinanceWebSocketStream _stream;

        public BinanceWebSocketStreamTest()
        {
            _uri = BinanceWebSocketStream.CreateUri(_streamName);
            _stream = new BinanceWebSocketStream(DefaultWebSocketClientTest.CreateWebSocketClient(_message));
        }

        [Fact]
        public void Properties()
        {
            Assert.NotNull(_stream.WebSocket);
            Assert.Null(_stream.Uri);

            _stream.Uri = _uri;
            _stream.Uri = _uri; // can set to same URI.

            Assert.Equal(_uri, _stream.Uri);

            _stream.Uri = null; // can set to null.

            Assert.Null(_stream.Uri);
        }

        [Fact]
        public async Task IsStreaming()
        {
            _stream.Uri = _uri;

            using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1)))
            {
                Assert.False(_stream.IsStreaming);

                var task = _stream.StreamAsync(cts.Token);

                Assert.True(_stream.IsStreaming);

                await task;

                Assert.False(_stream.IsStreaming);
            }
        }

        [Fact]
        public void CreateUri()
        {
            var uri = BinanceWebSocketStream.CreateUri();

            Assert.Null(uri);

            uri = BinanceWebSocketStream.CreateUri(_streamName);

            Assert.Equal($"{BinanceWebSocketStream.BaseUri}/ws/{_streamName}", uri.AbsoluteUri);

            // Duplicates are ignored.
            var uri2 = BinanceWebSocketStream.CreateUri(_streamName, _streamName);
            Assert.Equal(uri, uri2);

            const string streamName = "combined";
            uri = BinanceWebSocketStream.CreateUri(_streamName, streamName);

            Assert.Equal($"{BinanceWebSocketStream.BaseUri}/stream?streams={_streamName}/{streamName}", uri.AbsoluteUri);
        }

        [Fact]
        public void IsCombined()
        {
            Assert.False(_stream.IsCombined);

            _stream.Uri = BinanceWebSocketStream.CreateUri(_streamName);

            Assert.False(_stream.IsCombined);

            _stream.Uri = BinanceWebSocketStream.CreateUri(_streamName, "combined");

            Assert.True(_stream.IsCombined);
        }

        [Fact]
        public async Task StreamThrows()
        {
            using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1)))
            {
                Assert.Null(_stream.Uri);

                await Assert.ThrowsAsync<InvalidOperationException>(() => _stream.StreamAsync(cts.Token));

                _stream.Uri = _uri;

                Assert.NotNull(_stream.Uri);

                var task = _stream.StreamAsync(cts.Token);

                await Assert.ThrowsAsync<InvalidOperationException>(() => _stream.StreamAsync(cts.Token));

                await task;
            }
        }

        [Fact]
        public async Task MessageEvent()
        {
            _stream.Uri = _uri;

            var isMessageEventReceived = false;

            using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1)))
            {
                _stream.Message += (s, e) =>
                {
                    isMessageEventReceived = e.Subject == _streamName && e.Json == _message;
                };

                Assert.False(isMessageEventReceived);

                await _stream.StreamAsync(cts.Token);

                Assert.True(isMessageEventReceived);
            }
        }

        [Fact]
        public async Task WainUntilOpen()
        {
            _stream.Uri = _uri;

            using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1)))
            {
                Assert.False(_stream.WebSocket.IsOpen);

                var task = _stream.StreamAsync(cts.Token);

                Assert.False(_stream.WebSocket.IsOpen);

                // Wait when web socket is not open.
                await _stream.WaitUntilWebSocketOpenAsync(cts.Token);

                Assert.True(_stream.WebSocket.IsOpen);

                // Wait when web socket is open.
                await _stream.WaitUntilWebSocketOpenAsync(cts.Token);

                Assert.False(cts.IsCancellationRequested);

                await task;

                Assert.True(cts.IsCancellationRequested);
            }
        }

        [Fact]
        public async Task HandleCombinedStreamData()
        {
            var message = $"{{\"stream\":\"{_streamName}\",\"data\":{_message}}}";

            var stream = new BinanceWebSocketStream(DefaultWebSocketClientTest.CreateWebSocketClient(message))
            {
                Uri = _uri // NOTE: Processing of combined stream data is not dependant upon URI.
            };

            var isMessageEventReceived = false;

            using (var cts = new CancellationTokenSource(TimeSpan.FromSeconds(1)))
            {
                stream.Message += (s, e) =>
                {
                    isMessageEventReceived = e.Subject == _streamName && e.Json == _message;
                };

                Assert.False(isMessageEventReceived);

                await stream.StreamAsync(cts.Token);

                Assert.True(isMessageEventReceived);
            }
        }
    }
}
