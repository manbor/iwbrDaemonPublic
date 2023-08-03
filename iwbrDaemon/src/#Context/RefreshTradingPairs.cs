using IwbrDaemon.Clients;

namespace IwbrDaemon
{
    public partial class Context
    {

        public static void RefreshTradingPairs()
        {
            //if (isBoot)
            //    return;

            if (!SchedulerEnable)
                return;

            if (MethodIsRunning())
                return;

            MethodStart();

            Binance.RefreshTradingPairs();
            
            MethodStop();
        }


    }
}
