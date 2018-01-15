using System.Collections.Generic;
using Binance.Market;

namespace Binance.Serialization
{
    public interface IOrderBookTopSerializer
    {
        /// <summary>
        /// Deserialize JSON to an <see cref="OrderBookTop"/>.
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        OrderBookTop Deserialize(string json);

        /// <summary>
        /// Deserialize JSON to multiple <see cref="OrderBookTop"/> instances.
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        IEnumerable<OrderBookTop> DeserializeMany(string json);

        /// <summary>
        /// Serialize an <see cref="OrderBookTop"/> to JSON.
        /// </summary>
        /// <param name="orderBookTop"></param>
        /// <returns></returns>
        string Serialize(OrderBookTop orderBookTop);
    }
}
