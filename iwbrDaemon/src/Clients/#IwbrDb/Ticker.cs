using MongoDB.Driver;
using IwbrDaemon.Trading;
using IwbrDaemon.Types.Exchange;

namespace IwbrDaemon.Clients
{
    public partial class IwbrDb
    {

        public static void MergeTicker(Ticker ticker) {
            var options = new ReplaceOptions { IsUpsert = true };
            
            var filter = Builders<Ticker>.Filter.And(
                    Builders<Ticker>.Filter.Eq("symbol", ticker.Symbol)
                    );

            var result = Collection.Tickers.ReplaceOne(filter, ticker, options);
        }

        
        public static Ticker GetTicker(string symbol) 
        {
            try{
                var filter1 = Builders<Ticker>.Filter.And(
                    Builders<Ticker>.Filter.Eq("symbol",symbol)
                );  

                return IwbrDb.Collection.Tickers.Find(filter1).First();
            }
            catch(Exception e){

                var filter2 = Builders<KLine>.Filter.And(
                    Builders<KLine>.Filter.Eq("symbol",symbol), 
                    Builders<KLine>.Filter.Eq("interval", "1m")
                );

                var sort2 = Builders<KLine>.Sort.Descending("closeTime");

                try
                {
                    var kline = Collection.KLines.Find(filter2).Sort(sort2).Limit(1).First();

                    return new Ticker(symbol, kline.ClosePrice, kline.CloseTime);
                } 
                catch (System.InvalidOperationException ex) {
                    return new Ticker(symbol, 0, 0);
                }

            }

        }
    }
}
