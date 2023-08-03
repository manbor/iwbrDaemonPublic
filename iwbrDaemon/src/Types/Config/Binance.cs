namespace IwbrDaemon.Types.Config 
{
    public class Binance
    {
        public string ApiKey { get; private set; }
        public string SecretKey { get; private set; }
        public string QuoteAsset { get; private set; }
        public int MaxCallPerSec { get; private set; }
        public int RecentKLinesSize { get; private set; }
        public bool EnableTradingLong { get; private set;}
        public bool EnableTradingShort { get; private set; }


        public Binance(string apiKey, string secretKey, string quoteAsset, int maxCallPerSec, int recentKLinesSize, bool enableTradingLong, bool enableTradingShort)
        {
            ApiKey = apiKey;
            SecretKey = secretKey;
            QuoteAsset = quoteAsset;
            MaxCallPerSec = maxCallPerSec;
            RecentKLinesSize = recentKLinesSize;
            EnableTradingLong = enableTradingLong;
            EnableTradingShort = enableTradingShort;
        }
    }
}