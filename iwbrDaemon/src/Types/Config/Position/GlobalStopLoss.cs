using MongoDB.Bson.Serialization.Attributes;

namespace IwbrDaemon.Types.Config.Position
{
    [BsonIgnoreExtraElements]
    public class GlobalStopLoss
    {
        [BsonElement("isEnabled")]
        public bool IsEnabled { get; private set; }

        [BsonElement("value")]
        public decimal Value { get; private set; }

        public GlobalStopLoss(bool isEnabled, decimal value)
        {
            IsEnabled = isEnabled;
            Value = value;
        }
    }
}