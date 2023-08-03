using IwbrDaemon.Trading.Analysis;

namespace IwbrDaemon.Trading
{

    public partial class Position
    {
        private void CalcClosePriceAvg() {

            if(CloseResponse==null) {
                ClosePriceAvg = 0;
                return;
            }
            if(CloseResponse.Fills==null) {
                ClosePriceAvg = 0;
                return;
            }
            if(CloseResponse.Fills.Count()==0) {
                ClosePriceAvg = 0;
                return;
            }

            ClosePriceAvg =  CloseResponse.Fills.Select(p => p.Price * p.Quantity).Sum() / CloseResponse.Fills.Select(p => p.Quantity).Sum();

        }

    }
}
