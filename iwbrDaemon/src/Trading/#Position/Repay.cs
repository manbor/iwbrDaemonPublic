using IwbrDaemon.Clients;

namespace IwbrDaemon.Trading
{
    public partial class Position
    {

        private bool Repay(){

            decimal toPay=0;

            //while(true) {

                if(Type == ePositionType.Long){
                    toPay =  OpenResponse.MarginBuyBorrowAmount;
                } else {
                    toPay = IwbrDb.GetUserAssets().Where(p => p.Asset == InvestedAsset).First().Borrowed;
                }
                
                if(toPay==0) {
                    PayTranId = 1;
                    IwbrDaemon.Clients.IwbrDb.MergePosition(this);
                    return true;
                }

                //--------------------------------------
                try {
                    PayTranId  = Binance.Repay(InvestedAsset,toPay);

                    if(PayTranId != 0){ 
                        IwbrDaemon.Clients.IwbrDb.MergePosition(this);
                        return true;
                    }

                }
                catch(Exception e){

                }
            //} 
            
            if(Type == ePositionType.Long){
                PayTranId = 1;          
                return true;
            }

            return false;
        }
    }
}