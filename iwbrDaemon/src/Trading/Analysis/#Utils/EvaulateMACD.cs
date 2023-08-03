using IwbrDaemon.Trading.Analysis;
using Skender.Stock.Indicators;

namespace IwbrDaemon.Trading.Analysis
{
    public partial class Utils
    {
        public static eActions EvaluateMACD(List<MacdResult> macdResults){
            if(macdResults.Count() == 0){
                return eActions.Neutral;
            }

            //--------- buy or sell ---------------------------------------------------
            Dictionary<int, eActions> tmpCalc = new Dictionary<int, eActions>();

            for( int i = 1; i < macdResults.Count()-1; i++ ){
                var prev = macdResults[i-1];
                var act = macdResults[i];
                var next = macdResults[i+1];

                if( prev.Macd < act.Signal && act.Signal < next.Macd ) 
                    tmpCalc.Add(i, eActions.Buy);
                else if(prev.Macd > act.Signal && act.Signal > next.Macd) 
                    tmpCalc.Add(i, eActions.Sell);           
                ;
        
            }

            if(tmpCalc.Count() == 0){
                return eActions.Neutral;
            }            


            /*------------------------------------------------------------------------------
                calc the percent of buy/sell in function of the distance from the last moment
            --------------------------------------------------------------------------------*/
            int denomin = (tmpCalc.Count()/2) * (   tmpCalc.Keys.Min() + tmpCalc.Keys.Max() );

            if(denomin==0)
                return eActions.Neutral;

            var percents = new Dictionary<eActions, decimal>();

            percents.Add(eActions.Buy, ( tmpCalc.Where( p => p.Value == eActions.Buy ).Sum(p => p.Key) / denomin) * 100 );
            percents.Add(eActions.Sell,( tmpCalc.Where( p => p.Value == eActions.Sell).Sum(p => p.Key) / denomin) * 100 );

            return percents.OrderByDescending(p => p.Value).First().Key;

        } 
    }
}