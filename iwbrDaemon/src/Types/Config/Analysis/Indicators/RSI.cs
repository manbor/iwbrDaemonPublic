using MongoDB.Bson.Serialization.Attributes;

namespace IwbrDaemon.Types.Config.Analysis.Indicators
{
    [BsonIgnoreExtraElements]
    public class RSI
    {
        [BsonElement("isEnabled")]
        public bool IsEnabled { get; private set; }

        [BsonElement("period")]
        public int Period { get; private set; }

        public RSI(bool isEnabled, int period)
        {
            IsEnabled = isEnabled;
            Period = period;
        }
    }
}