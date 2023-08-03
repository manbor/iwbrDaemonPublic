using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IwbrDaemon
{
    public partial class Utils
    {
        public static long GetCurrentTimestamp()
        {
            //DateTimeOffset localTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, TimeZoneInfo.FindSystemTimeZoneById("Central European Standard Time"));
            DateTime epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            long timestamp = (long)(DateTime.UtcNow - epoch).TotalSeconds * 1000;

            return timestamp;
        }
    }
}
