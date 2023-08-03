using MongoDB.Bson.Serialization.Attributes;
using Skender.Stock.Indicators;

namespace IwbrDaemon.Trading.Analysis
{
    public partial class IndicatorsBag
    {
        [BsonElement("stochastic")]
        public Dictionary<string, IEnumerable<StochResult>> Stochastics { get; private set; } = new Dictionary<string, IEnumerable<StochResult>>();

        private IEnumerable<StochResult> calc_Stochastic(List<Quote> inputQuotes) {
            var config = IwbrDaemon.Config.Analysis.Indicators.Stochastic;

            if (!config.IsEnabled)
                return null;

            return inputQuotes.GetStoch(
                                    config.LoopbackPeriods,
                                    config.SignalPeriods,
                                    config.SmoothPeriods,
                                    config.KFactor,
                                    config.DFactor,
                                    config.MaType
                                );
        }
    }
}


