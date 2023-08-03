using System.Globalization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json.Linq;

namespace IwbrDaemon.Types.Exchange
{
    [BsonIgnoreExtraElements]
    public class KLine
    {
        [BsonElement("lastUpdateTime")]
        public DateTime CreationTime { get; private set; }

        [BsonElement("symbol")]
        public string Symbol { get; private set; }

        [BsonElement("interval")]
        public string Interval { get; private set; }

        [BsonElement("openTime")]
        [BsonRepresentation(BsonType.Int64)]
        public long OpenTime { get; private set; }

        [BsonElement("openPrice")]
        public decimal OpenPrice { get; private set; }

        [BsonElement("highPrice")]
        public decimal HighPrice { get; private set; }

        [BsonElement("lowPrice")]
        public decimal LowPrice { get; private set; }

        [BsonElement("closePrice")]
        public decimal ClosePrice { get; private set; }

        [BsonElement("volume")]
        public decimal Volume { get; private set; }

        [BsonElement("closeTime")]
        [BsonRepresentation(BsonType.Int64)]
        public long CloseTime { get; private set; }

        [BsonElement("quoteVolume")]
        public decimal QuoteVolume { get; private set; }

        [BsonElement("tradeCount")]
        public int TradeCount { get; private set; }

        [BsonElement("takerBuyBaseVolume")]
        public decimal TakerBuyBaseVolume { get; private set; }

        [BsonElement("takerBuyQuoteVolume")]
        public decimal TakerBuyQuoteVolume { get; private set; }

        [BsonElement("isKLineClosed")]
        public bool? IsKLineClosed { get; private set; }

        [BsonElement("source")]
        public string Source { get; private set; }

        public KLine(string symbol, string interval, long openTime, decimal openPrice, decimal highPrice, decimal lowPrice, decimal closePrice, decimal volume, long closeTime, decimal quoteVolume, int tradeCount, decimal takerBuyBaseVolume, decimal takerBuyQuoteVolume, bool? isKLineClosed, string source)
        {

            CreationTime = DateTime.UtcNow;
            Symbol = symbol;
            Interval = interval;
            OpenTime = openTime;
            OpenPrice = openPrice;
            HighPrice = highPrice;
            LowPrice = lowPrice;
            ClosePrice = closePrice;
            Volume = volume;
            CloseTime = closeTime;
            QuoteVolume = quoteVolume;
            TradeCount = tradeCount;
            TakerBuyBaseVolume = takerBuyBaseVolume;
            TakerBuyQuoteVolume = takerBuyQuoteVolume;
            IsKLineClosed = isKLineClosed;
            Source = source;
        }

        public static KLine FromArray(string symbol, string interval, object[] array, string source, bool? isKLineClosed)
        {
            if (array.Length != 12)
            {
                throw new ArgumentException("Invalid array length for KLine data.");
            }

            return new KLine(
                symbol,
                interval,
                (long)array[0],
                decimal.Parse((string)array[1], CultureInfo.InvariantCulture),
                decimal.Parse((string)array[2], CultureInfo.InvariantCulture),
                decimal.Parse((string)array[3], CultureInfo.InvariantCulture),
                decimal.Parse((string)array[4], CultureInfo.InvariantCulture),
                decimal.Parse((string)array[5], CultureInfo.InvariantCulture),
                (long)array[6],
                decimal.Parse((string)array[7], CultureInfo.InvariantCulture),
                (int)array[8],
                decimal.Parse((string)array[9], CultureInfo.InvariantCulture),
                decimal.Parse((string)array[10], CultureInfo.InvariantCulture),
                isKLineClosed,
                source
            );
        }

        public static KLine FromJArray(string symbol, string interval, JArray array, string source, bool? isKLineClosed)
        {
            if (array.Count != 12)
            {
                throw new ArgumentException("Invalid JArray size for KLine data.");
            }

            return new KLine(
                symbol,
                interval,
                (long)array[0],
                decimal.Parse(array[1].ToString(), CultureInfo.InvariantCulture),
                decimal.Parse(array[2].ToString(), CultureInfo.InvariantCulture),
                decimal.Parse(array[3].ToString(), CultureInfo.InvariantCulture),
                decimal.Parse(array[4].ToString(), CultureInfo.InvariantCulture),
                decimal.Parse(array[5].ToString(), CultureInfo.InvariantCulture),
                long.Parse(array[6].ToString()),
                decimal.Parse(array[7].ToString(), CultureInfo.InvariantCulture),
                int.Parse(array[8].ToString()),
                decimal.Parse(array[9].ToString(), CultureInfo.InvariantCulture),
                decimal.Parse(array[10].ToString(), CultureInfo.InvariantCulture),
                isKLineClosed,
                source
            );
        }

    }

}
