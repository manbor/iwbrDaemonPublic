using MongoDB.Bson.Serialization.Attributes;

namespace IwbrDaemon.Types.Config.Analysis.Indicators
{
    [BsonIgnoreExtraElements]
    public class MACD
    {
        [BsonElement("isEnabled")]
        public bool IsEnabled { get; private set; }

        [BsonElement("fastPeriod")]
        public int FastPeriod { get; private set; }

        [BsonElement("slowPeriod")]
        public int SlowPeriod { get; private set; }

        [BsonElement("signalPeriod")]
        public int SignalPeriod { get; private set; }

        public MACD(bool isEnabled, int fastPeriod, int slowPeriod, int signalPeriod)
        {
            IsEnabled = isEnabled;
            FastPeriod = fastPeriod; // 12
            SlowPeriod = slowPeriod; // 26
            SignalPeriod = signalPeriod; // 9
        }
    }
}