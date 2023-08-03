using IwbrDaemon.Clients;
using IwbrDaemon.Trading;

namespace IwbrDaemon
{
    public partial class Context
    {
        public static void CheckReboot()
        {
            var openPos = IwbrDb.GetOpenPositions().Count();
            var deltaSec = (int) ( IwbrDaemon.Trading.Position.DtLastUpdateExecution == default(DateTime) ? 0 : (DateTime.UtcNow - IwbrDaemon.Trading.Position.DtLastUpdateExecution).TotalSeconds );
            const int maxWait = 240;

            int minutesOn = IwbrDaemon.Program.stopwatch.Elapsed.Minutes;

            return ;
            
            if( (openPos > 0 && deltaSec >= maxWait) 
                // || minutesOn >= 30 // TODO add to config
                ) {

                // for avoid to open trades and for reboot when iwbr is not trading
                lock(Position.LockTrade){
                    log.Info("Reboot");
                    System.Environment.Exit(1);
                }
            }
        }

    }
}
