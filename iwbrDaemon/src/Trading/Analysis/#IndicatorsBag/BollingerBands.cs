using MongoDB.Bson.Serialization.Attributes;
using Skender.Stock.Indicators;

namespace IwbrDaemon.Trading.Analysis
{
    public partial class IndicatorsBag
    {

        [BsonElement("bollingerBand")]
        public Dictionary<string, IEnumerable<BollingerBandsResult>> BollingerBands { get; private set; } = new Dictionary<string, IEnumerable<BollingerBandsResult>>();
                
        private IEnumerable<BollingerBandsResult> calc_BollingerBands(List<Quote> inputQuotes) {
            var config = IwbrDaemon.Config.Analysis.Indicators.BollingerBands;

            if (!config.IsEnabled)
                return null; 

            return inputQuotes.GetBollingerBands(config.Period, config.Deviation);

        }
    }
}


