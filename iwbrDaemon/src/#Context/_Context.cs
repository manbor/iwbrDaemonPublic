using System.Reflection;

namespace IwbrDaemon
{
    public partial class Context
    {
        private static System.Type thisType = MethodBase.GetCurrentMethod().DeclaringType;
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(thisType);

        public static bool isBoot = true;

        public static int UserId {get; private set;} = 1;

        static Context() {
            log.Info("Init Context");
        }
    }
}
