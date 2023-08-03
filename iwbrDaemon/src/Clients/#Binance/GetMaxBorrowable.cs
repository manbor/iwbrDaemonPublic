using Newtonsoft.Json.Linq;
using RestSharp;
using System.Security.Cryptography;
using System.Text;

namespace IwbrDaemon.Clients
{
    public partial class Binance
    {
        public static decimal GetMaxBorrowable(string asset, out bool status, out string info)
        {
            string methodName = "GetMaxBorrowable".PadRight(Config.PadRightMethodName);

            try
            {
                var request = new RestRequest("/sapi/v1/margin/maxBorrowable", Method.Get);
                request.AddParameter("asset", asset);
                var response = ApiCall(request, AuthLevel.LoginRequired, true);

                if (response.IsOk)
                {
                    JObject json = JObject.Parse(response.HttpResponse.Content);
                    status = true;
                    info = null;
                    return ((JToken) json["amount"]).Value<decimal>();        
                } else {
                    throw new Exception($"{response.HttpResponse.Content}");
                }
            }
            catch (Exception ex)
            {
                if(!ex.ToString().Contains("-3045")){
                    log.Error(ex.ToString());
                }
                if(Config.DebugEnable) log.Debug($"{methodName}: FAILED");

                status = false;
                info = ex.ToString();
                return -1;
            }

        }
    }
}
