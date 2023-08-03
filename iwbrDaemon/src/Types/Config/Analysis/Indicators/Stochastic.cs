using MongoDB.Bson.Serialization.Attributes;
using Skender.Stock.Indicators;

namespace IwbrDaemon.Types.Config.Analysis.Indicators
{
    [BsonIgnoreExtraElements]
    public class Stochastic
    {
        [BsonElement("isEnabled")]
        public bool IsEnabled { get; private set; }

        [BsonElement("lookbackPeriods")]
        public int LoopbackPeriods { get; private set; }

        [BsonElement("signalPeriods")]
        public int SignalPeriods { get; private set; }

        [BsonElement("smoothPeriods")]
        public int SmoothPeriods { get; private set; }

        [BsonElement("kFactor")]
        public double KFactor { get; private set; }

        [BsonElement("dFactor")]
        public double DFactor { get; private set; }

        [BsonElement("maType")]
        public MaType MaType { get; private set; }

        public Stochastic(bool isEnabled, int loopbackPeriods, int signalPeriods, int smoothPeriods, double kFactor, double dFactor)
        {
            IsEnabled = isEnabled;
            LoopbackPeriods = loopbackPeriods;
            SignalPeriods = signalPeriods;
            SmoothPeriods = smoothPeriods;
            KFactor = kFactor;
            DFactor = dFactor;
            MaType = MaType.SMA;
        }
    }
}