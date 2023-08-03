using IwbrDaemon.Clients;

namespace IwbrDaemon.Trading
{

    public partial class Position
    {
        private string LogBase() {

            var diff = (DateTime.UtcNow - CreationTime).TotalSeconds;
            string diffStr = Utils.FmtSeconds(diff);

            var actualBalace = IwbrDb.GetActualBalance();

            //return$"{Symbol.PadRight(15)}{CreationTime.ToString("yyyyMMdd-HHmmss").PadRight(17)}{diffStr.PadRight(10)}{Type.ToString().PadRight(7)}";
            return$"[{actualBalace.ToString("0.00").PadLeft(10)}]  {Symbol.PadRight(15)}{diffStr.PadLeft(10)}{Type.ToString().PadLeft(7)} ";

        }

    }
}
