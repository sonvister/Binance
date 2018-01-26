using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Binance.Account;
using Binance.Account.Orders;
using Binance.Api;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Binance.Serialization
{
    public class OrderSerializer : IOrderSerializer
    {
        private const string KeySymbol = "symbol";
        private const string KeyOrderId = "orderId";
        private const string KeyClientOrderId = "clientOrderId";
        private const string KeyTime = "time";
        private const string KeyPrice = "price";
        private const string KeyOriginalQuantity = "origQty";
        private const string KeyExecutedQuantity = "executedQty";
        private const string KeyStatus = "status";
        private const string KeyTimeInForce = "timeInForce";
        private const string KeyType = "type";
        private const string KeySide = "side";
        private const string KeyStopPrice = "stopPrice";
        private const string KeyIcebergQuantity = "icebergQty";
        private const string KeyIsWorking = "isWorking";
        private const string KeyFills = "fills";
        private const string KeyFillPrice = "price";
        private const string KeyFillQuantity = "qty";
        private const string KeyFillCommission = "commission";
        private const string KeyFillCommissionAsset = "commissionAsset";
        private const string KeyFillTradeId = "tradeId";

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
                new JProperty(KeySymbol, order.Symbol),
                new JProperty(KeyOrderId, order.Id),
                new JProperty(KeyClientOrderId, order.ClientOrderId),
                new JProperty(KeyTime, order.Time.ToTimestamp()),
                new JProperty(KeyPrice, order.Price.ToString(CultureInfo.InvariantCulture)),
                new JProperty(KeyOriginalQuantity, order.OriginalQuantity.ToString(CultureInfo.InvariantCulture)),
                new JProperty(KeyExecutedQuantity, order.ExecutedQuantity.ToString(CultureInfo.InvariantCulture)),
                new JProperty(KeyStatus, order.Status.AsString()),
                new JProperty(KeyTimeInForce, order.TimeInForce.ToString().ToUpperInvariant()),
                new JProperty(KeyType, order.Type.AsString()),
                new JProperty(KeySide, order.Side.ToString().ToUpperInvariant()),
                new JProperty(KeyStopPrice, order.StopPrice.ToString(CultureInfo.InvariantCulture)),
                new JProperty(KeyIcebergQuantity, order.IcebergQuantity.ToString(CultureInfo.InvariantCulture)),
                new JProperty(KeyIsWorking, order.IsWorking)
            };

            // ReSharper disable once InvertIf
            if (order.Fills.Any())
            {
                var jArray = new JArray();

                foreach (var fill in order.Fills)
                {
                    jArray.Add(new JObject
                    {
                        new JProperty(KeyFillPrice, fill.Price.ToString(CultureInfo.InvariantCulture)),
                        new JProperty(KeyFillQuantity, fill.Quantity.ToString(CultureInfo.InvariantCulture)),
                        new JProperty(KeyFillCommission, fill.Commission.ToString(CultureInfo.InvariantCulture)),
                        new JProperty(KeyFillCommissionAsset, fill.CommissionAsset),
                        new JProperty(KeyFillTradeId, fill.TradeId)
                    });
                }

                jObject.Add(new JProperty(KeyFills, jArray));
            }

            return jObject.ToString(Formatting.None);
        }

        private static Order FillOrder(Order order, JToken jToken)
        {
            order.Symbol = jToken[KeySymbol].Value<string>();
            order.Id = jToken[KeyOrderId].Value<long>();
            order.ClientOrderId = jToken[KeyClientOrderId].Value<string>();
            order.Time = (jToken[KeyTime] ?? jToken["transactTime"]).Value<long>().ToDateTime();
            order.Price = jToken[KeyPrice].Value<decimal>();
            order.OriginalQuantity = jToken[KeyOriginalQuantity].Value<decimal>();
            order.ExecutedQuantity = jToken[KeyExecutedQuantity].Value<decimal>();
            order.Status = jToken[KeyStatus].Value<string>().ConvertOrderStatus();
            order.TimeInForce = jToken[KeyTimeInForce].Value<string>().ConvertTimeInForce();
            order.Type = jToken[KeyType].Value<string>().ConvertOrderType();
            order.Side = jToken[KeySide].Value<string>().ConvertOrderSide();
            order.StopPrice = jToken[KeyStopPrice]?.Value<decimal>() ?? order.StopPrice;
            order.IcebergQuantity = jToken[KeyIcebergQuantity]?.Value<decimal>() ?? order.IcebergQuantity;
            order.IsWorking = jToken[KeyIsWorking]?.Value<bool>() ?? order.IsWorking;

            var fills = jToken[KeyFills]?
                    .Select(entry => new Fill(
                        entry[KeyFillPrice].Value<decimal>(),
                        entry[KeyFillQuantity].Value<decimal>(),
                        entry[KeyFillCommission].Value<decimal>(),
                        entry[KeyFillCommissionAsset].Value<string>(),
                        entry[KeyFillTradeId].Value<long>()))
                    .ToArray();

            if (fills != null)
            {
                order.Fills = fills;
            }

            return order;
        }
    }
}
