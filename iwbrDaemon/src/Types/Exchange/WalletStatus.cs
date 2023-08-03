using MongoDB.Bson.Serialization.Attributes;

namespace IwbrDaemon.Types.Exchange
{
    [BsonIgnoreExtraElements]
    public class WalletStatus
    {

        [BsonElement("creationTime")]
        public DateTime CreationTime { get; private set; }

        [BsonElement("balance")]
        public ActualBalance Balance { get; private set; }

        [BsonElement("assets")]
        public List<AssetBalance> Assets { get; private set; }

        public WalletStatus(List<AssetBalance> assetsList)
        {
            CreationTime = DateTime.UtcNow;

            Assets = assetsList
                    //.Where(p => p.NetAsset != 0)
                    .OrderBy(p => p.Asset)
                    .ToList();

            Balance = new ActualBalance(Assets);


        }

    }
}