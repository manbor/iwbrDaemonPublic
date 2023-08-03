using MongoDB.Bson.Serialization.Attributes;
using Skender.Stock.Indicators;

namespace IwbrDaemon.Trading.Analysis
{
    public partial class IndicatorsBag
    {
        [BsonElement("rsi")]
        public Dictionary<string, IEnumerable<RsiResult>> RSIs { get; private set; } = new Dictionary<string, IEnumerable<RsiResult>>();

        private IEnumerable<RsiResult> calc_RSI(List<Quote> inputQuotes) {
            var config = IwbrDaemon.Config.Analysis.Indicators.RSI;

            if (!config.IsEnabled)
                return null;

            return inputQuotes.GetRsi(config.Period);

        }
    }
}


