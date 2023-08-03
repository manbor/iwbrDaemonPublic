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
        public static string FmtSeconds(double p_seconds)
        {
            TimeSpan time = TimeSpan.FromSeconds(p_seconds);
            int hours = (int)time.TotalHours;
            int minutes = (int)time.TotalMinutes % 60;
            int seconds = (int)time.TotalSeconds % 60;

            var str = String.Empty;

            if (hours > 0)
                str += $"{hours}h";
            if (minutes > 0)
                str += $"{minutes}m";
            if (seconds > 0)
                str += $"{seconds}s";

            if(str==String.Empty) str = "0s";

            return str;
        }
    }
}
