using MongoDB.Bson.Serialization.Attributes;

namespace IwbrDaemon.Trading
{
    [BsonIgnoreExtraElements]
    public class PlaceOrderResponse
    {
        [BsonElement("symbol")]
        public string Symbol { get; private set; }

        [BsonElement("orderId")]
        public long OrderId { get; private set; }

        [BsonElement("clientOrderId")]
        public string ClientOrderId { get; private set; }

        [BsonElement("transactTime")]
        public long TransactTime { get; private set; }

        [BsonElement("price")]
        public decimal Price { get; private set; }

        [BsonElement("origQty")]
        public decimal OriginalQuantity { get; private set; }

        [BsonElement("executedQty")]
        public decimal ExecutedQuantity { get; private set; }

        [BsonElement("cummulativeQuoteQty")]
        public decimal CumulativeQuoteQuantity { get; private set; }

        [BsonElement("status")]
        public string Status { get; private set; }

        [BsonElement("timeInForce")]
        public string TimeInForce { get; private set; }

        [BsonElement("type")]
        public string Type { get; private set; }

        [BsonElement("side")]
        public string Side { get; private set; }

        [BsonElement("marginBuyBorrowAmount")]
        public decimal MarginBuyBorrowAmount { get; private set; }

        [BsonElement("marginBuyBorrowAsset")]
        public string MarginBuyBorrowAsset { get; private set; }

        [BsonElement("isIsolated")]
        public bool IsIsolated { get; private set; }

        [BsonElement("selfTradePreventionMode")]
        public string SelfTradePreventionMode { get; private set; }

        [BsonElement("fills")]
        public List<Fill> Fills { get; private set; }

        public PlaceOrderResponse(string symbol, long orderId, string clientOrderId, long transactTime, decimal price,
                    decimal origQty, decimal executedQty, decimal cummulativeQuoteQty, string status,
                    string timeInForce, string type, string side, decimal marginBuyBorrowAmount,
                    string marginBuyBorrowAsset, bool isIsolated, string selfTradePreventionMode,
                    List<Fill> fills)
        {
            Symbol = symbol;
            OrderId = orderId;
            ClientOrderId = clientOrderId;
            TransactTime = transactTime;
            Price = price;
            OriginalQuantity = origQty;
            ExecutedQuantity = executedQty;
            CumulativeQuoteQuantity = cummulativeQuoteQty;
            Status = status;
            TimeInForce = timeInForce;
            Type = type;
            Side = side;
            MarginBuyBorrowAmount = marginBuyBorrowAmount;
            MarginBuyBorrowAsset = marginBuyBorrowAsset;
            IsIsolated = isIsolated;
            SelfTradePreventionMode = selfTradePreventionMode;
            Fills = fills;
        }
    }


    [BsonIgnoreExtraElements]
    public class Fill
    {
        [BsonElement("price")]
        public decimal Price { get; private set; }

        [BsonElement("qty")]
        public decimal Quantity { get; private set; }

        [BsonElement("commission")]
        public decimal Commission { get; private set; }

        [BsonElement("commissionAsset")]
        public string CommissionAsset { get; private set; }

        public Fill(decimal price, decimal qty, decimal commission, string commissionAsset)
        {
            Price = price;
            Quantity = qty;
            Commission = commission;
            CommissionAsset = commissionAsset;
        }        
    }

}
