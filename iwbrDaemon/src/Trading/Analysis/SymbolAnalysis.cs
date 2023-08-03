using MongoDB.Bson.Serialization.Attributes;
using Skender.Stock.Indicators;
using System.Reflection;

namespace IwbrDaemon.Trading.Analysis
{
    [BsonIgnoreExtraElements]

    public partial class SymbolAnalysis{
        private static System.Type thisType = MethodBase.GetCurrentMethod().DeclaringType;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(thisType);

        [BsonElement("creationTime")]
        public DateTime CreationTime { get; private set; }

        [BsonElement("requestTime")]
        public DateTime RequestTime { get; private set; }

        [BsonElement("symbol")]
        public string Symbol {get; private set;}

        [BsonElement("position")]
        public Position Position { get; private set; }

        [BsonElement("config")]
        public IwbrDaemon.Config.AnalysisConfig Config { get; private set; } = IwbrDaemon.Config.Analysis;

        [BsonElement("intervals")]
        public string[] Intervals { get; private set; }   

        [BsonElement("indicators")]
        public IndicatorsBag Indicators { get; private set; }

        [BsonElement("positionAction")]
        public ePositionAction PositionAction { get; private set; } = ePositionAction.Nothing;

        [BsonElement("positionType")]
        public ePositionType PositionType { get; private set; }

        [BsonElement("orderResult")]
        public eAnaylysOrderResult OrderResult { get; private set; }

        [BsonElement("volatility")]
        public decimal Volatility { get; private set; }

        public eAnalysAlgorithm Algorithm  { get; private set; }

        public SymbolAnalysis(
            DateTime creationTime,
            DateTime requestTime,
            string symbol,
            Position position,
            Config.AnalysisConfig config,
            IndicatorsBag indicators,
            ePositionAction positionAction,
            ePositionType positionType,
            eAnaylysOrderResult orderResult,
            decimal volatility,
            eAnalysAlgorithm algorithm
        )
        {
            RequestTime = requestTime;
            Symbol = symbol;
            Position = position;
            CreationTime = creationTime;
            Config = config;
            Indicators = indicators;
            PositionType = positionType;
            PositionAction = positionAction;   
            OrderResult = orderResult;  
            Volatility = volatility;
            Algorithm = algorithm;
        }

