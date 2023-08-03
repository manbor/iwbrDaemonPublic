using MongoDB.Driver;
using MongoDB.Bson;
using IwbrDaemon.Types.Exchange;

namespace IwbrDaemon.Clients
{
    public partial class IwbrDb
    {
        public static List<AssetBalance> GetUserAssets() { 
            return GetLastWalletStatus().Assets;
        }

        

        public static decimal GetFirstBalance() { 
            return GetFirstWalletStatus().Balance.Value;
        }

        public static decimal GetActualBalance() { 
            return GetLastWalletStatus().Balance.Value;
        }

        public static decimal GetMaxBalance() { 
        
            var dateThreshold = DateTime.UtcNow.AddDays(-1);

            var filter = Builders<WalletStatus>.Filter.Gte("creationTime", dateThreshold);
            var sort = Builders<WalletStatus>.Sort.Descending("balance.value");

            var result = Collection.WalletStatus
                            .Find(filter)
                            .Sort(sort)
                            .Limit(1)
                            .First()
                        ;

            return result.Balance.Value;

        }


        public static WalletStatus GetFirstWalletStatus() { 
            var filter = Builders<WalletStatus>.Filter.Empty;
            var sort = Builders<WalletStatus>.Sort.Ascending("creationTime");
            return Collection.WalletStatus.Find(filter).Sort(sort).First();
        }

        public static WalletStatus GetLastWalletStatus() { 
            var filter = Builders<WalletStatus>.Filter.Empty;
            var sort = Builders<WalletStatus>.Sort.Descending("creationTime");
            return Collection.WalletStatus.Find(filter).Sort(sort).First();
        }


        public static void MergeWalletStatus(WalletStatus wallet) {
            Collection.WalletStatus.InsertOne(wallet);
        }


        public static void WalletStatusRetention() {
            string methodName = $"UserAssetsRetention".PadRight(Config.PadRightMethodName);

            var timeToStartKeep = DateTime.UtcNow.AddYears(-1);

            var filter = Builders<WalletStatus>.Filter.Lt("creationTime", timeToStartKeep);
            Collection.WalletStatus.DeleteMany(filter);
        
            //-----------------------------------------------------------------------------------------
            var timeToStartKeepDx = DateTime.UtcNow.AddDays(-7);
            var timeToStartKeepSx = DateTime.UtcNow.AddDays(-14);

            filter = Builders<WalletStatus>.Filter.And(
                        Builders<WalletStatus>.Filter.Gte("creationTime", timeToStartKeepSx)
                        ,Builders<WalletStatus>.Filter.Lte("creationTime", timeToStartKeepDx)
                        ,Builders<WalletStatus>.Filter.Ne("assets", BsonNull.Value)
            );

            var update = Builders<WalletStatus>.Update.Set("assets",  BsonNull.Value);
            var updateResult = Collection.WalletStatus.UpdateMany(filter, update);

            if(Config.DebugEnable) log.Debug($"{methodName}done");
        }

    }

}