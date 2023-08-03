using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IwbrDaemon
{
    public partial class Utils
    {
        public static DateTime UnixTimeStampToDateTime(long unixTimeStamp)
        {
            return DateTimeOffset.FromUnixTimeMilliseconds(unixTimeStamp).UtcDateTime;
        }

    }
}
