using System.Diagnostics;

namespace IwbrDaemon
{
    internal partial class Program {     
        protected static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        public static Stopwatch stopwatch {get; private set;} = new Stopwatch();

        private static void Main(string[] args)
        {
            string methodName = $"Main";

            try
            {
                _ = Activator.CreateInstance(System.Type.GetType("IwbrDaemon.Clients.Binance"));
                _ = Activator.CreateInstance(System.Type.GetType("IwbrDaemon.Clients.IwbrDb"));
                _ = Activator.CreateInstance(System.Type.GetType("IwbrDaemon.Context"));

                Context.isBoot = false;

                // keep at the end. In this order
                Context.InitScheduler();

                _ = Activator.CreateInstance(System.Type.GetType("IwbrDaemon.CLI"));

                stopwatch.Start();

                while (true)
                {
                    CLI.Prompt();
                }
            }
            catch (Exception ex)
            {
                log.Error($"{methodName}{ex}");

            }
        }
    }
}