using MongoDB.Bson.Serialization.Attributes;
using Skender.Stock.Indicators;

namespace IwbrDaemon.Trading.Analysis
{
    public partial class IndicatorsBag
    {
        [BsonElement("sma")]
        public Dictionary<string, IEnumerable<SmaResult>> SMAs { get; private set; } = new Dictionary<string, IEnumerable<SmaResult>>();

        private IEnumerable<SmaResult> calc_SMA(List<Quote> inputQuotes) {
            var config = IwbrDaemon.Config.Analysis.Indicators.SMA;

            if (!config.IsEnabled)
                return null;

            return inputQuotes.GetSma(config.Period);

        }
    }
}


