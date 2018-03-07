using System;
using System.Linq;
using Binance.Client;
using Moq;
using Xunit;

namespace Binance.Tests.Client
{
    public class UserDataClientTests
    {
        private readonly IUserDataClient _client;

        public UserDataClientTests()
        {
            _client = new UserDataClient();
        }

        [Fact]
        public void Throws()
        {
            var user = new Mock<IBinanceApiUser>().Object;

            Assert.Throws<ArgumentNullException>("listenKey", () => _client.Subscribe(null, user));
            Assert.Throws<ArgumentNullException>("listenKey", () => _client.Subscribe(string.Empty, user));

            Assert.Throws<ArgumentNullException>("listenKey", () => _client.Unsubscribe(null));
            Assert.Throws<ArgumentNullException>("listenKey", () => _client.Unsubscribe(string.Empty));
        }

        [Fact]
        public void Subscribe()
        {
            var listenKey1 = "<listen key 1>";
            var user1 = new Mock<IBinanceApiUser>().Object;

            var listenKey2 = "<listen key 2>";
            var user2 = new Mock<IBinanceApiUser>().Object;

            Assert.Empty(_client.SubscribedStreams);

            // Subscribe to listen key.
            _client.Subscribe(listenKey1, user1);
            Assert.True(_client.SubscribedStreams.Count() == 1);

            // Re-Subscribe to same listen key doesn't fail.
            _client.Subscribe(listenKey1, user1);
            Assert.True(_client.SubscribedStreams.Count() == 1);

            // Subscribe to a different listen key.
            _client.Subscribe(listenKey2, user2);
            Assert.True(_client.SubscribedStreams.Count() == 2);
        }

        [Fact]
        public void Unsubscribe()
        {
            var listenKey = "<listen key>";
            var user = new Mock<IBinanceApiUser>().Object;

            Assert.Empty(_client.SubscribedStreams);

            // Unsubscribe non-subscribed listen key doesn't fail.
            _client.Unsubscribe(listenKey);
            Assert.Empty(_client.SubscribedStreams);

            // Subscribe and unsubscribe listen key.
            _client.Subscribe(listenKey, user).Unsubscribe(listenKey);

            Assert.Empty(_client.SubscribedStreams);
        }

        [Fact]
        public void UnsubscribeAll()
        {
            var listenKey1 = "<listen key 1>";
            var user1 = new Mock<IBinanceApiUser>().Object;

            var listenKey2 = "<listen key 2>";
            var user2 = new Mock<IBinanceApiUser>().Object;

            // Unsubscribe all when not subscribed doesn't fail.
            _client.Unsubscribe();

            // Subscribe to multiple listen keys.
            _client.Subscribe(listenKey1, user1).Subscribe(listenKey2, user2);
            Assert.True(_client.SubscribedStreams.Count() == 2);

            // Unsubscribe all.
            _client.Unsubscribe();

            Assert.Empty(_client.SubscribedStreams);
        }
    }
}
