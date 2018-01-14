using Binance.Market;

namespace Binance.Serialization
{
    public interface IOrderBookSerializer
    {
        /// <summary>
        /// Get the <see cref="OrderBookJsonConverter"/>.
        /// </summary>
        OrderBookJsonConverter JsonConverter { get; }

        /// <summary>
        /// Deserialize JSON to an <see cref="OrderBook"/>.
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        OrderBook Deserialize(string json);

        /// <summary>
        /// Serialize an <see cref="OrderBook"/> to JSON.
        /// </summary>
        /// <param name="orderBook"></param>
        /// <returns></returns>
        string Serialize(OrderBook orderBook);
    }
}
