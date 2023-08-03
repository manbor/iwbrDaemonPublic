namespace IwbrDaemon.Trading
{

    public partial class Position
    {
        public static bool MaxPositionsReached() {
            return  IwbrDaemon.Clients.IwbrDb.GetOpenPositions().Count() >= Config.Position.MaxOpenPos;
        }

    }
}
