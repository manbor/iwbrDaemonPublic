using System.Collections.Concurrent;
using IwbrDaemon.Clients;
using Newtonsoft.Json.Linq;
using static IwbrDaemon.Utils;

namespace IwbrDaemon.Trading
{
    public partial class Position
    {

        //private static ConcurrentDictionary<string,DateTime> LastOpenIntent = new ConcurrentDictionary<string, DateTime>();

        private const decimal coeffSeguridad = 1.25m;

        private void Open(){

            if(!Binance.UserAssetsIsLoaded) 
                return;

            var dtStart1 = DateTime.UtcNow;

            string logMsg = $"{LogBase()} [{LC.Green}opening{LC.OFF}] ";

            //---------------------------------------------------------------
            // check if there was an recent position closed with no profit
            {
                var secodsSinceLastClose = 60 * 5;  // TODO for realtime
                var recentPositions = IwbrDb.GetRecentClosedPositionsNoProfit(Symbol, secodsSinceLastClose); 

                if(recentPositions.Count() > 0 ) {
                    if(Config.DebugEnable) log.Debug($"{logMsg} Recent close with no profit");
                    return;
                }
            }
            
            //---------------------------------------------------------------
            var tradingPair = IwbrDaemon.Clients.Binance.TradingPairs
                                .Where(p => p.Symbol == Symbol)
                                .First();
    
            try {
                var userAsset = IwbrDb.GetUserAssets()
                                .Where( p => p.Asset == tradingPair.BaseAsset)
                                .First();
            } 
            catch( System.InvalidOperationException e) {
                if(Config.DebugEnable) log.Debug ($"{logMsg} userAsset not found");
                return;
            }

            var filters = Binance.TradingPairs
                        .Where(p => p.Symbol == Symbol)
                        .First()
                        .Filters;            

            var ticker = IwbrDaemon.Clients.IwbrDb.GetTicker(Symbol);

            //---------------------------------------------------------------
            InvestedAsset = Type==ePositionType.Long?tradingPair.QuoteAsset:tradingPair.BaseAsset;
            
            var lastCloseWithNoProfit = IwbrDb.GetRecentClosedPositionsNoProfit(Symbol, 180)
                                        .OrderByDescending(p => p.LastUpdateTime)
                                        .FirstOrDefault() ?? null;
            
            if(lastCloseWithNoProfit != null ) {
                if(lastCloseWithNoProfit.Type == this.Type){
                    Type = Type==ePositionType.Long?ePositionType.Short:ePositionType.Long;
                }
            }
            CalcOrigInvestQty();
            
            //---------------------------------------------------------------
            // check NOTIONAL 
            try {
                var notional = JArray.Parse(filters).Where(p => p["filterType"].ToString() == "NOTIONAL").First();

                if(notional != null ) {
                    var min = decimal.Parse(notional["minNotional"].ToString());
                    var max = decimal.Parse(notional["maxNotional"].ToString());

                    if(OrigInvestedQty < min || OrigInvestedQty > max){
                        log.Error($"{logMsg} flter NOTIONAL not passed (min {min},max {max}, requested {OrigInvestedQty})");
                        return;
                    }
                }                  
            } 
            catch(Exception e){

            }

            //---------------------------------------------------------------
            if(Type==ePositionType.Long) {
                InvestedQty = RoundLotSize(OrigInvestedQty, ePositionAction.Open);
            } else {
                InvestedQty = RoundLotSize((OrigInvestedQty / ticker.Price) * 1.01m, ePositionAction.Open);
            }
            InvestedQty = Math.Floor(InvestedQty * 100000000M) / 100000000M; // fo keep only 8 decimals

            //---------------------------------------------------------------
            /*
            if(!LastOpenIntent.ContainsKey(Symbol)) {
                LastOpenIntent.TryAdd(Symbol, DateTime.UtcNow);
            } else {
                DateTime lastTry;
                LastOpenIntent.TryGetValue(Symbol,out lastTry );

                if( (DateTime.UtcNow-lastTry).TotalSeconds <= 20) 
                    return;
                
            };
            */

            lock(LockTrade){

                //---------------------------------------------------------------
                if (( DateTime.UtcNow - dtStart1).TotalSeconds > 30)
                    return;

                //---------------------------------------------------------------
                if( Position.MaxPositionsReached() ) {
                    if(Config.DebugEnable) log.Debug($"{logMsg} MaxPositionsReached");
                    return;
                }

                if (IwbrDaemon.Clients.IwbrDb.GetOpenPositions(Symbol).Count() > 0  ) {
                    return;
                }

                //---------------------------------------------------------------
                // place order
                var placeOrderFailed = false;
                try{
                    OpenResponse = Binance.PlaceOrder(Symbol,InvestedQty, Type, ePositionAction.Open );

                    OrigInvestedQty = OpenResponse.Fills.Select(p => p.Price*p.Quantity).Sum();
                    
                    CalcOpenPriceAvg();
                    IwbrDaemon.Clients.IwbrDb.MergePosition(this);
                }
                catch(Exception e){ 
                    placeOrderFailed = true;    
                }

                if(placeOrderFailed){
                    if(Config.DebugEnable) log.Debug($"{logMsg} PlaceOrder failed");
                    return;
                }

                //---------------------------------------------------------------
                // get (and check) order
                var orderStatus = false;
                try {
                    OpenOrder = GetOrder(OpenResponse.OrderId, out orderStatus);
                    IwbrDaemon.Clients.IwbrDb.MergePosition(this);
                }   
                catch(Exception e){ 
                    orderStatus = false;    
                }

                if(!orderStatus){
                    if(Config.DebugEnable) log.Debug($"{logMsg} PlaceOrder failed");
                    return;
                }

                //---------------------------------------------------------------
                // 
                Profit = 0;
                LastUpdateTime = DateTime.UtcNow;
                IwbrDaemon.Clients.IwbrDb.MergePosition(this);

                log.Info($"{logMsg} DONE ({OrigInvestedQty} {tradingPair.QuoteAsset}) RR:{RiskReducer}");
                Console.Write('\a');

            }
        }
    }
}