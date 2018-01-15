using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Binance.Account.Orders;
using Binance.Api;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Binance.Serialization
{
    public class OrderSerializer : IOrderSerializer
    {
        private const string Key_Symbol = "symbol";
        private const string Key_OrderId = "orderId";
        private const string Key_ClientOrderId = "clientOrderId";
        private const string Key_Time = "time";
        private const string Key_Price = "price";
        private const string Key_OriginalQuantity = "origQty";
        private const string Key_ExecutedQuantity = "executedQty";
        private const string Key_Status = "status";
        private const string Key_TimeInForce = "timeInForce";
        private const string Key_Type = "type";
        private const string Key_Side = "side";
        private const string Key_StopPrice = "stopPrice";
        private const string Key_IcebergQuantity = "icebergQty";

        public virtual Order Deserialize(string json, Order order)
        {
            Throw.IfNullOrWhiteSpace(json, nameof(json));
            Throw.IfNull(order, nameof(order));

            return FillOrder(order, JObject.Parse(json));
        }

        public virtual Order Deserialize(string json, IBinanceApiUser user)
        {
            Throw.IfNullOrWhiteSpace(json, nameof(json));
            Throw.IfNull(user, nameof(user));

            return FillOrder(new Order(user), JObject.Parse(json));
        }

        public virtual IEnumerable<Order> DeserializeMany(string json, IBinanceApiUser user)
        {
            Throw.IfNullOrWhiteSpace(json, nameof(json));
            Throw.IfNull(user, nameof(user));

            return JArray.Parse(json)
                .Select(item => FillOrder(new Order(user), item))
                .ToArray();
        }

        public virtual string Serialize(Order order)
        {
            Throw.IfNull(order, nameof(order));

            var jObject = new JObject
            {
                new JProperty(Key_Symbol, order.Symbol),
                new JProperty(Key_OrderId, order.Id),
                new JProperty(Key_ClientOrderId, order.ClientOrderId),
                new JProperty(Key_Time, order.Timestamp),
                new JProperty(Key_Price, order.Price.ToString(CultureInfo.InvariantCulture)),
                new JProperty(Key_OriginalQuantity, order.OriginalQuantity.ToString(CultureInfo.InvariantCulture)),
                new JProperty(Key_ExecutedQuantity, order.ExecutedQuantity.ToString(CultureInfo.InvariantCulture)),
                new JProperty(Key_Status, order.Status.AsString()),
                new JProperty(Key_TimeInForce, order.TimeInForce.ToString().ToUpperInvariant()),
                new JProperty(Key_Type, order.Type.AsString()),
                new JProperty(Key_Side, order.Side.ToString().ToUpperInvariant()),
                new JProperty(Key_StopPrice, order.StopPrice.ToString(CultureInfo.InvariantCulture)),
                new JProperty(Key_IcebergQuantity, order.IcebergQuantity.ToString(CultureInfo.InvariantCulture))
            };

            return jObject.ToString(Formatting.None);
        }

        private static Order FillOrder(Order order, JToken jToken)
        {
            order.Symbol = jToken[Key_Symbol].Value<string>();
            order.Id = jToken[Key_OrderId].Value<long>();
            order.ClientOrderId = jToken[Key_ClientOrderId].Value<string>();
            order.Timestamp = (jToken[Key_Time] ?? jToken["transactTime"]).Value<long>();
            order.Price = jToken[Key_Price].Value<decimal>();
            order.OriginalQuantity = jToken[Key_OriginalQuantity].Value<decimal>();
            order.ExecutedQuantity = jToken[Key_ExecutedQuantity].Value<decimal>();
            order.Status = jToken[Key_Status].Value<string>().ConvertOrderStatus();
            order.TimeInForce = jToken[Key_TimeInForce].Value<string>().ConvertTimeInForce();
            order.Type = jToken[Key_Type].Value<string>().ConvertOrderType();
            order.Side = jToken[Key_Side].Value<string>().ConvertOrderSide();
            order.StopPrice = jToken[Key_StopPrice]?.Value<decimal>() ?? order.StopPrice;
            order.IcebergQuantity = jToken[Key_IcebergQuantity]?.Value<decimal>() ?? order.IcebergQuantity;

            return order;
        }
    }
}
