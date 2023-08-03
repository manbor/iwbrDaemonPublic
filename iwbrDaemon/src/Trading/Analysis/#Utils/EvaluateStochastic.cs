using Skender.Stock.Indicators;

namespace IwbrDaemon.Trading.Analysis
{
    public partial class Utils
    {
        public static eConditions EvaluateStochastic(StochResult stochResult ){

            if(stochResult.PercentJ <= 20){
                return eConditions.Oversold;
            } else if(stochResult.PercentJ >= 80){
                return eConditions.Overbought;
            } else {
                return eConditions.Neutral;
            }

        } 
    }
}