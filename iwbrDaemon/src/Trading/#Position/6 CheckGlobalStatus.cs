using IwbrDaemon.Clients;
using IwbrDaemon.Trading.Analysis;
using static IwbrDaemon.Utils;

namespace IwbrDaemon.Trading
{

    public partial class Position
    {
        private static bool IsClosingAllForProfit = false;

        private static int PrintPosLastMin2;
        private static int PrintPosActMin2;

        public static void CheckGlobalStatus()
        {
            if(!Config.Position.GlobalTP.IsEnabled && !Config.Position.GlobalSL.IsEnabled )
                return;

            string methodName = $"CheckGlobalStatus".PadRight(Config.PadRightMethodName);

            if(Config.DebugEnable) log.Debug($"{methodName}start");

            var openPositions = IwbrDb.GetOpenPositions();

            var maxBalance = IwbrDb.GetMaxBalance();
            var actualBalance = IwbrDb.GetActualBalance();
            var lastBalanceOnCloseAll = IwbrDb.GetLastCloseAllInfoBalance().Value;
            var balanceProfit = (actualBalance/lastBalanceOnCloseAll -1 ) * 100;

            PrintPosActMin2 = DateTime.UtcNow.Minute;
            var printLog = PrintPosLastMin2 != PrintPosActMin2 && (PrintPosActMin2 % 1 == 0);

            if(printLog)
                log.Info($"BalanceProfit: {(balanceProfit>=0?LC.Green:LC.Red)}{balanceProfit.ToString("0.00")}{LC.OFF} / LastBalanceOnCloseAll: {lastBalanceOnCloseAll.ToString("0.00")} / Actual: {actualBalance.ToString("0.00")} / MaxBalance: {maxBalance.ToString("0.00")}");

            
            bool isTP = Config.Position.GlobalTP.IsEnabled && balanceProfit >= Config.Position.GlobalTP.Value;
            bool isSL = Config.Position.GlobalSL.IsEnabled && balanceProfit <= Config.Position.GlobalSL.Value;

            if(isTP || isSL)
            {
                log.Info($"Closing all for {(isSL?"SL":"TP")} reached {(balanceProfit>=0?LC.Green:LC.Red)}{balanceProfit}{LC.OFF}%/{(isSL?Config.Position.GlobalSL.Value:Config.Position.GlobalTP.Value)}%");

                IsClosingAllForProfit = true;

                foreach(var pos in openPositions){
                    new Thread(( _ => pos.Close($"[{(isSL?"SL":"global profit")} reached {balanceProfit}%/{(isSL?Config.Position.GlobalSL.Value:Config.Position.GlobalTP.Value)}%]") )).Start();
                }

                IwbrDb.UpdateCloseAllHistory();

                IsClosingAllForProfit = false;
            }

            PrintPosLastMin2 = PrintPosActMin2;
            if(Config.DebugEnable) log.Debug($"{methodName}finished");

        }

    }
}