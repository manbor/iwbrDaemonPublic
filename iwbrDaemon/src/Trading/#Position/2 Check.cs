using System.Collections.Concurrent;
using IwbrDaemon.Types.Exchange;
using IwbrDaemon.Clients;
using IwbrDaemon.Trading.Analysis;
using static IwbrDaemon.Utils;

namespace IwbrDaemon.Trading
{
    public partial class Position
    {

        public static DateTime DtLastUpdateExecution {get; private set;}

        public static ConcurrentDictionary<String, Boolean> IsChecking {get; private set; }= new ConcurrentDictionary<string, bool>();

        public void Check(SymbolAnalysis checkSymbolAnalysis = null, bool forcePrintLog = false){
            string logMSg = $"{LogBase()} [{(forcePrintLog?LC.Yellow:LC.Sky)}checking{LC.OFF}]{(forcePrintLog?"*":String.Empty).PadRight(3)}";
            
            if(!IsChecking.ContainsKey(Symbol)){
                IsChecking.TryAdd(Symbol,true);                
            } else {

                if(IsChecking[Symbol]) 
                    return;

                IsChecking[Symbol] = true;
            }

            if(checkSymbolAnalysis != null )
                CheckSymbolAnalysis = checkSymbolAnalysis;

            var logMsgOrig = logMSg;
            var closePosition = false;
            
            if(Symbol=="GMTBUSD"){
                // FOR DEBUG
            }

            ReasonOfClose = null;

            if(!closePosition && CloseResponse!=null) {
                closePosition = true;
                ReasonOfClose = "Detected CloseResponse";
            }
            if( IwbrDb.GetActualBalance() <= Config.Position.MinWalletBalance) {
                closePosition = true;
                ReasonOfClose = $"ActualBalance <= {Config.Position.MinWalletBalance}"; 
            }
            if(!closePosition && Config.Position.CloseAll) {
                closePosition = true;
                ReasonOfClose = "CloseAll from config.json";
            }

            CalcRiskReducer();

            if(!closePosition) 
            {
                try {
                    var tradingPair = Binance.TradingPairs
                                .Where(p => p.Symbol == Symbol)
                                .First();      
                }
                catch(System.InvalidOperationException e){
                    log.Error($"{logMSg} tradingPair not found");
                    return;  
                }

                var ticker = IwbrDaemon.Clients.IwbrDb.GetTicker(Symbol);

                LastUpdateTime = DateTime.UtcNow;            
                ReasonOfClose = string.Empty;
                var prevProfit = Profit; 
                
                Profit = Math.Round(CalcProfit(ticker.Price),4);  
                IwbrDaemon.Clients.IwbrDb.MergePosition(this);

                ProfitIsChanged = prevProfit != Profit;
                var KLines = IwbrDaemon.Clients.IwbrDb.GetKLines(Symbol,"1m",CreationTime, DateTime.UtcNow);

                if(ProfitIsChanged || forcePrintLog ) {
                    string arrow = " ";

                    if(ProfitIsChanged)
                        arrow = (Profit>prevProfit?$"{LC.Green}↗{LC.OFF}":$"{LC.Red}↘{LC.OFF}");

                    logMSg+=$"{arrow} {(Profit>=0?LC.Green:LC.Red)}{Profit.ToString().PadLeft(8)}%{LC.OFF}".PadRight(10);
                }

                //-------------------------------------------------------------------------------------------------------
                //                               checking closing conditions
                //-------------------------------------------------------------------------------------------------------
                string tmpReasonOfClose=String.Empty;
                var elapsedSec = (int) (DateTime.UtcNow - CreationTime).TotalSeconds;
                var elapsedMin = (int) Math.Floor((double) elapsedSec/60);

                //------------------------------------------------------------------------------
                //  trend change
                /*
                if(!closePosition && 
                    CheckSymbolAnalysis != null && 
                    CheckSymbolAnalysis.PositionAction == ePositionAction.Open &&
                    Math.Abs(Profit) > 1
                    )
                {
                    if (
                        (Type == ePositionType.Long && CheckSymbolAnalysis.PositionType==ePositionType.Short) ||
                        (Type == ePositionType.Short && CheckSymbolAnalysis.PositionType==ePositionType.Long) 
                    ){
                        closePosition = true;
                        tmpReasonOfClose += "Detected analysis with opposite order";
                    }
                }
                */

                //------------------------------------------------------------------------------
                //Stop Loss
                if(!closePosition && Config.Position.SL.IsEnabled && Profit < (Config.Position.SL.Value * RiskReducer) )
                {
                    closePosition=true;
                    tmpReasonOfClose += $"{LC.Red}[StopLoss]{LC.OFF}";
                }

                //------------------------------------------------------------------------------
                // freezed trend
                //
                if(!closePosition && Config.Position.FC.IsEnabled) {
                    int cooldownMinutes = Config.Position.FC.Minutes;
                    decimal aceptableError =  Config.Position.FC.AcceptableError;

                    if(KLines.Count() >= cooldownMinutes){
                        var lastNklines = KLines.TakeLast(cooldownMinutes);
                        bool isFreezedTrend = true;

                        foreach(var kline in lastNklines){
                            var priceAvg = (kline.OpenPrice + kline.LowPrice + kline.HighPrice + kline.ClosePrice) /4 ;
                            var oldProfitAvg = CalcProfit(priceAvg);

                            if(  !(oldProfitAvg >(Profit - aceptableError) && oldProfitAvg < (Profit + aceptableError)) ){
                                isFreezedTrend = false;
                                break;
                            }    
                        }

                        if(isFreezedTrend) {
                            closePosition=true;
                            tmpReasonOfClose += $"{LC.Red}[Timeout for freezed trend for more than {cooldownMinutes} minutes]{LC.OFF}";
                        }
                    }
                }

                //------------------------------------------------------------------------------
                // losing cooldown 
                // check if actual negative trend has started more than X mins ago.

                if(Config.Position.LC.IsEnabled && Profit < Config.Position.LC.StartFrom )
                {
                    int klinesToAnalyzeNum = Math.Max(1,Math.Min(elapsedMin,Config.Position.LC.Minutes));

                    var reverseProtifHistory =
                            KLines
                            .TakeLast(klinesToAnalyzeNum)
                            .Select(p => new {    profit = CalcProfit(    (Type==ePositionType.Long?p.LowPrice:p.HighPrice)   ) 
                                                , delta = (DateTime.UtcNow - Utils.UnixTimeStampToDateTime(p.CloseTime)).TotalSeconds
                                            }     
                                    )
                            .Reverse()
                        ;

                    double lastNegativeProfitPeriod = 0;
                    bool negativeFounds = false;

                    foreach(var p in reverseProtifHistory){
                        if(p.profit < Config.Position.LC.StartFrom ) {
                            lastNegativeProfitPeriod= p.delta ;
                            negativeFounds=true;
                        }
                        else{ 
                            break;
                        }
                    }

                    if(negativeFounds ) {
                        var color = String.Empty;
                        if( lastNegativeProfitPeriod >= (Config.Position.LC.Minutes*60 * 90/100) ) {
                            color = LC.Red;
                        } else if( lastNegativeProfitPeriod >= (Config.Position.LC.Minutes*60 * 50/100) ) {
                            color = LC.Orange;
                        } else {
                            //
                        }

                        tmpReasonOfClose += $"{color}[LC {Utils.FmtSeconds(lastNegativeProfitPeriod)}/{Utils.FmtSeconds(Config.Position.LC.Minutes*60)}";
                    }

                    if(    !closePosition 
                        && negativeFounds
                        //&& (DateTime.UtcNow - CreationTime).TotalMinutes >= Config.Position.LC.Minutes
                        && Profit < Config.Position.LC.StartFrom 
                        && lastNegativeProfitPeriod >= ( Config.Position.LC.Minutes * 60))
                    {
                        //log.Info($"{Symbol} LC CLOSE");
                        closePosition=true;
                        tmpReasonOfClose += $" Reached";
                    } 

                    tmpReasonOfClose += $"]{LC.OFF}";

                }

                /*
                //strong trend change
                if( !closePosition 
                    && CheckSymbolAnalysis!=null 
                    && CheckSymbolAnalysis.PositionAction == ePositionAction.Open 
                    && (CheckSymbolAnalysis.Algorithm==eAnalysAlgorithm.SMA1h || CheckSymbolAnalysis.Algorithm==eAnalysAlgorithm.SMA2h )
                    && CheckSymbolAnalysis.PositionType!= this.Type
                    && Profit < -5m 
                )
                {
                    closePosition=true;
                    tmpReasonOfClose += $"{LC.Red}[Detected strong trend change]{LC.OFF}";
                }
                */

                //------------------------------------------------------------------------------
                // trailing stop loss
                if(Config.Position.TSL.IsEnabled){

                    decimal topProfit = 0;  

                    var maxSeconds = 60*60*1;   // TODO config
                    var durationReducer = 1- elapsedSec/maxSeconds;

                    var startProfit = Math.Min(Config.Position.TSL.StartProfit ,  Config.Position.TSL.StartProfit * RiskReducer * durationReducer);
                    var diffProfit =  Math.Min(Config.Position.TSL.DiffProfit  ,Config.Position.TSL.DiffProfit * Math.Min(1,RiskReducer) * durationReducer);
                    string tslString = String.Empty;


                    if(Profit> 0 ) tslString = $"{LC.Yellow}[TSL /{startProfit.ToString("#.0000")}]{LC.OFF}";

                    if(!closePosition && Profit >= Config.Position.TSL.MinProfit)
                    {
                        try {
                            KLine topKLine;

                            if(Type==ePositionType.Long){
                                    topKLine = KLines
                                                .OrderByDescending(  p =>  p.HighPrice  )
                                                .First();
                                    topProfit = CalcProfit(topKLine.HighPrice);
                            } else {
                                    topKLine = KLines
                                                .OrderBy(  p =>p.LowPrice )
                                                .First();
                                    topProfit = CalcProfit(topKLine.LowPrice);
                            }

                            if( topProfit >= startProfit)  
                            {

                                    tslString = $"{LC.Green}[TSL {topProfit.ToString("0.####")}/{diffProfit.ToString("0.####")}/{startProfit.ToString("0.####")}";

                                    if((topProfit- Profit) > diffProfit)
                                    {
                                        closePosition=true;
                                        tslString += $" Reached";
                                    } 
                                    
                                    tslString += $"]{LC.OFF}";
                                    
                            }
                        }
                        catch(Exception e){

                        }
                    }

                    tmpReasonOfClose += tslString;
                }

                //------------------------------------------------------------------------------
                //TakeProfit
                if(!closePosition && Config.Position.TP.IsEnabled && Profit >= Config.Position.TP.Value) 
                {
                    closePosition=true;
                    tmpReasonOfClose += $"{LC.Green}[TakeProfit]{LC.OFF}";
                }                

                ReasonOfClose = !string.IsNullOrEmpty(tmpReasonOfClose)?tmpReasonOfClose:null;
            }



            //-------------------------------------------------------------------------------------------------------
            //-------------------------------------------------------------------------------------------------------
            //-------------------------------------------------------------------------------------------------------

            if(closePosition){                
                Close();
            } else {
                IwbrDaemon.Clients.IwbrDb.MergePosition(this);
                DtLastUpdateExecution = DateTime.UtcNow;

                if(logMSg != logMsgOrig || forcePrintLog)
                    log.Info($"{logMSg}   RR: {RiskReducer.ToString("#0.0000").PadLeft(7)}  {ReasonOfClose}");            
            }

            IsChecking[Symbol] = false;

        }

    }
}
