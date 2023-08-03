using RestSharp;
using System.Collections.Concurrent;
using System.Net;

namespace IwbrDaemon.Clients
{

    public partial class Binance
    {
        private enum AuthLevel
        {
            NoLogin,            // no login required
            LoginRequired,      // login required
            ManualApproval      // strong authentication
        }

        private static string[] ServicesAllowedToBypass = new string[] {
            "/sapi/v1/margin/account"
            ,"/api/v3/allOrders"
            ,"/api/v3/myTrades"
            ,"/api/v3/exchangeInfo"
        };

        private static ConcurrentDictionary<string, int> ApiCallsRates = new ConcurrentDictionary<string, int>();
        private static object _apiCallCheckCounter = new object();

        private static RestResponseWrapper ApiCall(RestRequest request, AuthLevel authLevel, bool requireTimestamp = false)
        {
            string methodName = "ApiCall".PadRight(Config.PadRightMethodName);


            string parameters = String.Join(" ", request
                                                .Parameters
                                                .Select(p => $"{p.Name}:{p.Value}")
                                        );

            /*
             * ----------------------------------------
             * check if need wait
             * 
             */

            // counter check
            bool isWating = true;
            string nowKey = "";

            bool waitWasLogged = false;


            while (isWating)
            {
                nowKey = DateTime.UtcNow.ToString("yyyyMMddHHmmss");

                // update apiMinute counter
                lock (_apiCallCheckCounter)
                {
                    if (!ApiCallsRates.ContainsKey(nowKey))
                    {
                        ApiCallsRates.TryAdd(nowKey, 0);
                    }

                    int countPerSecOld,countPerSec;

                    bool getStatus = ApiCallsRates.TryGetValue(nowKey, out countPerSecOld);
                    countPerSec = countPerSecOld + 1;
                    bool updateStatus = ApiCallsRates.TryUpdate(nowKey, countPerSec, countPerSecOld);

                    isWating = countPerSec > Config.Binance.MaxCallPerSec;

                    if (isWating)
                    {
                        if (!waitWasLogged)
                        {
                            if(Config.DebugEnable) log.Debug($"{methodName} {nowKey} {(countPerSecOld).ToString().PadLeft(3) }/s { "LOCK".PadLeft(10) } {request.Resource} {parameters}");
                            waitWasLogged = true;   
                        }
                        //Thread.Sleep(100);
                        continue;
                    }

                    if (waitWasLogged)
                    {
                        if(Config.DebugEnable) log.Debug($"{methodName} {nowKey} {countPerSecOld.ToString().PadLeft(3)}/s {(waitWasLogged ? "UNLOCK" : String.Empty).PadLeft(10)} {request.Resource} {parameters}");
                    }

                }
            }

            // delete all prev keys
            var keysToRemove =  ApiCallsRates
                                .Where(p => long.Parse(p.Key) < long.Parse(nowKey))
                                .Select(p => p.Key)
                                .ToList();

            foreach(var key in keysToRemove) {
                int tmp;
                ApiCallsRates.TryRemove(key, out tmp);
            }



            /*
             * ----------------------------------------
             * Adding parameters
             * 
             */

            if (requireTimestamp)
            {
                request.AddParameter("timestamp", Utils.GetCurrentTimestamp());
            }

            if (authLevel == AuthLevel.LoginRequired)
            {
                request.AddHeader("X-MBX-APIKEY", _ApiKey);
                request.AddParameter("signature", GetSignature(request.Parameters));
            }

            /*
             * ----------------------------------------
             * execute call
             * 
             */
            RestResponse response = new RestResponse();

            try
            {
                response = client.Execute(request);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    return new RestResponseWrapper(response, false); ;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                log.Error($"{methodName}Content -> {response.Content}");
                log.Error($"{methodName}ErrorMessage -> {response.ErrorMessage}");
                System.Environment.Exit(0);
            }

            return new RestResponseWrapper(response, true);

        }

        private class RestResponseWrapper
        {
            public RestResponse HttpResponse { get; private set; }
            public bool IsOk { get; private set; }

            public RestResponseWrapper(RestResponse httpResponse, bool isOk)
            {
                HttpResponse = httpResponse;
                IsOk = isOk;
            }
        }

    }
}
