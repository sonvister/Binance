using System;
using Newtonsoft.Json;

namespace Binance.Serialization
{
    public sealed class TimestampJsonConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(DateTime);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            return DateTimeOffset.FromUnixTimeMilliseconds(long.Parse(reader.Value.ToString())).UtcDateTime;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            writer.WriteValue(new DateTimeOffset((DateTime)value).ToUnixTimeMilliseconds());
        }
    }
}
