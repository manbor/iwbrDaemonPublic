using MongoDB.Bson.Serialization.Attributes;
using Skender.Stock.Indicators;

namespace IwbrDaemon.Trading.Analysis
{
    public partial class IndicatorsBag
    {
        [BsonElement("priceAvg")]
        public Dictionary<string,decimal> PriceAvg {get; private set;}

        private decimal CalcPriceAvg(List<Quote> quotes ){
            
            List<decimal> avgsOpen = new List<decimal>();
            List<decimal> avgsLow = new List<decimal>();
            List<decimal> avgsHigh = new List<decimal>();
            List<decimal> avgsClose = new List<decimal>();

            decimal last =  quotes.Last().Close;

            for(int i = 0; i < quotes.Count()-1; i++){
                avgsOpen.Add ( last /  quotes[i].Open - 1  );
                avgsLow.Add  ( last /  quotes[i].Low - 1   );
                avgsHigh.Add ( last /  quotes[i].High - 1  );
                avgsClose.Add( last /  quotes[i].Close - 1 );
            }

            var avgOpen = avgsOpen.Sum() / avgsLow.Count();
            var avgLow = avgsLow.Sum() / avgsLow.Count();
            var avgHigh = avgsHigh.Sum() / avgsHigh.Count();
            var avgClose = avgsClose.Sum() / avgsLow.Count();

            var varAvg = (avgOpen+avgLow+avgHigh+avgClose)/4;



            return Math.Round(  last/(varAvg+1)  ,4) ;

        }
    }
}


