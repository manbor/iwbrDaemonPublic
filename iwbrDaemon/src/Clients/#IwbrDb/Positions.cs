using MongoDB.Driver;
using IwbrDaemon.Trading.Analysis;
using IwbrDaemon.Trading;
using MongoDB.Bson;

namespace IwbrDaemon.Clients
{
    public partial class IwbrDb
    {
        public static void MergePosition(Position pos) {

            pos.ClearReasonOfClose();

            var options = new ReplaceOptions { IsUpsert = true };

            var filter = Builders<Position>.Filter.And(
                Builders<Position>.Filter.Eq("symbol", pos.Symbol)
                ,Builders<Position>.Filter.Eq("creationTime", pos.CreationTime)
            );

            var result = Collection.Positions.ReplaceOne(filter, pos, options);

        }


        public static Position GetOpenPosition(string symbol) { 
            return GetOpenPositions(symbol).First();            
        }


        public static List<Position> GetOpenPositions(string? symbol = null) { 

            var filterList = new List<FilterDefinition<Position>>();

            filterList.Add(Builders<Position>.Filter.Ne("openResponse", BsonNull.Value));
            filterList.Add(Builders<Position>.Filter.Ne("openOrder", BsonNull.Value));
            filterList.Add(Builders<Position>.Filter.Eq("payTranId", 0));

            if(!string.IsNullOrEmpty(symbol)) {
                filterList.Add(Builders<Position>.Filter.Eq("symbol", symbol));
            }

            //---------------------------------

            var filter = Builders<Position>.Filter.And(filterList);
            return Collection.Positions.Find(filter).ToList() ;
        }



        public static List<Position> GetRecentClosedPositionsNoProfit(string? symbol = null, int seconds = 600, bool isGte = true) 
        { 
            List<FilterDefinition<Position>> filterList = new List<FilterDefinition<Position>>();
            var startDate = DateTime.UtcNow.AddSeconds(-seconds);    

            filterList.Add(
                Builders<Position>.Filter.And(
                    Builders<Position>.Filter.Ne("payTranId", BsonNull.Value)
                    ,Builders<Position>.Filter.Ne("payTranId", 0)
                )
            );
            filterList.Add(Builders<Position>.Filter.Lt("profit", 0));

            if(isGte)
                filterList.Add(Builders<Position>.Filter.Gte("lastUpdateTime", startDate));
            else
                filterList.Add(Builders<Position>.Filter.Lte("lastUpdateTime", startDate));


            if(!string.IsNullOrEmpty(symbol)){
                filterList.Add(Builders<Position>.Filter.Eq("symbol", symbol));
            }

            var filter = Builders<Position>.Filter.And(filterList);

            return Collection.Positions.Find(filter).ToList();
        }



        public static List<Position> GetFailOpenPositions(string? symbol = null) { 

            var filterList = new List<FilterDefinition<Position>>();
            var startDate = DateTime.UtcNow.AddMinutes(-1);    

            filterList.Add(Builders<Position>.Filter.Lte("lastUpdateTime", startDate));
            filterList.Add(Builders<Position>.Filter.Eq("openResponse", BsonNull.Value));
            filterList.Add(Builders<Position>.Filter.Eq("openOrder", BsonNull.Value));
            filterList.Add(Builders<Position>.Filter.Eq("payTranId", 0));

            if(!string.IsNullOrEmpty(symbol)) {
                filterList.Add(Builders<Position>.Filter.Eq("symbol", symbol));
            }

            //---------------------------------

            var filter = Builders<Position>.Filter.And(filterList);
            return Collection.Positions.Find(filter).ToList() ;
        }


        public static void DeletePosition(Position pos) {

            var filter = Builders<Position>.Filter.And(
                Builders<Position>.Filter.Eq("symbol", pos.Symbol)
                ,Builders<Position>.Filter.Eq("creationTime", pos.CreationTime)
            );

            var result = Collection.Positions.DeleteOne(filter);

        }

        public static void PositionsRetention() {
            string methodName = $"PositionsRetention".PadRight(Config.PadRightMethodName);
            //-------------------------------------------------------------------------------

            var timeToStartKeep =  DateTime.UtcNow.AddHours(-2);

            var filter = Builders<Position>.Filter.And(
                    Builders<Position>.Filter.Lt("lastUpdateTime", timeToStartKeep)
                    ,Builders<Position>.Filter.Eq("openResponse", BsonNull.Value)
                    ,Builders<Position>.Filter.Eq("openOrder", BsonNull.Value)
                    ,Builders<Position>.Filter.Eq("closeResponse", BsonNull.Value)
                    ,Builders<Position>.Filter.Eq("closeOrder", BsonNull.Value)
                    ,Builders<Position>.Filter.Eq("profit", 0)
            );

            Collection.Positions.DeleteMany(filter);

            if(Config.DebugEnable) log.Debug($"{methodName}done");
        }


    }

}