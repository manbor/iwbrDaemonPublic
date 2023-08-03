namespace IwbrDaemon.Trading
{

    public partial class Position
    {
        public static bool IsAlreadyOpen(string symbol) {
            return  IwbrDaemon.Clients.IwbrDb.GetOpenPositions(symbol).Count() > 0;
        }

    }
}
