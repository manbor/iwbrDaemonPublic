using MongoDB.Bson.Serialization.Attributes;

namespace IwbrDaemon.Types.Exchange
{
    [BsonIgnoreExtraElements]
    public class Order
    {
        [BsonElement("symbol")]
        public string Symbol { get; private set; }

        [BsonElement("orderId")]
        public long OrderId { get; private set; }

        [BsonElement("OrdersId")]
        public long OrdersId { get; private set; }

        [BsonElement("clientOrderId")]
        public string ClientOrderId { get; private set; }

        [BsonElement("price")]
        public decimal Price { get; private set; }

        [BsonElement("qrigQty")]
        public decimal OrigQty { get; private set; }

        [BsonElement("executedQty")]
        public decimal ExecutedQty { get; private set; }

        [BsonElement("cummulativeQuoteQty")]
        public decimal CummulativeQuoteQty { get; private set; }

        [BsonElement("status")]
        public string Status { get; private set; }

        [BsonElement("timeInForce")]
        public string TimeInForce { get; private set; }

        [BsonElement("type")]
        public string Type { get; private set; }

        [BsonElement("side")]
        public string Side { get; private set; }

        [BsonElement("stopPrice")]
        public decimal StopPrice { get; private set; }

        [BsonElement("icebergQty")]
        public decimal IcebergQty { get; private set; }

        [BsonElement("time")]
        public long Time { get; private set; }

        [BsonElement("updateTime")]
        public long UpdateTime { get; private set; }

        [BsonElement("isWorking")]
        public bool IsWorking { get; private set; }

        [BsonElement("workingTime")]
        public long WorkingTime { get; private set; }

        [BsonElement("origQuoteOrderQty")]
        public decimal OrigQuoteOrderQty { get; private set; }

        [BsonElement("selfTradePreventionMode")]
        public string SelfTradePreventionMode { get; private set; }

        [BsonElement("preventedMatchId")]
        public long PreventedMatchId { get; private set; }

        [BsonElement("preventedQuantity")]
        public decimal PreventedQuantity { get; private set; }

        public Order(string symbol, long orderId, long ordersId, string clientOrderId, decimal price, decimal origQty, decimal executedQty, decimal cummulativeQuoteQty, string status, string timeInForce, string type, string side, decimal stopPrice, decimal icebergQty, long time, long updateTime, bool isWorking, long workingTime, decimal origQuoteOrderQty, string selfTradePreventionMode, long preventedMatchId, decimal preventedQuantity)
        {
            Symbol = symbol;
            OrderId = orderId;
            OrdersId = ordersId;
            ClientOrderId = clientOrderId;
            Price = price;
            OrigQty = origQty;
            ExecutedQty = executedQty;
            CummulativeQuoteQty = cummulativeQuoteQty;
            Status = status;
            TimeInForce = timeInForce;
            Type = type;
            Side = side;
            StopPrice = stopPrice;
            IcebergQty = icebergQty;
            Time = time;
            UpdateTime = updateTime;
            IsWorking = isWorking;
            WorkingTime = workingTime;
            OrigQuoteOrderQty = origQuoteOrderQty;
            SelfTradePreventionMode = selfTradePreventionMode;
            PreventedMatchId = preventedMatchId;
            PreventedQuantity = preventedQuantity;
        }
    }
}
