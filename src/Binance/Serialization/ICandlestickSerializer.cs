using System.Collections.Generic;

namespace Binance.Serialization
{
    public interface ICandlestickSerializer
    {
        /// <summary>
        /// Deserialize JSON to a <see cref="Candlestick"/>.
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        Candlestick Deserialize(string json);

        /// <summary>
        /// Deserialize JSON to multiple <see cref="Candlestick"/> instances.
        /// </summary>
        /// <param name="json"></param>
        /// <param name="symbol"></param>
        /// <param name="interval"></param>
        /// <returns></returns>
        IEnumerable<Candlestick> DeserializeMany(string json, string symbol, CandlestickInterval interval);

        /// <summary>
        /// Serialize a <see cref="Candlestick"/> to JSON.
        /// </summary>
        /// <param name="candlestick"></param>
        /// <returns></returns>
        string Serialize(Candlestick candlestick);
    }
}
