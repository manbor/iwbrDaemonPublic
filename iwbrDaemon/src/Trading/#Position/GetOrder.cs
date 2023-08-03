using IwbrDaemon.Types.Exchange;
using IwbrDaemon.Clients;

namespace IwbrDaemon.Trading
{
    public partial class Position
    {
        private Order GetOrder(long orderId, out bool status){
            Order order;

            try{
                {
                    order = Binance.GetOrder(Symbol, orderId);
                    IwbrDb.MergeOrder(order);

                    Thread.Sleep(1000);

                } while(order.Status=="NEW");

                string[] resultsWell = { "TRADE", "FILLED" };

                status = resultsWell.Contains(order.Status);

            }
            catch(Exception e){
                order = null;
                status = false;
            }

            return order;
        }
    }
}