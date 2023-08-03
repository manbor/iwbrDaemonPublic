using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System.Collections.Concurrent;
using IwbrDaemon.Types.Exchange;

namespace IwbrDaemon.Clients
{
    public partial class Binance
    {
        public static ConcurrentBag<TradingPair> TradingPairs { get; private set; }
        public static bool TradingPairsIsLoaded { get; private set; } = false;

        public static Boolean TradingPairsIsEmpty()
        {
            if (TradingPairs == null || TradingPairs.Count == 0)
                return true;

            return false;
        }

        public static void RefreshTradingPairs()
        {
            string methodName = "RefreshTradingPairs".PadRight(Config.PadRightMethodName);
            ConcurrentBag<TradingPair> _TradingPairs = new ConcurrentBag<TradingPair>();

            try
            {
                var request = new RestRequest("/api/v3/exchangeInfo", Method.Get);
                var response = ApiCall(request, AuthLevel.NoLogin);

                if (response.IsOk)
                {
                    var content = JObject.Parse(response.HttpResponse.Content);
                    _TradingPairs = JsonConvert.DeserializeObject< ConcurrentBag<TradingPair >> (content["symbols"].ToString() );

                    TradingPairs = new ConcurrentBag<TradingPair>(
                                                                        (
                                                                                from pair in _TradingPairs
                                                                                where 1 == 1
                                                                                    && pair.QuoteAsset == Config.Binance.QuoteAsset
                                                                                    && pair.IsMarginTradingAllowed == true
                                                                                    && pair.OrderTypes.Contains("MARKET")
                                                                                    && !Config.Blacklist.Contains(pair.Symbol)
                                                                                select pair
                                                                        )
                                                                        .ToList()
                                                                        .OrderBy(p => p.Symbol)
                                                                );

                    IwbrDb.MergeTradingPairs(TradingPairs);

                    TradingPairsIsLoaded = true;
                } else {
                    throw new Exception($"{response.HttpResponse.Content}");
                }
            }
            catch (Exception ex)
            {
                log.Error(ex.ToString());
                if(Config.DebugEnable) log.Debug($"{methodName}: FAILED");
                return ;
            }

            if(Config.DebugEnable) log.Debug($"{methodName} {TradingPairs.Count} element{(TradingPairs.Count == 1 ? string.Empty : 's')} fetched");
        }

    }
}