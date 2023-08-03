using Newtonsoft.Json.Linq;
using RestSharp;
using Newtonsoft.Json;
using IwbrDaemon.Trading;

namespace IwbrDaemon.Clients
{
    public partial class Binance
    {
        //TODO add StopLoss parameter
        public static PlaceOrderResponse PlaceOrder(string symbol, decimal amount,ePositionType type, ePositionAction action)
        {
            string methodName = $"PlaceOrder {symbol} {type} {amount}".PadRight(Config.PadRightMethodName);

            bool isExceptionCalled = false;
            try
            {                
                var request = new RestRequest("/sapi/v1/margin/order", Method.Post);
                request.AddParameter("symbol", symbol);
                request.AddParameter("type", "MARKET");               
                request.AddParameter("newOrderRespType", "FULL");

                if(type == ePositionType.Long && action == ePositionAction.Open){
                    request.AddParameter("sideEffectType", "MARGIN_BUY");
                    request.AddParameter("side", "BUY");  
                    request.AddParameter("quoteOrderQty", amount);

                }else if(type == ePositionType.Long && action == ePositionAction.Close){
                    request.AddParameter("sideEffectType", "NO_SIDE_EFFECT");
                    request.AddParameter("side", "SELL");
                    request.AddParameter("quantity", amount);

                }else if(type == ePositionType.Short && action == ePositionAction.Open){
                    request.AddParameter("sideEffectType", "MARGIN_BUY");
                    request.AddParameter("side", "SELL");  
                    request.AddParameter("quantity", amount);

                }else if(type == ePositionType.Short && action == ePositionAction.Close){
                    request.AddParameter("sideEffectType", "NO_SIDE_EFFECT");
                    request.AddParameter("side", "BUY");  
                    request.AddParameter("quantity", amount);

                }


                var response = ApiCall(request, AuthLevel.LoginRequired, true);
                

                if (response.IsOk)
                {
                    JObject json = JObject.Parse(response.HttpResponse.Content);
                    return JsonConvert.DeserializeObject<PlaceOrderResponse>(response.HttpResponse.Content);

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
                throw new Exception("Place order failed");
            }

        }
    }
}
