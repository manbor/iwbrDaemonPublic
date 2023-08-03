using IwbrDaemon.Clients;
using IwbrDaemon.Trading;

namespace IwbrDaemon
{
    public partial class Context
    {

        public static void ExecuteCheckGlobalStatus()
        {
            string methodName = $"ExecuteCheckGlobalStatus".PadRight(Config.PadRightMethodName);

            if (!SchedulerEnable)
                return;

            if (MethodIsRunning())
                return;

            if (Binance.TradingPairsIsEmpty())
                return;

            if (!Binance.UserAssetsIsLoaded)
                return;

            MethodStart();

            if(Config.DebugEnable) log.Debug($"{methodName}start");

            Position.CheckGlobalStatus();

            if(Config.DebugEnable) log.Debug($"{methodName}finished");

            MethodStop();

        }
    }

}