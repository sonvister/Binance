using System.Collections.Generic;
using Binance.Market;

namespace Binance.Serialization
{
    public interface IAggregateTradeSerializer
    {
        /// <summary>
        /// Deserialize JSON to a <see cref="AggregateTrade"/>.
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        AggregateTrade Deserialize(string json);

        /// <summary>
        /// Deserialize JSON to multiple <see cref="AggregateTrade"/> instances.
        /// </summary>
        /// <param name="json"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        IEnumerable<AggregateTrade> DeserializeMany(string json, string symbol);

        /// <summary>
        /// Serialize a <see cref="AggregateTrade"/> to JSON.
        /// </summary>
        /// <param name="trade"></param>
        /// <returns></returns>
        string Serialize(AggregateTrade trade);
    }
}
