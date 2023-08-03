using System.Text.RegularExpressions;
using IwbrDaemon.Clients;

namespace IwbrDaemon.Trading
{
    public partial class Position
    {

        public void ClearStrings() {
            if( string.IsNullOrEmpty(ReasonOfClose))
                ReasonOfClose = null;

            string unixColorsPattern = @"\\u001b.*?m";
            ReasonOfClose = Regex.Replace(ReasonOfClose, unixColorsPattern, String.Empty);

        }

    }
}