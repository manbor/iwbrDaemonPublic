using IwbrDaemon.Clients;

namespace IwbrDaemon.Trading
{

    public partial class Position
    {
        private void CalcOrigInvestQty() {

                const decimal minInvest = 12.5m;
                const int borrowX = 3;

                decimal calculatedInvestQty = 0;
                try { 
                    var maxBalance = IwbrDb.GetMaxBalance();

                    var maxBorrowable = maxBalance * borrowX;
                    var maxInvest = maxBorrowable * 2/3;

                    //var calculatedInvestQty0 = (maxInvest / Config.Position.MaxOpenPos)* 1.05m;
                    //calculatedInvestQty =  ((int) Math.Ceiling( calculatedInvestQty0*10 )) / 10m ;
                    
                    calculatedInvestQty = (maxInvest / Config.Position.MaxOpenPos)* 1.05m ;

                } catch{
                    calculatedInvestQty = 0;
                }

                OrigInvestedQty = Math.Max(minInvest, Math.Max(calculatedInvestQty, Config.Position.InvestQty));

        }

    }
}
