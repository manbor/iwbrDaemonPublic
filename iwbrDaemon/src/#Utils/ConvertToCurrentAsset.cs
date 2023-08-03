using IwbrDaemon.Types.Exchange;
using IwbrDaemon.Clients;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IwbrDaemon
{
    public partial class Utils
    {
        public static decimal ConvertToCurrentAsset(AssetBalance p)
        {
            try
            {
                return  p.NetAsset *  IwbrDb.GetTicker($"{p.Asset}{Config.Binance.QuoteAsset}").Price;
            }
            catch (KeyNotFoundException e)
            {
                return 0;
            }
        }
    }
}
