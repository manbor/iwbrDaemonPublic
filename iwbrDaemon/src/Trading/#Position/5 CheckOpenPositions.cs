using IwbrDaemon.Clients;
using IwbrDaemon.Trading.Analysis;
using static IwbrDaemon.Utils;

namespace IwbrDaemon.Trading
{

    public partial class Position
    {

        private static int PrintPosLastMin;
        private static int PrintPosActMin;

        public static void CheckOpenPositions()
        {
            string methodName = $"CheckOpenPositions".PadRight(Config.PadRightMethodName);

            if(IsClosingAllForProfit) return;

            if(Config.DebugEnable) log.Debug($"{methodName}start");

            var openPositions = IwbrDb.GetOpenPositions();
    
            PrintPosActMin = DateTime.UtcNow.Minute;
            var printLog = PrintPosLastMin != PrintPosActMin && (PrintPosActMin % 1 == 0);

            foreach(var pos in openPositions){
                new Thread((() => {
                                        var analysis = new SymbolAnalysis(pos.Symbol, DateTime.UtcNow);
                                        pos.Check(analysis,printLog);                                    
                                })).Start();
            }

            PrintPosLastMin = PrintPosActMin;
            if(Config.DebugEnable) log.Debug($"{methodName}finished");

        }

    }
}