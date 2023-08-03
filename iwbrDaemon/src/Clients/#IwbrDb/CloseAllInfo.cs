using IwbrDaemon.Types.Exchange;
using IwbrDaemon.Types;
using MongoDB.Driver;

namespace IwbrDaemon.Clients
{
    public partial class IwbrDb
    {

        public static ActualBalance GetLastCloseAllInfoBalance()
        {
            try {
                var filter = FilterDefinition<CloseAllInfo>.Empty;
                var sort = Builders<CloseAllInfo>.Sort.Descending("creationTime");
                return Collection.CloseAllHistory.Find(filter).Sort(sort).First().Balance ;
            }
            catch(Exception e){
                log.Debug(e);
                var balance = GetFirstWalletStatus().Balance;

                UpdateCloseAllHistory(balance);
                log.Info("GetLastCloseAllInfoBalance: 1st entry added");
                return balance;
            }
        }

        public static void UpdateCloseAllHistory(ActualBalance balance = null)
        {
            var actualBalance = balance!=null?balance:IwbrDb.GetLastWalletStatus().Balance;
            var CloseAllInfo = new CloseAllInfo(DateTime.UtcNow, actualBalance );
            Collection.CloseAllHistory.InsertOne(CloseAllInfo);
        }


    }
}
