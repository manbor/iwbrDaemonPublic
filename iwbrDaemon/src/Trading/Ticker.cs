using MongoDB.Bson.Serialization.Attributes;

namespace IwbrDaemon.Trading
{
    [BsonIgnoreExtraElements]
    public class Ticker
    {
        [BsonElement("symbol")]
        public string Symbol { get; private set; }

        [BsonElement("priceTime")]
        public DateTime PriceTime{ get; private set; }

        [BsonElement("price")]
        public decimal Price { get; private set; }

        [BsonElement("price_var")]
        public decimal Price_var { get; private set; }

        public Ticker(string symbol, decimal price, decimal price_var, DateTime priceTime)
        {
            Symbol = symbol;
            PriceTime = priceTime;
            Price = price;
            Price_var = price_var;
        }

        public Ticker(string symbol, decimal price, long priceTimeTs)
        {
            Symbol = symbol;
            PriceTime =  new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(priceTimeTs);
            Price = price;
        }

    }
}
