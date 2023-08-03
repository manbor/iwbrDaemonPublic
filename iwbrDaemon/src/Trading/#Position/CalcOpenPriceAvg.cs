using IwbrDaemon.Trading.Analysis;

namespace IwbrDaemon.Trading
{

    public partial class Position
    {
        private void CalcOpenPriceAvg() {

            if(OpenResponse==null) {
                OpenPriceAvg = 0;
                return;
            }
            if(OpenResponse.Fills==null) {
                OpenPriceAvg = 0;
                return;
            }
            if(OpenResponse.Fills.Count()==0) {
                OpenPriceAvg = 0;
                return;
            }

            OpenPriceAvg =  OpenResponse.Fills.Select(p => p.Price * p.Quantity).Sum() / OpenResponse.Fills.Select(p => p.Quantity).Sum();

            InvestedQty = OpenResponse.Fills.Select(p => p.Quantity).Sum();

        }

    }
}