        public SymbolAnalysis(string symbol, DateTime requestTime){
            CreationTime = DateTime.UtcNow;
            Symbol = symbol;
            RequestTime = requestTime;
            Algorithm = eAnalysAlgorithm.Null;

            Intervals = IwbrDaemon.Clients.Binance.KLineIntervals.Where(p => p != "1s").ToArray();
            Indicators = new IndicatorsBag(symbol,Intervals,requestTime);

            var priceAvg = Indicators.PriceAvg;

            var var60m = Indicators.VarAvg60m;
            var adx1m = Utils.EvaulateAdx(Indicators.ADXs["1m"].Last());
            var adx5m = Utils.EvaulateAdx(Indicators.ADXs["5m"].Last());
            var adx30m = Utils.EvaulateAdx(Indicators.ADXs["30m"].Last());
            var stoch1 = Utils.EvaluateStochastic(Indicators.Stochastics["5m"].Last());
            var macd1 = Utils.EvaluateMACD(Indicators.MACDs["5m"].TakeLast(30).ToList());

            Volatility = Indicators.Volatility30m;
            //VolatilityRange = (int) Math.Floor(Indicators.Volatility30m * 10m);

            //-------------------------------------------------------------------------------------------------------------------

            if(PositionAction == ePositionAction.Nothing) {
                int minToCheck = 30;
                decimal aceptableError =  0.02m;

                var lastIQs = Indicators.InputQuotes["1m"].TakeLast(minToCheck);
                bool isFreezedTrend = true;


                for(int i=1; i< lastIQs.Count()-1 ; i++) {

                    var quotePrec  = lastIQs.ElementAt(i-1);
                    var quoteAct = lastIQs.ElementAt(i);

                    var pricePrec = (quotePrec.Open + quotePrec.Close + quotePrec.High + quotePrec.Close) /4 ;
                    var priceAct = (quoteAct.Open + quoteAct.Close + quoteAct.High + quoteAct.Close) /4 ;

                    if(  priceAct < (pricePrec * (1- aceptableError)) || priceAct < (pricePrec * (1 + aceptableError)) ){
                        isFreezedTrend = false;
                        break;
                    }   
                }

                if(isFreezedTrend) {
                    PositionAction = ePositionAction.Skip;
                }

            }



            // full short/long using SMA
            Action<IEnumerable<SmaResult>> evaulateSMA = (lastSMAs) => {

                List<Boolean> upDown = new List<bool>();

                // true = up
                // false = down
                for(int i = 1; i < lastSMAs.Count();i++) {
                        upDown.Add(   lastSMAs.ElementAt(i-1).Sma < lastSMAs.ElementAt(i).Sma  ) ;
                }

                var check = upDown.GroupBy(value => value)
                            .Select(group => new {
                                                    Value = group.Key,
                                                    Percentage = (double)group.Count() / upDown.Count * 100
                                            })
                            .OrderByDescending(p => p.Percentage)
                            .First()
                            ;

                if(check.Percentage >= 80){
                    PositionAction = ePositionAction.Open;
                    PositionType = check.Value?ePositionType.Long:ePositionType.Short;
                }
            };

            if(PositionAction == ePositionAction.Nothing) {
                var lastSMAs =  Indicators.SMAs["5m"].TakeLast(20); // 1 hours
                evaulateSMA(lastSMAs);
                if(PositionAction != ePositionAction.Nothing) {
                    OrderResult = eAnaylysOrderResult.High;
                    Algorithm = eAnalysAlgorithm.SMA1h;
                }
            }

            /*
            if(PositionAction == ePositionAction.Nothing) {
                var lastSMAs =  Indicators.SMAs["5m"].TakeLast(40); // 2 hours
                evaulateSMA(lastSMAs);
                if(PositionAction != ePositionAction.Nothing) {
                    OrderResult = eAnaylysOrderResult.Medium;
                    Algorithm = eAnalysAlgorithm.SMA2h;
                }
            }
            */


            /*
            //cryptohopper long
            if(PositionAction == ePositionAction.Nothing &&
                adx30m >= eTrendStrenght.Strong &&
                adx5m >= eTrendStrenght.Strong &&
                stoch1 == eConditions.Oversold &&
                macd1 == eActions.Buy
            ) {
                PositionAction = ePositionAction.Open;
                PositionType = ePositionType.Long;
                OrderResult = eAnaylysOrderResult.Medium;
                Algorithm = eAnalysAlgorithm.CryptoHopper;

                if(Volatility >= 0.85m){
                    OrderResult = eAnaylysOrderResult.Low;
                    PositionType = PositionType==ePositionType.Long? ePositionType.Short : ePositionType.Long;    
                } 

            }

            //cryptohopper short
            if(PositionAction == ePositionAction.Nothing &&
                adx30m >= eTrendStrenght.Strong &&
                adx5m >= eTrendStrenght.Strong &&
                stoch1 == eConditions.Overbought &&
                macd1 == eActions.Sell
            ) {
                PositionAction = ePositionAction.Open;
                PositionType = ePositionType.Short;
                OrderResult = eAnaylysOrderResult.Medium;
                Algorithm = eAnalysAlgorithm.CryptoHopper;

                if(Volatility >= 0.85m){
                    OrderResult = eAnaylysOrderResult.Low;
                    PositionType = PositionType==ePositionType.Long? ePositionType.Short : ePositionType.Long;    
                } 
            
            }
            */


            IwbrDaemon.Clients.IwbrDb.MergeSymbolAnalysis(this);
        }

    }
}