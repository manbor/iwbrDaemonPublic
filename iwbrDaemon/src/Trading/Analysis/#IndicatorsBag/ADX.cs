using MongoDB.Bson.Serialization.Attributes;
using Skender.Stock.Indicators;

namespace IwbrDaemon.Trading.Analysis
{
    public partial class IndicatorsBag
    {
        [BsonElement("adx")]
        public Dictionary<string, IEnumerable<AdxResult>> ADXs { get; private set; } = new Dictionary<string, IEnumerable<AdxResult>>();

        private IEnumerable<AdxResult> calc_ADX(List<Quote> inputQuotes) {
            var config = IwbrDaemon.Config.Analysis.Indicators.ADX;

            if (!config.IsEnabled)
                return null; 

            return inputQuotes.GetAdx(config.Period);

        }
    }
}


