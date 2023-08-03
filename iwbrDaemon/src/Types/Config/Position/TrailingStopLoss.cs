using MongoDB.Bson.Serialization.Attributes;

namespace IwbrDaemon.Types.Config.Position
{
    [BsonIgnoreExtraElements]
    public class TrailingStopLoss
    {
        [BsonElement("isEnabled")]
        public bool IsEnabled { get; private set; }

        [BsonElement("startProfit")]
        public decimal StartProfit { get; private set; }

        [BsonElement("diffProfit")]
        public decimal DiffProfit { get; private set; }

        [BsonElement("minProfit")]
        public decimal MinProfit { get; private set; }

        public TrailingStopLoss(bool isEnabled, decimal startProfit, decimal diffProfit, decimal minProfit)
        {
            IsEnabled = isEnabled;
            StartProfit = startProfit;
            DiffProfit = diffProfit;
            MinProfit = minProfit;
        }
    }
}