using MongoDB.Bson.Serialization.Attributes;

namespace IwbrDaemon.Types.Config.Analysis.Indicators
{
    [BsonIgnoreExtraElements]
    public class BollingerBands
    {
        [BsonElement("isEnabled")]
        public bool IsEnabled { get; private set; }

        [BsonElement("period")]
        public int Period { get; private set; }

        [BsonElement("deviation")]
        public double Deviation { get; private set; }

        public BollingerBands(bool isEnabled, int period, double deviation)
        {
            IsEnabled = isEnabled;
            Period = period;
            Deviation = deviation;
        }
    }
}