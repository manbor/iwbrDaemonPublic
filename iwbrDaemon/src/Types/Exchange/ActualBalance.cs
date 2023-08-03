using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace IwbrDaemon.Types.Exchange
{

    public class ActualBalance
    {

        [BsonElement("quoteAsset")]
        public string QuoteAsset { get; private set; }

        [BsonElement("value")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal Value { get; private set; }

        public ActualBalance(string quoteAsset, decimal value)
        {
            QuoteAsset = quoteAsset;
            Value = value;
        }

        public ActualBalance(List<AssetBalance> userAssets)
        {
            QuoteAsset = IwbrDaemon.Config.Binance.QuoteAsset;
            Value = 0;

            Value += userAssets
                            .Where(p => p.Asset != IwbrDaemon.Config.Binance.QuoteAsset)
                            .Select(p => Utils.ConvertToCurrentAsset(p))
                            .Sum();

            Value += userAssets
                                .Where(p => p.Asset == IwbrDaemon.Config.Binance.QuoteAsset)
                                .Select(p => p.NetAsset)
                                .Sum()
                                ;

        }

    }

}