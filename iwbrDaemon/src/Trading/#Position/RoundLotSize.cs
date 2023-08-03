using IwbrDaemon.Clients;
using Newtonsoft.Json.Linq;

namespace IwbrDaemon.Trading
{
    public partial class Position
    {

        private decimal RoundLotSize(decimal input, ePositionAction action){

            var filters = Binance.TradingPairs
                        .Where(p => p.Symbol == Symbol)
                        .First()
                        .Filters;

            var lot_size = JArray.Parse(filters).Where(p => p["filterType"].ToString() == "LOT_SIZE").First();
            var stepSize = decimal.Parse(lot_size["stepSize"].ToString());


            if(Type== ePositionType.Long && action==ePositionAction.Open){

                return input;

            } else if(Type== ePositionType.Long && action==ePositionAction.Close){

                return Math.Floor(input / stepSize)  * stepSize ;     

            } else if(Type== ePositionType.Short && action==ePositionAction.Open){

                return Math.Ceiling(input / stepSize)  * stepSize ;     
 
            } else if(Type== ePositionType.Short && action==ePositionAction.Close){

                return Math.Ceiling(input / stepSize)  * stepSize ;     

            } else {

                return input;

            }

        }
    }
}