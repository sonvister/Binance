using Binance.Market;
using Newtonsoft.Json;

namespace Binance.Serialization
{
    public class OrderBookSerializer : IOrderBookSerializer
    {
        public OrderBookJsonConverter JsonConverter { get; }

        private JsonSerializerSettings _settings;

        public OrderBookSerializer()
        {
            JsonConverter = new OrderBookJsonConverter();

            _settings = new JsonSerializerSettings();
            _settings.Converters.Add(JsonConverter);
        }

        public virtual OrderBook Deserialize(string json)
        {
            return JsonConvert.DeserializeObject<OrderBook>(json, _settings);
        }

        public virtual string Serialize(OrderBook orderBook)
        {
            return JsonConvert.SerializeObject(orderBook, _settings);
        }
    }
}
