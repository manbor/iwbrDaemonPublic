using MongoDB.Bson.Serialization.Attributes;
using Skender.Stock.Indicators;

namespace IwbrDaemon.Trading.Analysis
{
    public partial class IndicatorsBag
    {
        [BsonElement("atrs")]
        public Dictionary<string, IEnumerable<AtrResult>> ATRs { get; private set; } = new Dictionary<string, IEnumerable<AtrResult>>();

        private IEnumerable<AtrResult> calc_ATR(List<Quote> inputQuotes) {
            //var config = IwbrDaemon.Config.Analysis.Indicators.ATR;

            return inputQuotes.GetAtr();
        }
    }
}


