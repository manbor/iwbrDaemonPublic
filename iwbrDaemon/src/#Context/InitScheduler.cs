using IwbrDaemon.Clients;
using IwbrDaemon.Trading;

namespace IwbrDaemon
{
    public partial class Context
    {
        public static bool SchedulerEnable { get; set; } = true;

        private static bool SchedulerIsOn = false;

        private static Timer _Reboot;
        private static Timer _tSchedulerReloadConfig;
        private static Timer _tSchedulerWebSocket;
        private static Timer _tSchedulerUserAsset;
        private static Timer _tRefreshTradingPairs;
        private static Timer _tLoadRecentKLines;
        private static Timer _tRetention;
        private static Timer _tCheckPositions;
        private static Timer _tCheckGlobalStatus;
        private static Timer _tFindNewPositions;

        private static Timer _tPrintAssets;
        public static void InitScheduler()
        {
            if (SchedulerIsOn)
                return;

            log.Info("Init scheduler");

            _tSchedulerReloadConfig = new Timer(
                e => IwbrDaemon.Config.LoadConfig(),
                null,
                TimeSpan.Zero,
                TimeSpan.FromSeconds(1)
            );

            Thread.Sleep(2000);

            _tSchedulerUserAsset = new Timer(
                e => RefreshWalletStatus(),
                null,
                TimeSpan.Zero,
                TimeSpan.FromSeconds(1)
            );

            _tRefreshTradingPairs = new Timer(
                e => RefreshTradingPairs(),
                null,
                TimeSpan.Zero,
                TimeSpan.FromMinutes(1)
            );

            Thread.Sleep(3000);

            _tLoadRecentKLines = new Timer(
                _ => LoadRecentKLines(),
                null,
                TimeSpan.Zero,
                TimeSpan.FromMinutes(IwbrDaemon.Clients.Binance.KLinesRefreshMinutes)
            );

            _tSchedulerWebSocket = new Timer(
                e => {
                        Binance.TickerWSLoader();
                        Binance.KLineWSLoader();
                    },
                null,
                TimeSpan.Zero,
                TimeSpan.FromSeconds(2)
            );

            _Reboot = new Timer(
                _ => {
                    CheckReboot();
                },
            null,
            TimeSpan.Zero,
            TimeSpan.FromSeconds(1)
            );

            _tFindNewPositions = new Timer(
                _ => {
                    ExecuteFindNewPositions();
                },
            null,
            TimeSpan.Zero,
            TimeSpan.FromSeconds(20)
            );

            _tCheckPositions = new Timer(
                _ => {
                    ExecuteCheckPositions();
                },
            null,
            TimeSpan.Zero,
            TimeSpan.FromSeconds(10)
            );

            _tCheckGlobalStatus = new Timer(
                _ => {
                    ExecuteCheckGlobalStatus();
                },
            null,
            TimeSpan.Zero,
            TimeSpan.FromSeconds(2)
            );

/*
            _tPrintAssets = new Timer(
                _ => PrintAssets(),
                null,
                TimeSpan.Zero,
                TimeSpan.FromSeconds(5)
            );
*/

            _tRetention = new Timer(
                _ => {
                IwbrDb.PositionsRetention();
                IwbrDb.WalletStatusRetention();
                IwbrDb.SymbolsAnalysisRetention();
                IwbrDb.KLinesRetention();
            },
                null,
                TimeSpan.Zero,
                TimeSpan.FromHours(1)
            );
            
            SchedulerIsOn = true;
        }


    }
}
