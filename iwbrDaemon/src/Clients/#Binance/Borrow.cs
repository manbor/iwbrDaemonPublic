using Newtonsoft.Json.Linq;
using RestSharp;
using System.Security.Cryptography;
using System.Text;

namespace IwbrDaemon.Clients
{
    public partial class Binance
    {
        public static long Borrow(string asset, decimal amount)
        {
            string methodName = $"Borrow {asset} {amount}".PadRight(Config.PadRightMethodName);

            try
            {
                var request = new RestRequest("/sapi/v1/margin/loan", Method.Post);
                request.AddParameter("asset", asset);
                request.AddParameter("amount", amount);

                var response = ApiCall(request, AuthLevel.LoginRequired, true);

                if (response.IsOk)
                {
                    JObject json = JObject.Parse(response.HttpResponse.Content);
                    return ((JToken) json["tranId"]).Value<long>();        
                } else {
                    if(response.HttpResponse.Content.Contains("-1102")){
                        if(Config.DebugEnable) log.Debug($"{methodName} asset: {asset}, amount: {amount}");
                    }

                    throw new Exception($"{response.HttpResponse.Content}");
                }

            }
            catch (Exception ex)
            {
                log.Error($"{methodName} {ex.ToString()}");
                throw new Exception();
            }

        }
    }
}
