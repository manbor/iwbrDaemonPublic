using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using RestSharp;
using System.Collections.Concurrent;
using IwbrDaemon.Types.Exchange;

namespace IwbrDaemon.Clients
{
    public partial class Binance
    {
        public static Order GetOrder(string symbol, long orderId)
        {
            string methodName = "GetOrder".PadRight(Config.PadRightMethodName);

            try
            {
                var request = new RestRequest("/sapi/v1/margin/allOrders", Method.Get);
                request.AddParameter("symbol", symbol);
                request.AddParameter("orderId", orderId);
                var response = ApiCall(request, AuthLevel.LoginRequired, true);

                if (response.IsOk)
                {
                    JArray json = JArray.Parse(response.HttpResponse.Content);
                    List<Order> orders =JsonConvert.DeserializeObject<List<Order>>(response.HttpResponse.Content);
                    return orders.ElementAt(0);
                } else {
                    throw new Exception($"{response.HttpResponse.Content}");
                }

            }
            catch (Exception ex)
            {
                log.Error(ex.ToString());
                if(Config.DebugEnable) log.Debug($"{methodName}: FAILED");
                return null;
            }


        }
    }
}
