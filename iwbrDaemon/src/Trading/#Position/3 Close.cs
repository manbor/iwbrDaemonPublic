using IwbrDaemon.Types.Exchange;
using IwbrDaemon.Clients;
using static IwbrDaemon.Utils;

namespace IwbrDaemon.Trading
{
    public partial class Position
    {
        public void Close(string externalReasonOfClose = null){
            string logMsg = $"{LogBase()} [closing] RR:{RiskReducer.ToString("0.####").PadLeft(7).PadLeft(7)} ";

            if(!String.IsNullOrEmpty(externalReasonOfClose)) ReasonOfClose = externalReasonOfClose;

            lock(LockTrade){

                if(Symbol=="MASKBUSD"){
                    // FOR DEBUG
                }

                if (IwbrDaemon.Clients.IwbrDb.GetOpenPositions(Symbol).Count() == 0  ) {
                    return;
                }

                //---------------------------------------------------------------
                // create the closing order
                bool CloseRespDoesntKnow = false;
                if(CloseResponse==null) {

                    AssetBalance assetBalance;

                    try {
                        assetBalance = IwbrDb.GetUserAssets().Where(p => p.Asset ==  Symbol.Replace(Config.Binance.QuoteAsset,String.Empty) ).First();
                    }
                    catch(Exception e){
                        log.Error($"{logMsg} on getting netAsset.");
                        log.Error($"{logMsg} {e}");
                        return;
                    }

                    if(assetBalance.NetAsset==0) {
                        CloseRespDoesntKnow = true;
                    } else {

                        try{
                            decimal finalOrderQty;

                            if(Type==ePositionType.Long) {
                                finalOrderQty = Math.Max(0,RoundLotSize(assetBalance.NetAsset,ePositionAction.Close));
                            } 
                            else {
                                finalOrderQty =Math.Max(0, RoundLotSize((assetBalance.Borrowed - assetBalance.Free) *  1.01m ,ePositionAction.Close) );
                            }

                            if(finalOrderQty==0) {
                                CloseRespDoesntKnow = true;
                            }
                            else {
                                try {                                 
                                    CloseResponse = Binance.PlaceOrder(Symbol,  finalOrderQty , Type , ePositionAction.Close);                      
                                    CalcClosePriceAvg();    
                                    Profit = CalcProfit(ClosePriceAvg);

                                    IwbrDaemon.Clients.IwbrDb.MergePosition(this);
                                }
                                catch(Exception e){
                                    throw new Exception($"CloseResponse error");
                                }   
                            }                    
                        }
                        catch(Exception e){
                            log.Error($"{logMsg} {e}");
                            return;
                        }
                    }
                }

                //---------------------------------------------------------------
                // check the closing order
                if(CloseRespDoesntKnow){
                    //
                } else {
                    if(CloseOrder==null) {
                        var orderStatus = false;
                        try {
                            CloseOrder = GetOrder(CloseResponse.OrderId, out orderStatus);
                            IwbrDaemon.Clients.IwbrDb.MergePosition(this);
                        }   
                        catch(Exception e){ 
                            orderStatus = false;    
                        }

                        if(!orderStatus){
                            log.Error($"{logMsg} closing order failed");
                            return;
                        }
                    }
                }

                if(CloseRespDoesntKnow){
                    ProfitAsset=0;
                    ReasonOfClose += "NetAsset=0 and CloseResponse doesn't know.";
                }

                IwbrDaemon.Clients.IwbrDb.MergePosition(this);

                //---------------------------------------------------------------
                // loan repay        
                if(PayTranId==0) {
                    if(!Repay()) {
                        log.Error($"{logMsg} failed repay");
                        return;
                    }
                }

                LastUpdateTime = DateTime.UtcNow;

                IwbrDaemon.Clients.IwbrDb.MergePosition(this);

                IsChecking.TryRemove(Symbol, out _);

                log.Info($"{(Profit>0?LC.Green:LC.Red)}{logMsg} DONE - profit: {Profit.ToString("0.####")}% ({ProfitAsset.ToString("0.####")} {Config.Binance.QuoteAsset}) - {ReasonOfClose} {LC.OFF}");
                Console.Write('\a');

            }
        }
    }
}