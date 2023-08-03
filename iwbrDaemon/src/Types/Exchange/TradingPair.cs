using System.Text.RegularExpressions;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace IwbrDaemon.Types.Exchange
{
    [BsonIgnoreExtraElements]
    public class TradingPair
    {
        [BsonElement("lastUpdateTime")]
        public DateTime CreationTime { get; private set; }

        [BsonElement("symbol")]
        public string Symbol { get; private set; }

        [BsonElement("status")]
        public string Status { get; private set; }

        [BsonElement("baseAsset")]
        public string BaseAsset { get; private set; }

        [BsonElement("baseAssetPrecision")]
        public int BaseAssetPrecision { get; private set; }

        [BsonElement("quoteAsset")]
        public string QuoteAsset { get; private set; }

        [BsonElement("quotePrecision")]
        public int QuotePrecision { get; private set; }

        [BsonElement("quoteAssetPrecision")]
        public int QuoteAssetPrecision { get; private set; }

        [BsonElement("orderTypes")]
        public List<string> OrderTypes { get; private set; }

        [BsonElement("icebergAllowed")]
        public bool IcebergAllowed { get; private set; }

        [BsonElement("ocoAllowed")]
        public bool OcoAllowed { get; private set; }

        [BsonElement("quoteOrderQtyMarketAllowed")]
        public bool QuoteOrderQtyMarketAllowed { get; private set; }

        [BsonElement("allowTrailingStop")]
        public bool AllowTrailingStop { get; private set; }

        [BsonElement("cancelReplaceAllowed")]
        public bool CancelReplaceAllowed { get; private set; }

        [BsonElement("isSpotTradingAllowed")]
        public bool IsSpotTradingAllowed { get; private set; }

        [BsonElement("isMarginTradingAllowed")]
        public bool IsMarginTradingAllowed { get; private set; }

        [BsonElement("filters")]
        public string Filters { get; private set; }

        [BsonElement("permissions")]
        public List<string> Permissions { get; private set; }

        [BsonElement("defaultSelfTradePreventionMode")]
        public string DefaultSelfTradePreventionMode { get; private set; }

        [BsonElement("allowedSelfTradePreventionModes")]
        public List<string> AllowedSelfTradePreventionModes { get; private set; }

        public TradingPair(
            string symbol,
            string status,
            string baseAsset,
            int baseAssetPrecision,
            string quoteAsset,
            int quotePrecision,
            int quoteAssetPrecision,
            List<string> orderTypes,
            bool icebergAllowed,
            bool ocoAllowed,
            bool quoteOrderQtyMarketAllowed,
            bool allowTrailingStop,
            bool cancelReplaceAllowed,
            bool isSpotTradingAllowed,
            bool isMarginTradingAllowed,
            string filters,
            List<string> permissions,
            string defaultSelfTradePreventionMode,
            List<string> allowedSelfTradePreventionModes)
        {
            CreationTime = DateTime.UtcNow;
            Symbol = symbol;
            Status = status;
            BaseAsset = baseAsset;
            BaseAssetPrecision = baseAssetPrecision;
            QuoteAsset = quoteAsset;
            QuotePrecision = quotePrecision;
            QuoteAssetPrecision = quoteAssetPrecision;
            OrderTypes = orderTypes;
            IcebergAllowed = icebergAllowed;
            OcoAllowed = ocoAllowed;
            QuoteOrderQtyMarketAllowed = quoteOrderQtyMarketAllowed;
            AllowTrailingStop = allowTrailingStop;
            CancelReplaceAllowed = cancelReplaceAllowed;
            IsSpotTradingAllowed = isSpotTradingAllowed;
            IsMarginTradingAllowed = isMarginTradingAllowed;
            Filters = filters;
            Permissions = permissions;
            DefaultSelfTradePreventionMode = defaultSelfTradePreventionMode;
            AllowedSelfTradePreventionModes = allowedSelfTradePreventionModes;
        }

        [JsonConstructor]
        internal TradingPair(
            string symbol,
            string status,
            string baseAsset,
            int baseAssetPrecision,
            string quoteAsset,
            int quotePrecision,
            int quoteAssetPrecision,
            List<string> orderTypes,
            bool icebergAllowed,
            bool ocoAllowed,
            bool quoteOrderQtyMarketAllowed,
            bool allowTrailingStop,
            bool cancelReplaceAllowed,
            bool isSpotTradingAllowed,
            bool isMarginTradingAllowed,
            JArray filters,
            List<string> permissions,
            string defaultSelfTradePreventionMode,
            List<string> allowedSelfTradePreventionModes)
        {
            CreationTime = DateTime.UtcNow;
            Symbol = symbol;
            Status = status;
            BaseAsset = baseAsset;
            BaseAssetPrecision = baseAssetPrecision;
            QuoteAsset = quoteAsset;
            QuotePrecision = quotePrecision;
            QuoteAssetPrecision = quoteAssetPrecision;
            OrderTypes = orderTypes;
            IcebergAllowed = icebergAllowed;
            OcoAllowed = ocoAllowed;
            QuoteOrderQtyMarketAllowed = quoteOrderQtyMarketAllowed;
            AllowTrailingStop = allowTrailingStop;
            CancelReplaceAllowed = cancelReplaceAllowed;
            IsSpotTradingAllowed = isSpotTradingAllowed;
            IsMarginTradingAllowed = isMarginTradingAllowed;
            Filters = Regex.Replace(
                            filters.ToString().Replace("\r", string.Empty).Replace("\n", string.Empty)
                            , @"\s+", string.Empty);
            Permissions = permissions;
            DefaultSelfTradePreventionMode = defaultSelfTradePreventionMode;
            AllowedSelfTradePreventionModes = allowedSelfTradePreventionModes;
        }

    }

}