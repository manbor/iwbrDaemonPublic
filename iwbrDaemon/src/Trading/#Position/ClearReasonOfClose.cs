using IwbrDaemon.Clients;

namespace IwbrDaemon.Trading
{
    public partial class Position
    {

        public void ClearReasonOfClose() {
            if( string.IsNullOrEmpty(ReasonOfClose))
                ReasonOfClose = null;
        }

    }
}