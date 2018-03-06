using System.Collections.Generic;

namespace Binance.Serialization
{
    public interface ITradeSerializer
    {
        /// <summary>
        /// Deserialize JSON to a <see cref="Trade"/>.
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        Trade Deserialize(string json);

        /// <summary>
        /// Deserialize JSON to multiple <see cref="Trade"/> instances.
        /// </summary>
        /// <param name="json"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        IEnumerable<Trade> DeserializeMany(string json, string symbol);

        /// <summary>
        /// Serialize a <see cref="Trade"/> to JSON.
        /// </summary>
        /// <param name="trade"></param>
        /// <returns></returns>
        string Serialize(Trade trade);
    }
}
