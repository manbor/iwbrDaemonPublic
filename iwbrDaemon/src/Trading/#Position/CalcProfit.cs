using IwbrDaemon.Clients;

namespace IwbrDaemon.Trading
{
    public partial class Position
    {

        private decimal CalcProfit(decimal priceRif) {
            
            if(OpenPriceAvg==ClosePriceAvg){
                return 0;
            }

            ProfitAsset = OrigInvestedQty * (Profit/100) *  (decimal) ( 1 - 0.01 ); // 0.01 is composed by commission [0,1% . 2x (Open & Close) and loan interes . It is not exact import but only aproxim

            return Math.Round( (  priceRif/OpenPriceAvg -1) * 100 * (Type==ePositionType.Long?1:-1) , 6); 

        }

    }
}