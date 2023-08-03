using IwbrDaemon.Clients;

namespace IwbrDaemon
{
    public partial class Context
    {
        public static void PrintAssets()
        {
            if (!SchedulerEnable)
                return;

            //if (Binance.UserAssetsIsEmpty())
            //    return;

            string line;

            line =  "Asset".PadRight(15);
            line += "Quantity".PadRight(20);
            line += Config.Binance.QuoteAsset.PadRight(20);

            log.Info(line);

            var walletStatus = IwbrDb.GetLastWalletStatus();
            var userAssets = walletStatus.Assets;

            foreach (var _ua in userAssets)
            {
                line =  _ua.Asset.PadRight(15);
                line += (_ua.Asset==Config.Binance.QuoteAsset?string.Empty:_ua.NetAsset.ToString()).PadRight(20);
                line += (_ua.Asset==Config.Binance.QuoteAsset?_ua.NetAsset:Utils.ConvertToCurrentAsset(_ua)).ToString().PadRight(20);

                log.Info(line);
            }

            log.Info($"Total: {walletStatus.Balance.Value} {walletStatus.Balance.QuoteAsset}");
        }
    }
}
