using IwbrDaemon.Clients;
using IwbrDaemon.Trading;

namespace IwbrDaemon
{
    public partial class Context
    {
        private static int PrintPosLastMin;
        private static int PrintPosActMin;

        public static void ExecuteCheckPositions()
        {
            string methodName = $"ExecuteCheckPositions".PadRight(Config.PadRightMethodName);

            if (!SchedulerEnable)
                return;

            if (MethodIsRunning())
                return;

            if (Binance.TradingPairsIsEmpty())
                return;

            if (!Binance.UserAssetsIsLoaded || !Binance.RecentKLinesLoaded || Binance.RecentKLinesIsRefreshing)
                return;

            MethodStart();

            if(Config.DebugEnable) log.Debug($"{methodName}start");

            Position.CheckOpenPositions();

            if(Config.DebugEnable) log.Debug($"{methodName}finished");

            MethodStop();

        }
    }

}