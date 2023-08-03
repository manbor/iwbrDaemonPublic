using MongoDB.Bson.Serialization.Attributes;
using System.Reflection;
using IwbrDaemon.Trading.Analysis;
using MongoDB.Bson;
using IwbrDaemon.Types.Exchange;

namespace IwbrDaemon.Trading
{

    [BsonIgnoreExtraElements]
    public partial class Position
    {
        private static System.Type thisType = MethodBase.GetCurrentMethod().DeclaringType;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(thisType);

        [BsonElement("creationTime")]
        public DateTime CreationTime { get; private set; }

        [BsonElement("lastUpdateTime")]
        public DateTime LastUpdateTime { get; private set; }        

        [BsonElement("symbol")]
        public string Symbol { get; private set; }

        [BsonElement("type")]
        public ePositionType Type {get; private set;}   

        [BsonElement("profit")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal Profit { get; private set; }    

        [BsonElement("reasonOfClose")]
        public string ReasonOfClose { get; private set; }  

        [BsonElement("origInvestedQty")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal OrigInvestedQty { get; private set; }  

        [BsonElement("investedQty")]
        public decimal InvestedQty { get; private set; } 
        
        [BsonElement("investedAsset")]
        public string InvestedAsset { get; private set; }  

        [BsonElement("openResponse")]
        public PlaceOrderResponse OpenResponse { get; private set; }

        [BsonElement("openPriceAvg")]
        public decimal OpenPriceAvg { get; private set; }    

        [BsonElement("openOrder")]
        public Order OpenOrder { get; private set; }

        [BsonElement("closeResponse")]
        public PlaceOrderResponse CloseResponse { get; private set; }

        [BsonElement("closePriceAvg")]
        public decimal ClosePriceAvg { get; private set; }  

        [BsonElement("closeOrder")]
        public Order CloseOrder { get; private set; }

        [BsonElement("payTranId")]
        public long PayTranId { get; private set; }

        [BsonElement("profitAsset")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal ProfitAsset { get; private set; }

        [BsonElement("symbolAnalysis")]
        public SymbolAnalysis SymbolAnalysis { get; private set; }

        [BsonElement("checkSymbolAnalysis")]
        public SymbolAnalysis CheckSymbolAnalysis { get; private set; }

        [BsonElement("riskReducer")]
        public decimal RiskReducer { get; private set; }

        private bool ProfitIsChanged;
        public static object LockTrade {get; private set;}= new object();

        private static bool isClosingAll = false;

        
        public Position(
            DateTime creationTime,
            DateTime lastUpdateTime,
            string symbol,
            ePositionType type,
            decimal profit,
            string reasonOfClose,
            decimal origInvestedQty,
            string investedAsset,
            decimal investedQty,
            PlaceOrderResponse openResponse,
            decimal openPriceAvg,
            Order openOrder,
            PlaceOrderResponse closeResponse,
            decimal closePriceAvg,
            Order closeOrder,
            long payTranId,
            decimal profitAsset,
            SymbolAnalysis symbolAnalysis,
            SymbolAnalysis checkSymbolAnalysis,
            decimal riskReducer
            )
        {
            CreationTime = creationTime;
            LastUpdateTime = lastUpdateTime;
            Symbol = symbol;
            Type = type;
            OpenPriceAvg = openPriceAvg;
            ClosePriceAvg = closePriceAvg;
            Profit = profit;
            ReasonOfClose = reasonOfClose;
            InvestedAsset = investedAsset;
            InvestedQty = investedQty;
            PayTranId = payTranId;
            OpenResponse = openResponse;
            OpenOrder = openOrder;
            CloseOrder = closeOrder;
            CloseResponse = closeResponse;
            ProfitAsset = profitAsset;
            OrigInvestedQty = origInvestedQty;
            SymbolAnalysis = symbolAnalysis;
            CheckSymbolAnalysis = checkSymbolAnalysis;
            RiskReducer = riskReducer;
        }

        public Position(SymbolAnalysis analysis){

            CreationTime = DateTime.UtcNow;
            LastUpdateTime = DateTime.UtcNow;
            SymbolAnalysis = analysis;
            Symbol = SymbolAnalysis.Symbol;
            Type = SymbolAnalysis.PositionType;      
            CalcRiskReducer();

            if(!Config.Position.CloseAll && !IsClosingAllForProfit) 
                Open();
        }

        public Position(string symbol, ePositionType type){

            CreationTime = DateTime.UtcNow;
            LastUpdateTime = DateTime.UtcNow;
            SymbolAnalysis = new SymbolAnalysis(Symbol, CreationTime);
            Symbol = symbol;
            Type = type;      
            CalcRiskReducer();

            if(!Config.Position.CloseAll && !IsClosingAllForProfit) 
                Open();
        }

        public bool IsOpen(){
            return PayTranId==null || PayTranId==0;
        }

        public bool IsClose(){
            return !IsOpen();
        }

    }
}
