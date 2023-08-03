using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace IwbrDaemon.Types.Exchange
{

    [BsonIgnoreExtraElements]
    public class AssetBalance
    {

        [BsonElement("asset")]
        public string Asset { get; private set; }

        [BsonElement("borrowed")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal Borrowed { get; private set; }

        [BsonElement("free")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal Free { get; private set; }

        [BsonElement("interest")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal Interest { get; private set; }

        [BsonElement("locked")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal Locked { get; private set; }

        [BsonElement("netAsset")]
        [BsonRepresentation(BsonType.Decimal128)]
        public decimal NetAsset { get; private set; }

        public AssetBalance(string asset, decimal borrowed, decimal free, decimal interest, decimal locked, decimal netAsset)
        {
            Asset = asset;
            Borrowed = borrowed;
            Free = free;
            Interest = interest;
            Locked = locked;
            NetAsset = netAsset;
        }
    }
}
