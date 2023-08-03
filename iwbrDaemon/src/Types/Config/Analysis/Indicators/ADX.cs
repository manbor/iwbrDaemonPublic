using MongoDB.Bson.Serialization.Attributes;

namespace IwbrDaemon.Types.Config.Analysis.Indicators
{
    [BsonIgnoreExtraElements]
    public class ADX
    {
        [BsonElement("isEnabled")]
        public bool IsEnabled { get; private set; }

        [BsonElement("period")]
        public int Period { get; private set; }

        [BsonElement("threshold")]
        public decimal Threshold { get; private set; }

        public ADX(bool isEnabled, int period, decimal threshold)
        {
            IsEnabled = isEnabled;
            Period = period;
            Threshold = threshold;
        }
    }
}