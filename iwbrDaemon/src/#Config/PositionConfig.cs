using IwbrDaemon.Types.Config.Position;
using Newtonsoft.Json.Linq;

namespace IwbrDaemon
{

    public static partial class Config
    {
        public class PositionConfig {
            public StopLoss SL {get; private set;}
            public GlobalTakeProfit GlobalTP {get; private set;}
            public GlobalStopLoss GlobalSL {get; private set;}

            public TakeProfit TP {get; private set;}
            public TrailingStopLoss TSL {get; private set;}
            public LosingCooldown LC {get; private set;}
            public FreezeCooldown FC {get; private set;}
            public decimal InvestQty {get; private set;}
            public int MaxOpenPos {get; private set;}
            public bool CloseAll {get; private set;}
            public bool FindHighVolatility {get; private set;}
            public bool FindFirstLong {get; private set;}
            public decimal MinWalletBalance {get; private set;}

            public PositionConfig(JToken position) {
                
                InvestQty = ((JToken) position["InvestQty"]).Value<decimal>();
                MaxOpenPos = ((JToken) position["MaxOpenPos"]).Value<int>();
                CloseAll = ((JToken) position["CloseAll"]).Value<Boolean>();
                FindHighVolatility = ((JToken) position["FindHighVolatility"]).Value<Boolean>();
                MinWalletBalance = ((JToken) position["MinWalletBalance"]).Value<decimal>();
                FindFirstLong = ((JToken) position["MinWalletBalance"]).Value<Boolean>();

                GlobalTP = position["GlobalTakeProfit"].ToObject<GlobalTakeProfit>();         
                GlobalSL = position["GlobalStopLoss"].ToObject<GlobalStopLoss>();         

                SL  = position["StopLoss"].ToObject<StopLoss>();         
                TP  = position["TakeProfit"].ToObject<TakeProfit>();         
                TSL = position["TrailingStopLoss"].ToObject<TrailingStopLoss>();        
                LC  = position["LosingCooldown"].ToObject<LosingCooldown>();         
                FC  = position["FreezeCooldown"].ToObject<FreezeCooldown>();         


            }            
        }
    }
}