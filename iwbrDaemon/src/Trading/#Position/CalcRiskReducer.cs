using IwbrDaemon.Clients;

namespace IwbrDaemon.Trading
{

    public partial class Position
    {
        private void CalcRiskReducer() {

            //var risk0 = (decimal) eAnaylysOrderResult.Base / (decimal) SymbolAnalysis.OrderResult;
            //RiskReducer =((int) Math.Floor(risk0 * 1000)) / 1000m  ;    

            RiskReducer  = (decimal) eAnaylysOrderResult.Base / (decimal) SymbolAnalysis.OrderResult;

            if(CheckSymbolAnalysis != null){
                var volat0 = SymbolAnalysis.Indicators.Volatility5m;
                var volatN = CheckSymbolAnalysis.Indicators.Volatility5m;

                //RiskReducer = RiskReducer *  Math.Min((volatN/volat0),1)       ;

                var percent = (((volatN/volat0 ) -1 )/2)+1;
                RiskReducer = RiskReducer *  Math.Min(percent,1) ;
                
                
            }

        }

    }
}
