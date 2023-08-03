using MongoDB.Bson.Serialization.Attributes;
using Skender.Stock.Indicators;

namespace IwbrDaemon.Trading.Analysis
{
    public partial class IndicatorsBag
    {
        [BsonElement("pivotPoints")]
        public Dictionary<string, IEnumerable<PivotPointsResult>> PivotPoints { get; private set; } = new Dictionary<string, IEnumerable<PivotPointsResult>>();

        private IEnumerable<PivotPointsResult> calc_PivotPoints(string interval, List<Quote> inputQuotes) {           

            Skender.Stock.Indicators.PeriodSize PeriodSize = Skender.Stock.Indicators.PeriodSize.OneHour;
            PivotPointType Type = PivotPointType.Standard;

            if (!IwbrDaemon.Config.Analysis.Indicators.PivotPoints.IsEnabled)
                return null;

            switch (interval)
            {
                case "1m": PeriodSize = Skender.Stock.Indicators.PeriodSize.OneMinute; return null;
                case "5m": PeriodSize = Skender.Stock.Indicators.PeriodSize.FiveMinutes; return null;
                case "30m": PeriodSize = Skender.Stock.Indicators.PeriodSize.ThirtyMinutes; return null;
                case "1h": PeriodSize = Skender.Stock.Indicators.PeriodSize.OneHour; break;
            }

            return inputQuotes.GetPivotPoints(PeriodSize, Type);

        }
    }
}


