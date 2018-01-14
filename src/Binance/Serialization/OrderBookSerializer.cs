using Binance.Market;
using Newtonsoft.Json;

namespace Binance.Serialization
{
    public class OrderBookSerializer : IOrderBookSerializer
    {
        private JsonSerializerSettings _settings;

        public OrderBookSerializer()
        {
            _settings = new JsonSerializerSettings();
            _settings.Converters.Add(new OrderBookJsonConverter());
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
