using System.Collections.Generic;

namespace Binance.Serialization
{
    public interface ISymbolStatisticsSerializer
    {
        /// <summary>
        /// Deserialize JSON to an <see cref="SymbolStatistics"/>.
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        SymbolStatistics Deserialize(string json);

        /// <summary>
        /// Deserialize JSON to multiple <see cref="SymbolStatistics"/> instances.
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        IEnumerable<SymbolStatistics> DeserializeMany(string json);

        /// <summary>
        /// Serialize an <see cref="SymbolStatistics"/> to JSON.
        /// </summary>
        /// <param name="statistics"></param>
        /// <returns></returns>
        string Serialize(SymbolStatistics statistics);
    }
}
