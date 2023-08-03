using MongoDB.Driver;
using IwbrDaemon.Types.Exchange;
using IwbrDaemon.Trading.Analysis;

namespace IwbrDaemon.Clients
{
    public partial class IwbrDb
    {

        public static void InsertSymbolAnalysis(SymbolAnalysis symbolAnalysis)
        {
            Collection.SymbolsAnalysis.InsertOne(symbolAnalysis);
        }
        

        public static void InsertSymbolAnalysis(List<SymbolAnalysis> symbolAnalysisList) {
            Collection.SymbolsAnalysis.InsertMany(symbolAnalysisList);
        }


        public static void MergeSymbolAnalysis(SymbolAnalysis sym) {
            var options = new ReplaceOptions { IsUpsert = true };

            var filter = Builders<SymbolAnalysis>.Filter.Eq("symbol", sym.Symbol);
            
            var result = Collection.SymbolsAnalysis.ReplaceOne(filter, sym, options);
        }


        public static bool ExistSymbolAnalysis(string symbol, int seconds = 1) { // TODO: add realtime config

            DateTime dtStart = DateTime.UtcNow.AddSeconds(-seconds);

            var filter = Builders<SymbolAnalysis>.Filter.And(
                Builders<SymbolAnalysis>.Filter.Eq("symbol", symbol)
                ,Builders<SymbolAnalysis>.Filter.Gte("creationTime", dtStart)
            );

            return Collection.SymbolsAnalysis.Find(filter).CountDocuments() > 0;
        }


        public static void SymbolsAnalysisRetention() {
            string methodName = $"SymbolsAnalysisRetention".PadRight(Config.PadRightMethodName);

            var timeToStartKeep = DateTime.UtcNow.AddMinutes(-15);

            var filter = Builders<SymbolAnalysis>.Filter.And(
                Builders<SymbolAnalysis>.Filter.Lt("requestTime", timeToStartKeep)
            );

            Collection.SymbolsAnalysis.DeleteMany(filter);

            if(Config.DebugEnable) log.Debug($"{methodName}done");
        }

    }

}