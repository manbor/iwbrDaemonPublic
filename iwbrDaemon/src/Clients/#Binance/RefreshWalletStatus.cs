using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using RestSharp;
using MongoDB.Driver.Linq;
using IwbrDaemon.Types.Exchange;

namespace IwbrDaemon.Clients
{
    public partial class Binance
    {
        //public static ConcurrentBag<AssetBalance> UserAssets { get; private set; }
        public static bool UserAssetsIsLoaded { get; private set; } = false;

        public static void RefreshWalletStatus()
        {
            string methodName = "RefreshWalletStatus".PadRight(Config.PadRightMethodName);
            List<AssetBalance> _userAssets = new List<AssetBalance>();

            try
            {
                var request = new RestRequest("/sapi/v1/margin/account", Method.Get);
                var response = ApiCall(request, AuthLevel.LoginRequired, true);

                if (response.IsOk)
                {
                    JObject json = JObject.Parse(response.HttpResponse.Content);
                    _userAssets = JsonConvert.DeserializeObject<List<AssetBalance>>(json["userAssets"].ToString());

                    var wallet = new WalletStatus(_userAssets);
                    IwbrDb.MergeWalletStatus(wallet);

                    UserAssetsIsLoaded = true;             
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
            
            var cnt = _userAssets.Where(p => p.NetAsset != 0 || p.Borrowed != 0 || p.Free >0 ).Count();

            if(Config.DebugEnable) log.Debug($"{methodName} {cnt} assets fetched.");

        }

    }

}
