using MongoDB.Bson.Serialization.Attributes;
using Skender.Stock.Indicators;
using IwbrDaemon.Clients;

namespace IwbrDaemon.Trading.Analysis
{
    [BsonIgnoreExtraElements]
    public partial class IndicatorsBag
    {
        [BsonElement("creationTime")]
        public DateTime CreationTime { get; private set; }

        [BsonElement("inputQuotes")]
        public Dictionary<string,List<Quote>> InputQuotes { get; private set; } = new Dictionary<string,List<Quote>>();
                
        [BsonElement("volatility5m")]
        public decimal Volatility5m {get; private set;}

        [BsonElement("volatility30m")]
        public decimal Volatility30m {get; private set;}

        [BsonElement("varAvg60m")]
        public decimal VarAvg60m {get; private set;}

        public IndicatorsBag(
            DateTime creationTime,
            Dictionary<string, List<Quote>> inputQuotes,
            Dictionary<string, IEnumerable<PivotPointsResult>> pivotPoints,
            Dictionary<string, IEnumerable<AdxResult>> aDXs,
            Dictionary<string, IEnumerable<BollingerBandsResult>> bollingerBands,
            Dictionary<string, IEnumerable<MacdResult>> mACDs,
            Dictionary<string, IEnumerable<RsiResult>> rSIs,
            Dictionary<string, IEnumerable<SmaResult>> sMAs,
            Dictionary<string, IEnumerable<StochResult>> stochastics,
            Dictionary<string, IEnumerable<AtrResult>> aTRs,
            Dictionary<string, decimal> priceAvg,
            decimal varAvg60m,
            decimal volatility5m,
            decimal volatility30m
        )
        {
            CreationTime = creationTime;
            InputQuotes = inputQuotes;
            PivotPoints = pivotPoints;
            ADXs = aDXs;
            BollingerBands = bollingerBands;
            MACDs = mACDs;
            RSIs = rSIs;
            SMAs = sMAs;
            Stochastics = stochastics;
            ATRs = aTRs;
            PriceAvg = priceAvg;
            VarAvg60m = varAvg60m;
            Volatility5m = volatility5m;
            Volatility30m = volatility30m;
        }


        public IndicatorsBag(string symbol, string[] intervals, DateTime requestTime) {
            CreationTime = DateTime.UtcNow;

            int klinesToFecth = 100;

            foreach (var interval in intervals)
            {

                List<Quote> inputQuotesInterval  = IwbrDb
                                                    .GetKLines(symbol, interval, klinesToFecth, requestTime)
                                                    .OrderBy(p => p.OpenTime)
                                                    .Select( p =>  new Quote
                                                            {
                                                                Date = DateTimeOffset.FromUnixTimeMilliseconds(p.OpenTime).UtcDateTime, 
                                                                Open = p.OpenPrice,
                                                                High = p.HighPrice,
                                                                Low = p.LowPrice,
                                                                Close = p.ClosePrice,
                                                                Volume = p.Volume
                                                            }
                                                    )
                                                    .ToList();

                InputQuotes.Add(interval,inputQuotesInterval);
                PivotPoints.Add(interval, calc_PivotPoints(interval, inputQuotesInterval));
                ADXs.Add(interval, calc_ADX(inputQuotesInterval));
                BollingerBands.Add(interval, calc_BollingerBands(inputQuotesInterval));
                MACDs.Add(interval, calc_MACD(inputQuotesInterval));
                RSIs.Add(interval, calc_RSI(inputQuotesInterval));
                SMAs.Add(interval, calc_SMA(inputQuotesInterval));
                Stochastics.Add(interval, calc_Stochastic(inputQuotesInterval));
                ATRs.Add(interval, calc_ATR(inputQuotesInterval));
            }

            PriceAvg = new Dictionary<string, decimal>();
            PriceAvg.Add("0m", InputQuotes["1m"].Last().Close);
            PriceAvg.Add("5m",CalcPriceAvg( InputQuotes["1m"].TakeLast(5).ToList() ));
            PriceAvg.Add("15m",CalcPriceAvg( InputQuotes["1m"].TakeLast(15).ToList() ));
            PriceAvg.Add("30m",CalcPriceAvg( InputQuotes["1m"].TakeLast(30).ToList() ));
            PriceAvg.Add("60m",CalcPriceAvg( InputQuotes["1m"].TakeLast(60).ToList() ));         

            try{
                VarAvg60m = Math.Round((( PriceAvg["0m"] / PriceAvg["60m"] ) -1) * 100,2);  
            }
            catch (System.DivideByZeroException e) {
                VarAvg60m = 0;
            }

            try{
                Volatility5m = (decimal) ATRs["5m"].Last().Atrp;
                Volatility30m = (decimal) ATRs["30m"].Last().Atrp;
            } 
            catch (Exception e) {
                Volatility5m = 100;
            }
        }
    }
}


