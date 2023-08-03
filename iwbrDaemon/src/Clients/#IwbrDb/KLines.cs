using MongoDB.Driver;
using IwbrDaemon.Types.Exchange;
using MongoDB.Bson;

namespace IwbrDaemon.Clients
{
    public partial class IwbrDb
    {
        public static void MergeKLine(KLine KLine)
        {
            List<KLine> list = new List<KLine>();
            list.Add(KLine);
            MergeKLines(list);
        }

        public static void MergeKLines(List<KLine> KLinesList)
        {
            string methodName = "MergeKLines List".PadRight(Config.PadRightMethodName);

            var bulkOps = new List<WriteModel<KLine>>();

            foreach (var kline in KLinesList)
            {
                var filter = Builders<KLine>.Filter.And(
                    Builders<KLine>.Filter.Eq("symbol", kline.Symbol),
                    Builders<KLine>.Filter.Eq("interval", kline.Interval),
                    Builders<KLine>.Filter.Eq("openTime", kline.OpenTime)
                );

                var replaceOne = new ReplaceOneModel<KLine>(filter, kline) { IsUpsert = true };
                bulkOps.Add(replaceOne);
            }

            Collection.KLines.BulkWrite(bulkOps);
        }

        public static List<KLine> GetKLines(string symbol, string interval, DateTime startTime, DateTime endTime)  {
            string methodName = "GetKLines".PadRight(Config.PadRightMethodName) + $"{symbol} {interval}";

            long startTimeTs = new DateTimeOffset(startTime).ToUnixTimeMilliseconds();
            long endTimeTs = new DateTimeOffset(endTime.AddMinutes(1)).ToUnixTimeMilliseconds();

            var filter = Builders<KLine>.Filter.And(
                Builders<KLine>.Filter.Eq("symbol",symbol), 
                Builders<KLine>.Filter.Eq("interval", interval),
                Builders<KLine>.Filter.Lte("openTime", endTimeTs),
                Builders<KLine>.Filter.Gte("closeTime", startTimeTs)
            );

            var sort = Builders<KLine>.Sort.Ascending("openTime");

            List<KLine> kLines = Collection.KLines.Find(filter).Sort(sort).ToList();

            return kLines;

        }


        public static List<KLine> GetKLines(string symbol, string interval, int n, DateTime endTime = default) 
        {
            long minutes = 0;

            switch (interval) {
                case "1m":  minutes = n; break;
                case "5m":  minutes = n * 5; break;
                case "15m": minutes = n * 15; break;
                case "30m": minutes = n * 30; break;
                case "1h":  minutes = n * 60; break;
            }

            DateTime startTime = endTime.AddMinutes(-minutes);

            return GetKLines(symbol,interval, startTime, endTime).Take(n).ToList() ;

        }


        public static void KLinesRetention() {
            string methodName = $"KLinesRetention".PadRight(Config.PadRightMethodName);

            var today = DateTime.Today;
            var ts3days = new DateTimeOffset(today.AddDays(-3)).ToUnixTimeMilliseconds();
            var ts180days = new DateTimeOffset(today.AddDays(-180)).ToUnixTimeMilliseconds();

            var filters = new List<FilterDefinition<KLine>>();

            filters.Add(Builders<KLine>.Filter.Eq("interval", "1m") & Builders<KLine>.Filter.Lt("openTime", ts3days));
            //filters.Add(Builders<KLine>.Filter.Ne("interval", "1m") & Builders<KLine>.Filter.Lt("openTime", ts180days));

            var deleteFilter = Builders<KLine>.Filter.Or(filters);

            var result = Collection.KLines.DeleteMany(deleteFilter);


            if (Config.DebugEnable) log.Debug($"{methodName} done : {result.DeletedCount} record deleted");
        }


    }
}
