using IwbrDaemon.Clients;
using IwbrDaemon.Trading;

namespace IwbrDaemon
{
    public partial class Context
    {
        public static bool FindNewPositionsIsExecuting { get; private set; } = false;

        public static void ExecuteFindNewPositions()
        {
            string methodName = $"ExecuteFindNewPositions".PadRight(Config.PadRightMethodName);

            if (!SchedulerEnable)
                return;
    
            if (MethodIsRunning())
                return;

            if (Binance.TradingPairsIsEmpty() || !Binance.UserAssetsIsLoaded || !Binance.RecentKLinesLoaded || Position.MaxPositionsReached() )
                return;

            MethodStart();

            Position.FindNewPositions();

            MethodStop();

        }
    }

}