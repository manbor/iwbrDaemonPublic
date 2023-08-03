using System.Collections.Concurrent;
using System.Diagnostics;
using WebSocketSharp;

namespace IwbrDaemon
{
    public partial class Context
    {
        private static ConcurrentDictionary<string,bool> IsRunning = new ConcurrentDictionary<string,bool>();

        private static void MethodStart(string _key = "") {
            String key = null;

            key = _key.IsNullOrEmpty() ? _key : new StackTrace().GetFrame(1).GetMethod().Name;

            IsRunning[key] = true;
        }

        private static void MethodStop(string _key = "")
        {
            String key = null;

            key = _key.IsNullOrEmpty() ? _key : new StackTrace().GetFrame(1).GetMethod().Name;

            IsRunning[key] = false;
        }

        private static bool MethodIsRunning(string _key = "")
        {
            String key = null;

            key = !_key.IsNullOrEmpty() ? _key : new StackTrace().GetFrame(1).GetMethod().Name;

            if (!IsRunning.ContainsKey(key))
                return false;

            return IsRunning[key];
        }
    }
}
