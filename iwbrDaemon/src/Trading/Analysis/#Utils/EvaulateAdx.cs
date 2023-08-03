using Skender.Stock.Indicators;

namespace IwbrDaemon.Trading.Analysis
{
    public partial class Utils
    {
        public static eTrendStrenght EvaulateAdx(AdxResult adxResult){
            
            if(adxResult.Adx <= 20) {
                return eTrendStrenght.NoStability;
            } else if(adxResult.Adx <= 50){
                return eTrendStrenght.Strong;
            } else if(adxResult.Adx <= 75){
                return eTrendStrenght.VeryStrong;
            } else {
                return eTrendStrenght.ExtremelyStrong;
            }
        } 
    }
}





