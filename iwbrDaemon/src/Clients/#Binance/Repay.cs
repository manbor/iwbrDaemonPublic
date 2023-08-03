using Newtonsoft.Json.Linq;
using RestSharp;
using System.Security.Cryptography;
using System.Text;

namespace IwbrDaemon.Clients
{
    public partial class Binance
    {
        public static long Repay(string asset, decimal amount)
        {
            string methodName = $"Repay {asset} {amount}".PadRight(Config.PadRightMethodName);

            bool isExceptionCalled = false;
            try
            {
                var request = new RestRequest("/sapi/v1/margin/repay", Method.Post);
                request.AddParameter("asset", asset);
                request.AddParameter("amount", amount);

                var response = ApiCall(request, AuthLevel.LoginRequired, true);

                if (response.IsOk)
                {
                    JObject json = JObject.Parse(response.HttpResponse.Content);
                    return ((JToken) json["tranId"]).Value<long>();        
                } else {
                    isExceptionCalled = true;
                    throw new Exception($"{response.HttpResponse.Content}");
                }
            }
            catch (Exception ex)
            {
                if(isExceptionCalled){
                    if(Config.DebugEnable) log.Debug($"{methodName} {ex.ToString()}");
                } else {
                    log.Error($"{methodName} {ex.ToString()}");
                }
                throw ex;
            }

        }
    }
}
