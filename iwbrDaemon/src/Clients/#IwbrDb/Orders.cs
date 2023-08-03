using MongoDB.Driver;
using IwbrDaemon.Types.Exchange;

namespace IwbrDaemon.Clients
{
    public partial class IwbrDb
    {

        public static void MergeOrders(List<Order> orderList) {
            
            foreach (var order in orderList)    
                MergeOrder(order);
        
        }

        public static void MergeOrder(Order order) {
            var options = new ReplaceOptions { IsUpsert = true };
            
            var filter = Builders<Order>.Filter.And(
                        Builders<Order>.Filter.Eq("symbol", order.Symbol)
                        ,Builders<Order>.Filter.Eq("orderId", order.OrderId)
                    );

            var result = Collection.Orders.ReplaceOne(filter, order, options);
        }

        public static void OrdersRetention() {
            string methodName = $"OrdersRetention".PadRight(Config.PadRightMethodName);
            //-------------------------------------------------------------------------------

            var timeToStartKeep =  new DateTimeOffset(  DateTime.UtcNow.AddYears(-1) ).ToUnixTimeMilliseconds();

            var filter = Builders<Order>.Filter.And(
                Builders<Order>.Filter.Lt("time", timeToStartKeep)
            );

            Collection.Orders.DeleteMany(filter);

            if(Config.DebugEnable) log.Debug($"{methodName}done");
        }
    }
}