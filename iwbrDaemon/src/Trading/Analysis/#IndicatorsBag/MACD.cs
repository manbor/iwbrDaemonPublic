using MongoDB.Bson.Serialization.Attributes;
using Skender.Stock.Indicators;

namespace IwbrDaemon.Trading.Analysis
{
    public partial class IndicatorsBag
    {
        [BsonElement("macd")]
        public Dictionary<string, IEnumerable<MacdResult>> MACDs { get; private set; } = new Dictionary<string, IEnumerable<MacdResult>>();

        private IEnumerable<MacdResult> calc_MACD(List<Quote> inputQuotes) {
            var config = IwbrDaemon.Config.Analysis.Indicators.MACD;

            if (!config.IsEnabled)
                return null; 

            return inputQuotes.GetMacd(config.FastPeriod, config.SlowPeriod, config.SignalPeriod);

        }
    }
}


