using MongoDB.Driver;
using System.Collections.Concurrent;
using IwbrDaemon.Types.Exchange;

namespace IwbrDaemon.Clients
{
    public partial class IwbrDb
    {
        public static void MergeTradingPairs(ConcurrentBag<TradingPair> tradingPairs) {
            
            var options = new ReplaceOptions { IsUpsert = true };

            foreach (var pair in tradingPairs ) {
                
                var filter = Builders<TradingPair>.Filter.Eq("symbol", pair.Symbol);
                
                var result = Collection.TradingPairs.ReplaceOne(filter, pair, options);
            }
        }

        public static List<TradingPair> GetTradingPairs(string symbol = null) 
        {
            //var filterList = new List<FilterDefinition<TradingPair>>();
            //if(!string.IsNullOrEmpty(symbol)) {
            //     filterList.Add(Builders<TradingPair>.Filter.Eq("symbol", symbol));
            // }
            // var filter = Builders<TradingPair>.Filter.And(filterList);



            FilterDefinition<TradingPair> filter;

            if(!string.IsNullOrEmpty(symbol)) {
                filter = Builders<TradingPair>.Filter.Eq("symbol", symbol);
            } else {
                filter = Builders<TradingPair>.Filter.Empty;
            }

            var sort = Builders<TradingPair>.Sort.Ascending("symbol");

            return Collection.TradingPairs.Find(filter).Sort(sort).ToList();
        }


        
    }

}